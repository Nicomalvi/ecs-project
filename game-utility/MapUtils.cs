public static class MapUtils
{
    public static void AddPhysicalToMap(World w, int id)
    {
        var Physics = w.Physics.Get(id);
        int WidthCells = (int) Physics.width / Config.CELL_SIZE;
        int HeightCells = (int) Physics.height / Config.CELL_SIZE;

        for(int i = 0; i<=WidthCells; i++)
        {
            for(int j = 0; j<=HeightCells; j++)
            {
                w.GameMap[(int) Physics.x / Config.CELL_SIZE + i, 
                          (int) Physics.y / Config.CELL_SIZE + j].Add(id);
            }
        }
    }

    public static void RemovePhysicalFromMap(World w, int id)
    {
        var Physics = w.Physics.Get(id);
        int WidthCells = (int) Physics.width / Config.CELL_SIZE;
        int HeightCells = (int) Physics.height / Config.CELL_SIZE;

        for(int i = 0; i<=WidthCells; i++)
        {
            for(int j = 0; j<=HeightCells; j++)
            {
                w.GameMap[(int) Physics.x / Config.CELL_SIZE + i, 
                          (int) Physics.y / Config.CELL_SIZE + j].Remove(id);
            }
        }
    }
}