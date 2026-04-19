public static class InputSystem
{
    public static void Run(World w)
    {
        if (w.player == -1) return;

        while (true)
        {
            var key = Console.ReadKey(intercept: true).Key;

            // MOVIMIENTO
            (short dx, short dy) delta = key switch
            {   // medio feo, puedo cambiar por diccionarios por accion (movimiento, inventario, menu...)
                ConsoleKey.W or ConsoleKey.UpArrow    => (0,  -1),
                ConsoleKey.S or ConsoleKey.DownArrow  => (0,   1),
                ConsoleKey.A or ConsoleKey.LeftArrow  => (-1,  0),
                ConsoleKey.D or ConsoleKey.RightArrow => (1,   0),
                ConsoleKey.Q                          => (-1, -1),
                ConsoleKey.E                          => (1,  -1),
                ConsoleKey.Z                          => (-1,  1),
                ConsoleKey.C                          => (1,   1),
                ConsoleKey.OemPeriod                  => (0,   0),
                _                                     => (-99,-99)
            };

            if (delta != (-99, -99))
            {
                if (delta == (0, 0)) { w.movement.Set(w.player, (0,0)); return; }

                var pos = w.position.Get(w.player);
                short nx = (short)(pos.x + delta.dx);
                short ny = (short)(pos.y + delta.dy);
                if (w.aux_map[nx, ny].blocks_movement > 0) continue;

                w.movement.Set(w.player, delta);
                return;
            }
            // si llegue aca, la tecla no fue de movimiento
            // ACCIONES
            if (key == ConsoleKey.G) { HandlePickup(w);    return; }   // consume turno
            if (key == ConsoleKey.H) { HandleDrop(w);    return; }     // consume turno
            if (key == ConsoleKey.I) { HandleInventory(w); continue; } // no consume
            if (key == ConsoleKey.Escape) { HandleMenu(w); continue; } // no consume
        }
    }
    // ESTAS COSAS DEBERIAN PODERLAS HACER TODOS, NO SOLO EL PLAYER ==========================================================================
    static void HandlePickup(World w)
    {
        var pos = w.position.Get(w.player);
        List<int> items_in_cell = new List<int>(w.game_map[pos.x, pos.y]); // debo copiar la lista sino la edito mientras itero sobre ella
        foreach (int id in items_in_cell)
        {
            if (id == w.player) continue;
            //if (!w.ascii.Has(id)) continue; el ascii no deberia determinar si puedo agarrar
            w.holding.Get(w.player).Add(id);
            MapUtils.RemoveFromMap(w,id);
            w.held_by.Add(id, w.player);
        }
    }

    static void HandleDrop(World w)
    {
        if (!w.holding.Has(w.player)) return;
        var inv = w.holding.Get(w.player);
        if (inv.Count == 0) return;
        if (!w.position.Has(w.player)) return;
        int item = inv[0];
        inv.RemoveAt(0);
        w.held_by.Remove(item); // CLASES SE PASAN POR REFERENCIA! NO TENGO QUE AÑADIR DE VUELTA
        MapUtils.AddToMap(w, item, w.position.Get(w.player));
    }

    static void HandleInventory(World w) { }
    static void HandleMenu(World w) { }
}