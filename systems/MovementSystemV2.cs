using Raylib_cs;
public static class MovementSystemV2
{
    public static void Run(World w)
    {
        float dt = Raylib.GetFrameTime();
        for(int i = 0; i < w.Movement2.dense.Count; i++)
        {
            int id = w.Movement2.valid_ids[i];
            var movement = w.Movement2.dense[i];
            var position = w.Position.Get(id);
            // hago los pasos hasta llegar, me guardo toda colision
            (var newPosition, var noMovement, var collisionList) = MoveEntity(w,id,movement,position,dt);
            if(w.Hitbox.Has(id)){
                w.CollisionList.Add(id,collisionList);
                var hitbox = w.Hitbox.Get(id);
                MapUtils.RemoveFromHitboxMap(w,id);
                hitbox.x = newPosition.x;
                hitbox.y = newPosition.y;
                w.Hitbox.Set(id, hitbox);
                MapUtils.AddToHitboxMap(w,id);
            }
            w.Movement2.Set(id,noMovement);
            w.Position.Set(id,newPosition);
        }
    }
    // =====================================================================
    // DETECCIÓN COLISIONES
    // =====================================================================
    private static (Components.Position, Components.Movement2, List<(int,(int,int))>) MoveEntity(
        // voy desde origen hasta destino, devuelvo toda entidad que choque en el medio y desde que lado choque
        World w,
        int id,
        Components.Movement2 movement,
        Components.Position position,
        float dt
    )
    {
        float dx = Math.Clamp(-movement.max, movement.vx, movement.max)*dt;
        float dy = Math.Clamp(-movement.max, movement.vy, movement.max)*dt;

        // parto la distancia entera en steps, chequeo en cada step colisiones
        int max_steps = (int)Math.Ceiling(Math.Max(Math.Abs(dx), Math.Abs(dy)));
        float stepX = dx / max_steps;
        float stepY = dy / max_steps;

        List<(int,(int,int))> collisions = new List<(int, (int, int))>();

        // hitbox condicional: el movement system a veces mueve cosas sin hitbox/colisiones (ej. background animado)
        bool hasHitbox = w.Hitbox.Has(id);
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
            // considero entonces movimiento nulo igual como movimiento
            collisions = collisions.Concat(CellCollisionDetection(w,id,hitbox)).ToList();
            for(int i = 0; i < max_steps; i++)
            {
                position.x += stepX;
                position.x = Math.Clamp(position.x, 0, Config.WIDTH-hitbox.width); // correcion Out of bounds
                position.y += stepY;
                position.y = Math.Clamp(position.y, 0, Config.HEIGHT-hitbox.height); // correcion Out of bounds

                // OJO podria chequear colision 2 veces por paso: 1 en x otra en y
                // voy armando en cada paso lista de colisiones ordenada
                collisions = collisions.Concat(CellCollisionDetection(w,id,hitbox)).ToList();
            }
        }
        // aca puedo dividir y que exista "friccion"
        movement.vx = 0;
        movement.vy = 0;
        return (position, movement, collisions);
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
                    if (other == id) continue;
                    var ohterHitbox = w.Hitbox.Get(other);
                    (bool collided, var collisionVector) = HitboxCollision(hitbox, ohterHitbox);
                    if(collided)
                    {
                        collisionList.Add((other,collisionVector));
                    }
                }
            }
        }
        return collisionList;
    }
    public static (bool, (int,int)) HitboxCollision(Components.Hitbox A, Components.Hitbox B)
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
                if(A.x <= B.x)// chequeo si A esta a la izq o derecha
                {normalColision = (-1,0);} else{normalColision = (1,0);}
            } else
            {
                // la normal de la colision es vertical
                if(A.y <= B.y) // chequeo si A esta arriba o abajo
                {normalColision = (0,1);} else {normalColision = (0,-1);}
            }
            Console.WriteLine("col!!");
            return (true,normalColision);
        }
        return (false,normalColision);
    }
}