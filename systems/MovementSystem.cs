using Raylib_cs;

public static class MovementSystem
{
    public static void Run(World w)
    {
        float dt = Raylib.GetFrameTime();
        var movementComponents = w.MovementComponent;
        for (int i = 0; i < movementComponents.dense.Count; i++)
        {
            int id = movementComponents.valid_ids[i];
            var movement = movementComponents.Get(id);
            var physics = w.PhysicsComponent.Get(id);
            float dx = movement.vx * dt;
            float dy = movement.vy * dt;
            // ============================================================
            // MOVER EN X
            // ============================================================
            physics.x += dx; // PODRIA SUMAR DE A PARTES EN VEZ DE TODO DE GOLPE
            // correccion out of bounds
            if(physics.x >= Config.WIDTH){physics.x = Config.WIDTH - Config.CELL_SIZE;}
            if(physics.x < 0){physics.x = 0 + Config.CELL_SIZE;}

            var targetX = physics.x;
            physics = CollisionCorrectedPhys(w, id, physics, movement, horizontalMovement:true);
            if (Math.Abs(targetX - physics.x) > 0.0001f) {movement.vx = 0;} // si existe un pequeño cambio, hubo una colisión
            w.PhysicsComponent.Set(id, physics);
            // ============================================================
            // MOVER EN Y
            // ============================================================
            physics.y += dy; // PODRIA SUMAR DE A PARTES EN VEZ DE TODO DE GOLPE
            // correccion out of bounds
            if(physics.y >= Config.HEIGHT){physics.y = Config.HEIGHT - Config.CELL_SIZE;}
            if(physics.y < 0){physics.y = 0 + Config.CELL_SIZE;}

            var targetY = physics.y;
            physics = CollisionCorrectedPhys(w, id, physics, movement, horizontalMovement:false);
            if (Math.Abs(targetY - physics.y) > 0.0001f) {movement.vy = 0;} // si existe un pequeño cambio, hubo una colisión
            w.PhysicsComponent.Set(id, physics);
            // ============================================================
            // ACTUALIZAR MAPA
            // ============================================================
            // pequeña simulacion de friccion (solo en eje x)
            movement.vx *= 0.5f;
            // movement.vy *= 0.5f; 
            w.MovementComponent.Set(id, movement);
            MapUtils.RemovePhysicalFromMap(w, id);
            MapUtils.AddPhysicalToMap(w, id);
        }
    }

    // ============================================================
    // RESOLUCIÓN COLISIONES
    // ============================================================
    // esto se puede romper: por ejemplo chequeo colision contra caja 0, no hay, chequeo contra caja 1, correccion me mete en caja 0
    // con varias pasadas quizas entro en un loop infinito, numero limitado de pasadas + destrabar si quede adentro de algo?
    private static AuxTypes.PhysicsComponent CollisionCorrectedPhys(
        World w,
        int id,
        AuxTypes.PhysicsComponent phys,
        AuxTypes.MovementComponent movement,
        bool horizontalMovement
    )
    {
        int startX = (int)(phys.x / Config.CELL_SIZE);
        int endX   = (int)((phys.x + phys.width) / Config.CELL_SIZE);
        int startY = (int)(phys.y / Config.CELL_SIZE);
        int endY   = (int)((phys.y + phys.height) / Config.CELL_SIZE);
        var newPhys = phys;
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                var list = w.GameMap[x, y];
                foreach (int other in list)
                {
                    if (other == id) continue;
                    var otherPhys = w.PhysicsComponent.Get(other);
                    if (!CollisionCheck(newPhys, otherPhys)) continue;
                    // ====================================================
                    // CORRECCIÓN SEGÚN EJE
                    // ====================================================
                    if (horizontalMovement)
                    {
                        if (movement.vx > 0) // derecha
                            newPhys.x = otherPhys.x - newPhys.width;
                        else if (movement.vx < 0) // izquierda
                            newPhys.x = otherPhys.x + otherPhys.width;
                    }
                    else
                    {
                        if (movement.vy > 0) // sube
                            newPhys.y = otherPhys.y - newPhys.height;
                        else if (movement.vy < 0) // baja
                            newPhys.y = otherPhys.y + otherPhys.height;
                    }
                }
            }
        }
        return newPhys;
    }

    public static bool CollisionCheck(AuxTypes.PhysicsComponent A, AuxTypes.PhysicsComponent B)
    {
        return  A.x < B.x + B.width  &&
                A.x + A.width > B.x  &&
                A.y < B.y + B.height &&
                A.y + A.height > B.y;
    }
}