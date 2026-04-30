public static class MapUtils
{
    public static void AddPhysicalToMap(World W, int id)
    {
        var PhysicsComponent = W.PhysicsComponent.Get(id);
        int WidthCells = (int) PhysicsComponent.width / Config.CELL_SIZE;
        int HeightCells = (int) PhysicsComponent.height / Config.CELL_SIZE;

        for(int i = 0; i<WidthCells; i++)
        {
            for(int j = 0; j<HeightCells; j++)
            {
                W.GameMap[(int) PhysicsComponent.x / Config.CELL_SIZE + i, 
                          (int) PhysicsComponent.y / Config.CELL_SIZE + j].Add(id);
            }
        }
    }

    public static void RemovePhysicalFromMap(World W, int id)
    {
        var PhysicsComponent = W.PhysicsComponent.Get(id);
        int WidthCells = (int) PhysicsComponent.width / Config.CELL_SIZE;
        int HeightCells = (int) PhysicsComponent.height / Config.CELL_SIZE;

        for(int i = 0; i<WidthCells; i++)
        {
            for(int j = 0; j<HeightCells; j++)
            {
                W.GameMap[(int) PhysicsComponent.x / Config.CELL_SIZE + i, 
                          (int) PhysicsComponent.y / Config.CELL_SIZE + j].Remove(id);
            }
        }
    }
}