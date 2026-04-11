using static ComponentFlags;

public static class EntityTemplates
{
    public static int create_human(Components c, List<int>[,] game_map, short x, short y)
    {
        int id = IDManager.get_id();
        c.health.Add(id, 5);

        c.AddPosition(id, (x,y));
        c.AddMovement(id, (0,0)); 
        c.AddCanMove(id);
        c.AddAscii(id, '@');

        c.component_flags[id] |=  HAS_HEALTH | HAS_INVENTORY; // cosas que deberia sacar dsp

        game_map[x,y].Add(id);
        return id;
    }
    public static int create_item(Components c, List<int>[,] game_map, short x, short y, short weight, char ascii)
    {
        int id = IDManager.get_id();
        c.AddWeight(id, weight);
        c.AddAscii(id, ascii);
        c.AddPosition(id, (x,y));
        // mas tarde saco IS_ITEM de aca
        c.component_flags[id] |= IS_ITEM ;
        // interacciones con game_map van cerca de game_loop
        game_map[x,y].Add(id);
        return id;
    }
}