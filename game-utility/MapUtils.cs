public static class MapUtils
{
    public static void AddToHitboxMap(World w, int id)
    {
        var hitbox = w.Hitbox.Get(id);
        int WidthCells = (int) hitbox.width / Config.CELL_SIZE;
        int HeightCells = (int) hitbox.height / Config.CELL_SIZE;

        for(int i = 0; i<=WidthCells; i++)
        {
            for(int j = 0; j<=HeightCells; j++)
            {
                w.HitboxMap[(int) hitbox.x / Config.CELL_SIZE + i, 
                            (int) hitbox.y / Config.CELL_SIZE + j].Add(id);
            }
        }
    }

    public static void RemoveFromHitboxMap(World w, int id)
    {
        var hitbox = w.Hitbox.Get(id);
        int WidthCells = (int) hitbox.width / Config.CELL_SIZE;
        int HeightCells = (int) hitbox.height / Config.CELL_SIZE;

        for(int i = 0; i<=WidthCells; i++)
        {
            for(int j = 0; j<=HeightCells; j++)
            {
                w.HitboxMap[(int) hitbox.x / Config.CELL_SIZE + i, 
                          (int) hitbox.y / Config.CELL_SIZE + j].Remove(id);
            }
        }
    }
}