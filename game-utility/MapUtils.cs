public static class MapUtils
{
    public static void AddToMap(World w, int id, (short x, short y) pos)
    {
        w.position.Add(id, pos);
        w.movement.Add(id, (0, 0));
        w.game_map[pos.x, pos.y].Add(id);

        if (!w.map_blocks.Has(id)) return;
        var props = w.map_blocks.Get(id);
        if (props.blocks_move)
            w.aux_map[pos.x, pos.y].blocks_movement++;
        if (props.blocks_vision)
            w.aux_map[pos.x, pos.y].blocks_vision++;
    }

    public static void MoveOnMap(World w, int id, short newX, short newY)
    {
        var pos = w.position.Get(id);
        w.game_map[pos.x, pos.y].Remove(id);
        w.game_map[newX, newY].Add(id);
        if (w.map_blocks.Has(id))
        {
            var props = w.map_blocks.Get(id);
            if (props.blocks_move)  { w.aux_map[pos.x, pos.y].blocks_movement--; w.aux_map[newX, newY].blocks_movement++; }
            if (props.blocks_vision){ w.aux_map[pos.x, pos.y].blocks_vision--;   w.aux_map[newX, newY].blocks_vision++;   }
        }
        w.position.Set(id, (newX, newY));
    }

    public static void RemoveFromMap(World w, int id)
    {
        if (!w.position.Has(id)) return;
        var pos = w.position.Get(id);
        w.game_map[pos.x, pos.y].Remove(id);

        if (w.map_blocks.Has(id))
        {
            var props = w.map_blocks.Get(id);
            if (props.blocks_move)  w.aux_map[pos.x, pos.y].blocks_movement--;
            if (props.blocks_vision) w.aux_map[pos.x, pos.y].blocks_vision--;
        }

        w.position.Remove(id);
        w.movement.Remove(id);
    }
}