using Raylib_cs;

public static class MovementSystem
{
    public static void Run(World w)
    {
        float dt = Raylib.GetFrameTime();
        // IMPORTANTISIMO 
        // si multiplico velocidad por frame time, la velocidad no depende de los FPS
        // la velocidad pasa a depender del TIEMPO (bien!!)
        // sino, alguien corriendo el juego a 120fps juega distinto a alguien con 30fps
        for (int i = 0; i < w.MovementComponent.dense.Count; i++)
        {
            int id = w.MovementComponent.valid_ids[i];
            var movement = w.MovementComponent.Get(id);
            var physics = w.PhysicsComponent.Get(id);
            float initialX = physics.x;
            var stateComponent = w.StateComponent.Get(id);

            // las entidades afectadas por la gravedad se dejan llevar por las plataformas debajo
            // problema: deberia chequear si plataforma se movió primero y ver delta
            float groundHorizontalSpeed = 0;
            float groundVerticallSpeed = 0;
            bool onPlatform = false;
            if (w.Gravity.Has(id) && w.Gravity.Get(id) == true)
            {
                int ground = GetGroundId(w,physics,id);
                // sumo velocidad del piso por separado, si aplico directamente al movement
                // habria velocidades que crecen exponencialmente
                if(ground!=-1 && w.MovementComponent.Has(ground)) // si estoy parado sobre algo con movimiento...
                {
                    var groundMovement = w.MovementComponent.Get(ground);
                    groundHorizontalSpeed = groundMovement.vx;
                    groundVerticallSpeed  = groundMovement.vy;
                    onPlatform = true;
                }   
                else if (ground == -1)
                {    
                    stateComponent.state = AuxTypes.EntityStates.falling;
                }
            }
            
            float dx = (movement.vx + groundHorizontalSpeed) * dt;
            if (dx > 0) {physics.facing = AuxTypes.FacingDirection.right;}
            if (dx < 0) {physics.facing = AuxTypes.FacingDirection.left;} 
            // sino mantengo facing viejo

            // si tan solo ME INTENTO mover a algun lado, paso a mirar ese lado
            float dy = (movement.vy + groundVerticallSpeed) * dt;
            int max_steps = (int)Math.Ceiling(Math.Max(Math.Abs(dx), Math.Abs(dy)));
            if (max_steps == 0)
            {   // EVITO DIVISON POR 0
                movement.vx *= 0.5f;
                stateComponent.state = AuxTypes.EntityStates.idle;
                w.StateComponent.Set(id, stateComponent);
                w.MovementComponent.Set(id, movement);
                w.PhysicsComponent.Set(id, physics);
                continue;
            } 
            float stepX = dx/max_steps;
            float stepY = dy/max_steps;
            // hago tantos pasos como maxima cant. pixeles
            for (int j = 0; j<max_steps; j++)
            {
                // si llego acá y stepX != 0 es porque sigo moviendome horizontalmente , no choqué
                // CHEQUEO: si estaba en idle, paso a moverme
                //          si ya me estaba moviendo en esta dir, sigo moviendome
                //          si ya me estaba moviendo en otra dir, me doy vuelta
                //          si estaba en el aire, sigo en el aire
                physics.x += stepX;
                physics.x = Math.Clamp(physics.x, 0, Config.WIDTH - physics.width); // correcion Out of bounds
                if(stepX != 0 && CheckEntityColission(w,id,physics))
                {
                    physics.x -= stepX;
                    movement.vx = 0; 
                    stepX = 0; //ya choque, sumo 0 hasta el final del loop y no entro más a buscar colision
                }
                // si llego acá y stepY != 0 es porque sigo moviendome verticalmente , no choqué
                // CHEQUEO: si estaba en idle, paso a estar en el aire
                //          si ya estaba en el aire, sigo en el aire
                physics.y += stepY;
                physics.y = Math.Clamp(physics.y, 0, Config.HEIGHT - physics.height); // correccion Out of bounds
                if(stepY != 0 && CheckEntityColission(w,id,physics))
                {
                    physics.y -= stepY;
                    movement.vy = 0; 
                    stepY = 0; //ya choque, sumo 0 hasta el final del loop y no entro más a buscar colision
                }
            }

            movement.vx *= 0.5f; // friccion simulada

            if (stateComponent.state != AuxTypes.EntityStates.falling && 
                stateComponent.state != AuxTypes.EntityStates.jump)
            {
                if (movement.vx == 0) {stateComponent.state = AuxTypes.EntityStates.idle;}
                // no estoy en el aire + no me intenté mover = si o si estoy idle
                else if (initialX != physics.x) {stateComponent.state = AuxTypes.EntityStates.walk;}
                // no estoy en el aire + me intenté mover + me moví = estoy caminando
            } else if (stateComponent.state == AuxTypes.EntityStates.falling && onPlatform)
            {
                if (movement.vx == 0) {stateComponent.state = AuxTypes.EntityStates.idle;}
                else if (initialX != physics.x) {stateComponent.state = AuxTypes.EntityStates.walk;}
            }

            w.StateComponent.Set(id, stateComponent);
            w.PhysicsComponent.Set(id, physics);
            w.MovementComponent.Set(id, movement);
            MapUtils.RemovePhysicalFromMap(w, id);
            MapUtils.AddPhysicalToMap(w, id);
        }
    }
    // ============================================================
    // DETECCIÓN COLISIONES
    // ============================================================
    private static bool CheckEntityColission(
        // true si la entidad que pasé esta chocando con otra
        World w,
        int id,
        AuxTypes.PhysicsComponent phys
    )
    {
        int startX = (int)(phys.x / Config.CELL_SIZE);
        int endX   = (int)((phys.x + phys.width) / Config.CELL_SIZE);
        int startY = (int)(phys.y / Config.CELL_SIZE);
        int endY   = (int)((phys.y + phys.height) / Config.CELL_SIZE);

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                var list = w.GameMap[x, y];
                foreach (int other in list)
                {
                    if (other == id) continue;
                    var otherPhys = w.PhysicsComponent.Get(other);
                    if (!CollisionCheck(phys, otherPhys)) continue;
                    return true;
                }
            }
        }
        return false;
    }
    public static bool CollisionCheck(AuxTypes.PhysicsComponent A, AuxTypes.PhysicsComponent B)
    {
        return  A.x < B.x + B.width  &&
                A.x + A.width > B.x  &&
                A.y < B.y + B.height &&
                A.y + A.height > B.y;
    }
    public static int GetGroundId(
        World w, 
        AuxTypes.PhysicsComponent phys,
        int id
        )
    // chequeo si el componente fisico esta arriba de otro
    // equivalente a estar en mismo x, y-1 que otra hitbox
    {
        float epsilon = 1f;     // comparo lo que este a 1 pixel
        var probe = phys;       // una "probe" representa hitbox desplazada hacia abajo
        probe.y -= epsilon;

        int startX = (int)(probe.x / Config.CELL_SIZE);
        int endX   = (int)((probe.x + probe.width) / Config.CELL_SIZE);
        int cellY  = (int)(probe.y / Config.CELL_SIZE);

        int bestId = -1;
        float bestTop = float.NegativeInfinity;

        for (int x = startX; x <= endX; x++)
        {
            var list = w.GameMap[x, cellY];
            foreach (int other in list)
            {
                if (other == id) continue;
                var otherPhys = w.PhysicsComponent.Get(other);
                if (!CollisionCheck(probe, otherPhys)) continue;
                float top = otherPhys.y + otherPhys.height;
                // no voy a rozar todos los pisos por la misma cantidad exacta de pixeles
                // me quedo con el mas alto
                if (top > bestTop)
                {
                    bestTop = top;
                    bestId = other;
                }
            }
        }
        return bestId;
    }
}