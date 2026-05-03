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
            physics.x += dx;
            if (ResolveCollisions(w, id, ref physics, movement, axisX: true))
            {
                movement.vx = 0;
            }
            w.PhysicsComponent.Set(id, physics);
            // ============================================================
            // MOVER EN Y
            // ============================================================
            physics.y += dy;
            if (ResolveCollisions(w, id, ref physics, movement, axisX: false))
            {
                movement.vy = 0;
            }
            w.PhysicsComponent.Set(id, physics);

            // ============================================================
            // ACTUALIZAR MAPA
            // ============================================================
            movement.vx = movement.vx/2;
            movement.vy = movement.vy/2;
            w.MovementComponent.Set(id, movement);
            MapUtils.RemovePhysicalFromMap(w, id);
            MapUtils.AddPhysicalToMap(w, id);
        }
    }

    // ============================================================
    // RESOLUCIÓN COLISIONES
    // ============================================================

    private static bool ResolveCollisions(
        World w,
        int id,
        ref AuxTypes.PhysicsComponent phys,
        AuxTypes.MovementComponent movement,
        bool axisX
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
                    // ====================================================
                    // CORRECCIÓN SEGÚN EJE
                    // ====================================================
                    if (axisX)
                    {
                        if (movement.vx > 0) // derecha
                            phys.x = otherPhys.x - phys.width;
                        else if (movement.vx < 0) // izquierda
                            phys.x = otherPhys.x + otherPhys.width;
                    }
                    else
                    {
                        if (movement.vy > 0) // sube
                            phys.y = otherPhys.y - phys.height;
                        else if (movement.vy < 0) // baja
                            phys.y = otherPhys.y + otherPhys.height;
                    }
                    return true; // colisión encontrada
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
}