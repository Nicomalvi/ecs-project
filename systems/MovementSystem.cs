using Raylib_cs;
public static class MovementSystem
{
    public static void Run(World W)
    {
        var movementComponents = W.MovementComponent;
        for (int i = 0; i < movementComponents.dense.Count; i++) // estoy iterando sobre los que tienen w.movement
        {
            int id = movementComponents.valid_ids[i];
            // sumo a pos, chequeo si choca alguna hitbox, lo dejo en 0?
            var movementComponent = movementComponents.Get(id);
            var physicsComponent = W.PhysicsComponent.Get(id); // id = valor "real" de i

            // si choco con otra hitbox, me quedo quieto o lo mas cerca posible?
            // por ahora quieto

            float dt = Raylib.GetFrameTime();
            float newX = physicsComponent.x + movementComponent.vx;
            float newY = physicsComponent.y + movementComponent.vy;

            // offset hitboxes es 0 por ahora, no lo tengo en cuenta

            // coords en la grid
            int oldCellX = (int) physicsComponent.x / Config.CELL_SIZE; //div entera?
            int oldCellY = (int) physicsComponent.y / Config.CELL_SIZE;
            int newCellX = (int) newX / Config.CELL_SIZE; //div entera?
            int newCellY = (int) newY / Config.CELL_SIZE;

            // tamaño en tiles de la hitbox de esta entidad
            int newCellXEnd = (int)((newX + physicsComponent.width) / Config.CELL_SIZE);

            for (int j = 0; j<= newCellXEnd; j++)
            {
                // paso por las tiles que ocupara mi hitbox si me muevo horizontalmente para cheq. colision
                List<int> entitiesInMap = W.GameMap[newCellX + j, oldCellY];
                foreach (int entityInMap in entitiesInMap)
                {
                    if (entityInMap == id) continue;
                    var EntityphysicsComponent = W.PhysicsComponent.Get(entityInMap);
                    bool collides = CollisionCheck(physicsComponent, EntityphysicsComponent);
                    if (collides)
                    {
                        W.MovementComponent.Remove(id);
                        return;
                    }
                }
            }
            // si llegue hasta aca, no hubo colisiones en el eje X
            W.MovementComponent.Remove(id);

            physicsComponent.x = newX;
            W.PhysicsComponent.Set(id, physicsComponent);
            MapUtils.RemovePhysicalFromMap(W, id);
            MapUtils.AddPhysicalToMap(W, id);
        }
    }

    public static bool CollisionCheck(AuxTypes.PhysicsComponent A, AuxTypes.PhysicsComponent B)
    {
        return  A.x < B.x + B.width  &&
                A.x + A.width > B.x  &&
                A.y < B.y + B.height &&
                A.y + A.height > B.y;
    }
}