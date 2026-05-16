using Raylib_cs;
public static class MovementSystemV2
{
    public static void Run(World w)
    {
        // inicializo: nadie se movio aun
        for(int i = 0; i < w.MovementData.dense.Count; i++)
        {
            var moveData = w.MovementData.dense[i];
            moveData.hasMoved = false;
            w.MovementData.dense[i] = moveData;
        }
        
        float dt = Raylib.GetFrameTime();
        for(int i = 0; i < w.Movement2.dense.Count; i++)
        {
            int id = w.Movement2.valid_ids[i];
            var movement = w.Movement2.dense[i];
            var movementData = w.MovementData.Get(id);
            if (movementData.hasMoved == true){continue;} // ya lo procese 

            var position = w.Position.Get(id);
            // hago los pasos hasta llegar, me guardo toda colision
            MoveEntity(w,id,movement,position,dt);
        }
    }
    // =====================================================================
    // DETECCIÓN COLISIONES
    // =====================================================================
    private static void MoveEntity(
        // voy desde origen hasta destino, si choco con algo que me frena freno. 
        // todo choque va a una collisionList
        World w,
        int id,
        Components.Movement2 movement,
        Components.Position position,
        float dt
    )
    {
        float dx = Math.Clamp(movement.vx, -movement.max , movement.max)*dt;
        float dy = Math.Clamp(movement.vy, -movement.max, movement.max)*dt;
        // dx, dy es cuantos pixeles me voy a mover

        // CHEQUEO SI ESTOY PARADO SOBRE ALGO
        // SI LO ESTOY... LO MUEVO ANTES, ME PASO CUANTO MOVERME
        bool hasHitbox = w.Hitbox.Has(id);
        if (hasHitbox)
        {
            int groundId = GetGroundId(w,w.Hitbox.Get(id),id);
            if (groundId != -1)
            {
                // estoy parado sobre algo!!
                // lo muevo y sumo a mi movimiento cuanto se movio
                var groundPosition = w.Position.Get(groundId);
                var groundMovement = w.Movement2.Get(groundId);
                if(id == w.Player){Console.WriteLine(groundMovement.vx);}

                float initialFloorX = groundPosition.x;
                float initialFloorY = groundPosition.y;

                MoveEntity(w,groundId,groundMovement,groundPosition,dt);

                var newGroundPosition = w.Position.Get(groundId);
                float deltaX = newGroundPosition.x - initialFloorX;
                float deltaY = newGroundPosition.y - initialFloorY;
                dx += deltaX;
                dy += deltaY;
            }
        }

        // parto la distancia entera en steps, chequeo en cada step colisiones
        int max_steps = (int)Math.Ceiling(Math.Max(Math.Abs(dx), Math.Abs(dy)));
        float stepX = dx / max_steps;
        float stepY = dy / max_steps;

        List<(int,(int,int))> collisions = new List<(int, (int, int))>();

        // hitbox condicional: el movement system a veces mueve cosas sin hitbox/colisiones (ej. background animado)
        if (!hasHitbox)
        {
            position.x += dx;
            position.x = Math.Clamp(position.x, 0, Config.WIDTH); // correcion Out of bounds
            position.y += dy;
            position.y = Math.Clamp(position.y, 0, Config.HEIGHT); // correcion Out of bounds
        } else
        {
            // a partir de acá es código de entidades con hitbox, pueden tener colision
            Components.Hitbox hitbox = w.Hitbox.Get(id);
            // chequeo colisiones minimo 1 vez por si suceden mientras estoy quieto,
            // considero entonces movimiento nulo igual como movimiento?
            // collisions = collisions.Concat(CellCollisionDetection(w,id,hitbox)).ToList();
            for(int i = 0; i < max_steps; i++)
            {
                MapUtils.RemoveFromHitboxMap(w,id);
                position.x += stepX;
                position.x = Math.Clamp(position.x, 0, Config.WIDTH-hitbox.width); // correcion Out of bounds
                hitbox.x = position.x;
                w.Position.Set(id,position);
                w.Hitbox.Set(id,hitbox);
                MapUtils.AddToHitboxMap(w,id);
                var xCollisions = CellCollisionDetection(w,id,hitbox);
                if(xCollisions.Count > 0)
                {
                    // choqué contra otra hitbox en X
                    MapUtils.RemoveFromHitboxMap(w,id);
                    hitbox.x -= stepX;
                    position.x = hitbox.x;
                    stepX = 0; // no me muevo mas en esa dirección
                    w.Position.Set(id,position);
                    w.Hitbox.Set(id,hitbox);
                    MapUtils.AddToHitboxMap(w,id);
                }
                MapUtils.RemoveFromHitboxMap(w,id);
                position.y += stepY;
                position.y = Math.Clamp(position.y, 0, Config.HEIGHT-hitbox.height); // correcion Out of bounds
                hitbox.y = position.y;
                w.Position.Set(id,position);
                w.Hitbox.Set(id,hitbox);
                MapUtils.AddToHitboxMap(w,id);
                var yCollisions = CellCollisionDetection(w,id,hitbox);
                if(yCollisions.Count > 0)
                {
                    // choqué contra otra hitbox en Y
                    MapUtils.RemoveFromHitboxMap(w,id);
                    hitbox.y -= stepY;
                    position.y = hitbox.y;
                    stepY = 0; // no me muevo mas en esa dirección
                    w.Position.Set(id,position);
                    w.Hitbox.Set(id,hitbox);
                    MapUtils.AddToHitboxMap(w,id);
                }

                // RECORDAR POSITION =/= HITBOX, SIN ESTO SE ME ROMPIO
                collisions = collisions.Concat(xCollisions).ToList();
                collisions = collisions.Concat(yCollisions).ToList(); 
                if (stepX == 0 && stepY == 0){i = max_steps;}// corto el loop
            }
        }
        // aca puedo multiplicar y que exista "friccion" o dejar en 0 para precisión
        movement.vx*=0.2f;
        movement.vy*=0.2f;
        // PLACEHOLDER!!!!!!!!!!!!!!!!!!
        Components.MovementData moveData = new Components.MovementData{hasMoved = true};
        FinishMovement(w,id,position,movement,collisions,moveData);
    }

    private static List<(int,(int,int))> CellCollisionDetection(
        // estoy parado en una CELL del mapa, devuelvo a quienes choque
        World w,
        int id,
        Components.Hitbox hitbox 
        )
    {
        List<(int,(int,int))> collisionList = new List<(int, (int, int))>();
        int startX = (int)(hitbox.x / Config.CELL_SIZE);
        int endX   = (int)((hitbox.x + hitbox.width) / Config.CELL_SIZE);
        int startY = (int)(hitbox.y / Config.CELL_SIZE);
        int endY   = (int)((hitbox.y + hitbox.height) / Config.CELL_SIZE);
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                var list = w.HitboxMap[x, y];
                foreach (int other in list)
                {
                    if (other == id || collisionList.Any(val => val.Item1 == other)) continue;
                    // si ya choque en esta celda no me tengo en cuenta de vuelta
                    var ohterHitbox = w.Hitbox.Get(other);
                    (bool collided, var collisionVector) = HitboxCollision(hitbox, ohterHitbox);
                    if(collided)
                    {
                        if(id != w.Player)
                        {
                            Console.WriteLine("piso coli ");
                        }
                        collisionList.Add((other,collisionVector));
                    }
                }
            }
        }
        return collisionList;
    }
    private static (bool, (int,int)) HitboxCollision(Components.Hitbox A, Components.Hitbox B)
    {
        (int,int) normalColision = (0,0);
        if (A.x < B.x + B.width  &&
            A.x + A.width > B.x  &&
            A.y < B.y + B.height &&
            A.y + A.height > B.y)
        {
            // idea mental: "el minimo vértice más a la derecha - el máximo vértice más a la izq"
            float overlapHorizontal = Math.Max(0, Math.Min(A.x + A.width, B.x + B.width) - Math.Max(A.x, B.x));
            float overlapVertical = Math.Max(0, Math.Min(A.y + A.height, B.y + B.height) - Math.Max(A.y, B.y));

            if(overlapHorizontal < overlapVertical) // elijo forma MAS PEQUEÑA de separarlos en el futuro
            {
                // la normal de la colision es horizontal
                if(A.x < B.x)// chequeo si A esta a la izq o derecha
                {normalColision = (-1,0);} else{normalColision = (1,0);}
            } else
            {
                // la normal de la colision es vertical
                if(A.y < B.y) // chequeo si A esta arriba o abajo
                {normalColision = (0,-1);} else {normalColision = (0,1);}
            }
            return (true,normalColision);
        }
        return (false,normalColision);
    }
    public static int GetGroundId(World w, Components.Hitbox hitbox, int id)
    {
        var alteredHitbox = hitbox;
        alteredHitbox.y -= 1; // voy a mirar un pixel abajo si hay alguien
        var floorList = CellCollisionDetection(w,id,alteredHitbox);
        if (floorList.Count == 0){return -1;}
        return floorList[0].Item1;
    }

    private static void FinishMovement(World w, int id, Components.Position newPosition, Components.Movement2 movement, List<(int,(int,int))> collisionList, 
    Components.MovementData moveData)
    {
    if(w.Hitbox.Has(id)){
            w.CollisionList.Add(id,collisionList);
            var hitbox = w.Hitbox.Get(id);
            MapUtils.RemoveFromHitboxMap(w,id);
            hitbox.x = newPosition.x;
            hitbox.y = newPosition.y;
            w.Hitbox.Set(id, hitbox);
            MapUtils.AddToHitboxMap(w,id);
        }
        w.MovementData.Set(id,moveData);
        w.Movement2.Set(id,movement);
        w.Position.Set(id,newPosition);
    }
}