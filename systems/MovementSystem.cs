using Raylib_cs;

public static class MovementSystem
{
    public static void Run(World w)
    {
        float dt = Raylib.GetFrameTime();

        // reseteo componentes que cambian por frame
        for (int i = 0; i < w.PhysicsComponent.dense.Count; i++)
        {
            int id = w.PhysicsComponent.valid_ids[i];
            var phys = w.PhysicsComponent.Get(id);
            phys.hasMoved = false;
            phys.deltaX   = 0;
            phys.deltaY   = 0;
            w.PhysicsComponent.Set(id, phys);
        }

        // armo de 0 el grafo de plataformas, quien esta sobre quien
        for (int i = 0; i < w.PlatformId.dense.Count; i++)
        {
            int id = w.PlatformId.valid_ids[i];
            w.PlatformId.Remove(id);
        }
        for (int i = 0; i < w.OnTopList.dense.Count; i++)
        {
            int id = w.OnTopList.valid_ids[i];
            w.OnTopList.Remove(id);
        }
        for (int i = 0; i < w.PhysicsComponent.dense.Count; i++)
        {
            int id = w.PhysicsComponent.valid_ids[i];
            // solo entidades con gravedad pueden estar "paradas sobre algo"
            if (!w.Gravity.Has(id) || !w.Gravity.Get(id)) continue;
            var phys    = w.PhysicsComponent.Get(id);
            int groundId = GetGroundId(w, phys, id);
            if (groundId == -1) continue;
            w.PlatformId.Add(id, groundId);
            // yo tengo referencia del piso, piso tiene referencia mia
            if (!w.OnTopList.Has(groundId))
                w.OnTopList.Add(groundId, new List<int>());
            w.OnTopList.Get(groundId).Add(id);
        }
        // muevo todas las unidades, si una de las que intento mover es parte de un grafo
        // voy de abajo para arriba
        for (int i = 0; i < w.MovementComponent.dense.Count; i++)
        {
            int id = w.MovementComponent.valid_ids[i];
            MoveEntity(w, id, dt);
        }
    }

    // ─────────────────────────────────────────────────────────────────────
    // ORDEN BOTTOM-UP RECURSIVO
    // ─────────────────────────────────────────────────────────────────────
    private static void MoveEntity(World w, int id, float dt)
    {
        var physics = w.PhysicsComponent.Get(id);
        if (physics.hasMoved) return; // ya fue procesado este frame, no volver a tocarlo

        // si estoy parado sobre algo con movimiento, lo muevo primero
        if (w.PlatformId.Has(id))
        {
            int groundId = w.PlatformId.Get(id);
            if (w.MovementComponent.Has(groundId))
                MoveEntity(w, groundId, dt); // recursion
        }
        // obligatoriamente esta parte del codigo es posterior a mover la plataforma
        float groundDx = 0;
        float groundDy = 0;
        if (w.PlatformId.Has(id))
        {
            int groundId = w.PlatformId.Get(id);
            var groundPhys = w.PhysicsComponent.Get(groundId);
            groundDx = groundPhys.deltaX;
            groundDy = groundPhys.deltaY;
        }
        MoveComponent(w, id, dt, groundDx, groundDy);
    }

    // ─────────────────────────────────────────────────────────────────────
    // MOVIMIENTO INDIVIDUAL + COLISIONES
    // ─────────────────────────────────────────────────────────────────────
    private static void MoveComponent(World w, int id, float dt, float groundDx, float groundDy)
    {
        var physics  = w.PhysicsComponent.Get(id);
        var movement = w.MovementComponent.Get(id);
        float oldX = physics.x;
        float oldY = physics.y;
        // sumo groundDx/Y por separado para diferenciar de velocidad propia
        float dx = movement.vx * dt + groundDx;
        float dy = movement.vy * dt + groundDy;
        if (dx > 0) { physics.facing = AuxTypes.FacingDirection.right; }
        if (dx < 0) { physics.facing = AuxTypes.FacingDirection.left; }
        int max_steps = (int)Math.Ceiling(Math.Max(Math.Abs(dx), Math.Abs(dy)));
        if (max_steps == 0)
        {   // EVITO DIVISION POR 0
            movement.vx *= 0.5f;
            physics.hasMoved = true;
            physics.deltaX   = 0;
            physics.deltaY   = 0;
            w.MovementComponent.Set(id, movement);
            w.PhysicsComponent.Set(id, physics);
            return;
        }
        float stepX = dx / max_steps;
        float stepY = dy / max_steps;
        // hago tantos pasos como maxima cant. pixeles
        for (int j = 0; j < max_steps; j++)
        {
            // si llego acá y stepX != 0 es porque sigo moviendome horizontalmente, no choqué
            physics.x += stepX;
            physics.x = Math.Clamp(physics.x, 0, Config.WIDTH - physics.width); // correcion Out of bounds
            if (stepX != 0 && CheckEntityColission(w, id, physics))
            {
                physics.x -= stepX;
                movement.vx = 0;
                stepX = 0; //ya choque, sumo 0 hasta el final del loop y no entro más a buscar colision
            }
            // si llego acá y stepY != 0 es porque sigo moviendome verticalmente, no choqué
            physics.y += stepY;
            physics.y = Math.Clamp(physics.y, 0, Config.HEIGHT - physics.height); // correccion Out of bounds
            if (stepY != 0 && CheckEntityColission(w, id, physics))
            {
                physics.y -= stepY;
                movement.vy = 0;
                stepY = 0; //ya choque, sumo 0 hasta el final del loop y no entro más a buscar colision
            }
        }
        movement.vx *= 0.5f; // friccion simulada
        // guardo cuánto me moví REALMENTE
        physics.deltaX   = physics.x - oldX;
        physics.deltaY   = physics.y - oldY;
        physics.hasMoved = true;
        w.PhysicsComponent.Set(id, physics);
        w.MovementComponent.Set(id, movement);
        MapUtils.RemovePhysicalFromMap(w, id);
        MapUtils.AddPhysicalToMap(w, id);
    }
    // ─────────────────────────────────────────────────────────────────────
    // DETECCIÓN COLISIONES
    // ─────────────────────────────────────────────────────────────────────
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