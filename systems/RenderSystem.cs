using static ComponentFlags;
public static class RenderSystem
{
    public static void run(List<int>[,] game_map, Components c)
    {
        for (int y = 0; y < Config.HEIGHT; y++)
        {
            string row = "";
            for (int x = 0; x < Config.WIDTH; x++)
            {
                char current_cell = '_';
                for (int i = 0; i < game_map[x, y].Count; i++)
                {
                    int id = game_map[x, y][i];
                    if ((c.component_flags[id] & HAS_ASCII) != 0)
                    {
                        current_cell = c.ascii.Get(id);
                        break; // salgo de la busqueda de ids con ascii
                    }
                }
                row += current_cell;
                row += " ";
            }
            Console.WriteLine(row);
        }
        Console.WriteLine("FIN DE LA PANTALLA");
    }
}