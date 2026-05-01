using Raylib_cs;
public static class MovementSystem
{
    public static void Run(World w)
    {
        var movementComponents = w.MovementComponent;
        for (int i = 0; i < movementComponents.dense.Count; i++) // estoy iterando sobre los que tienen w.movement
        {
            int id = movementComponents.valid_ids[i];
            // sumo a pos, chequeo si choca alguna hitbox, lo dejo en 0?
            var movementComponent = movementComponents.Get(id);
            var physicsComponent = w.PhysicsComponent.Get(id); // id = valor "real" de i

            // si choco con otra hitbox, me quedo quieto o lo mas cerca posible?
            // por ahora quieto

            float dt = Raylib.GetFrameTime(); // necesario para comportamiento independiente de frame length
            float newX = physicsComponent.x + movementComponent.vx * dt;
            float newY = physicsComponent.y + movementComponent.vy * dt;

            // offset hitboxes es 0 por ahora, no lo tengo en cuenta

            // coords en la grid
            int oldCellX = (int) physicsComponent.x / Config.CELL_SIZE; //div entera?
            int oldCellY = (int) physicsComponent.y / Config.CELL_SIZE;
            int newCellX = (int) newX / Config.CELL_SIZE; //div entera?
            int newCellY = (int) newY / Config.CELL_SIZE;

            // tamaño en tiles de la hitbox de esta entidad
            int newCellXEnd = (int)((newX + physicsComponent.width) / Config.CELL_SIZE);
            int newCellYEnd = (int)((newY + physicsComponent.height) / Config.CELL_SIZE);

            var newPhysicsComponent = physicsComponent;
            var nullMovement = new AuxTypes.MovementComponent {vx = 0, vy = 0};
            newPhysicsComponent.x = newX;
            for (int j = newCellX; j<= newCellXEnd; j++)
            {
                // chequeo colisiones hitbox si me muevo HORIZONTALMENTE
                List<int> entitiesInMap = w.GameMap[j, oldCellY];
                foreach (int entityInMap in entitiesInMap)
                {
                    if (entityInMap == id) continue;
                    var EntityphysicsComponent = w.PhysicsComponent.Get(entityInMap);
                    bool collides = CollisionCheck(newPhysicsComponent, EntityphysicsComponent);
                    if (collides)
                    {
                        w.MovementComponent.Set(id, nullMovement);
                        return;
                    }
                }
            }
            // si llegue hasta aca, no hubo colisiones en el eje X
            w.PhysicsComponent.Set(id, newPhysicsComponent);
            newPhysicsComponent.y = newY;
            for (int j = newCellY; j<= newCellYEnd; j++)
            {
                // paso por las tiles que ocupara mi hitbox si me muevo VERTICALMENTE para cheq. colision
                List<int> entitiesInMap = w.GameMap[newCellX, j];
                foreach (int entityInMap in entitiesInMap)
                {
                    if (entityInMap == id) continue;
                    var EntityphysicsComponent = w.PhysicsComponent.Get(entityInMap);
                    bool collides = CollisionCheck(newPhysicsComponent, EntityphysicsComponent);
                    if (collides)
                    {
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!1");
                        w.MovementComponent.Set(id, nullMovement);
                        return;
                    }
                }
            }

            w.PhysicsComponent.Set(id, newPhysicsComponent);
            w.MovementComponent.Set(id, nullMovement);
            MapUtils.RemovePhysicalFromMap(w, id);
            MapUtils.AddPhysicalToMap(w, id);
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