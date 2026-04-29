public static class MapUtils
{
    public static void MapInit(World w)
    {
        for (int i = 0; i< Config.WIDTH; i++)
        {
            int wall = IDManager.get_id();
            w.map_blocks.Add(wall, (true,true));
            w.ascii.Add(wall, '#');
            w.game_map[i, 0].Add(wall);
            w.aux_map[i, 0] = (w.aux_map[i,0].blocks_movement + 1, w.aux_map[i,0].blocks_vision + 1); // se podria hacer mas limpio

            int wall2 = IDManager.get_id();
            w.map_blocks.Add(wall2, (true,true));
            w.ascii.Add(wall2, '#');
            w.game_map[i, Config.HEIGHT - 1].Add(wall2);
            w.aux_map[i, Config.HEIGHT - 1] = (w.aux_map[i,Config.HEIGHT - 1].blocks_movement + 1, w.aux_map[i,Config.HEIGHT - 1].blocks_vision + 1);

            if (i == 18) continue;
            int wall3 = IDManager.get_id();
            w.map_blocks.Add(wall3, (true,true));
            w.ascii.Add(wall3, '#');
            w.game_map[i, 14].Add(wall3);
            w.aux_map[i, 14] = (w.aux_map[i,14].blocks_movement + 1, w.aux_map[i,14].blocks_vision + 1);
        }

        for (int i = 1; i< 6; i++)
        {
            int wall = IDManager.get_id();
            w.map_blocks.Add(wall, (true,true));
            w.ascii.Add(wall, '#');
            w.game_map[i, 4].Add(wall);
            w.aux_map[i, 4] = (w.aux_map[i,4].blocks_movement + 1, w.aux_map[i,4].blocks_vision + 1); // se podria hacer mas limpio
        }

        for (int j = 0; j< Config.HEIGHT; j++)
        {
            int wall = IDManager.get_id();
            w.map_blocks.Add(wall, (true,true));
            w.ascii.Add(wall, '#');
            w.game_map[0, j].Add(wall);
            w.aux_map[0, j] = (w.aux_map[0,j].blocks_movement + 1, w.aux_map[0,j].blocks_vision + 1);

            int wall2 = IDManager.get_id();
            w.map_blocks.Add(wall2, (true,true));
            w.ascii.Add(wall2, '#');
            w.game_map[Config.WIDTH - 1, j].Add(wall2);
            w.aux_map[Config.WIDTH - 1, j] = (w.aux_map[Config.WIDTH - 1,j].blocks_movement + 1, w.aux_map[Config.WIDTH - 1,j].blocks_vision + 1);
        }
    }
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