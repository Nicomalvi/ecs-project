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
        
        // armar lista de items en la celda (sin el player)
        var items = new List<int>();
        foreach (int id in w.game_map[pos.x, pos.y])
        {
            if (id == w.player) continue;
            if (w.ascii.Has(id)) items.Add(id);  // solo items con representacion visual
        }

        if (items.Count == 0)
        {
            w.announcement_list.Add("There is nothing here to pick up.");
            return;
        }

        if (items.Count == 1)  // si hay solo 1, agarralo directo sin preguntar
        {
            Actions.PickUp(w, w.player, items[0]);
            return;
        }

        // mostrar menu de seleccion
        Console.SetCursorPosition(0, Config.HEIGHT + 1);
        Console.WriteLine("Pick up what?".PadRight(Console.WindowWidth));
        for (int i = 0; i < items.Count; i++)
        {
            string name = w.name.Has(items[i]) ? w.name.Get(items[i]) : "unknown item";
            char letter = (char)('a' + i);
            Console.WriteLine($"  {letter}) {name}".PadRight(Console.WindowWidth));
        }

        // esperar seleccion
        while (true)
        {
            var key = Console.ReadKey(intercept: true).Key;
            if (key == ConsoleKey.Escape) return;  // cancelar

            int index = key - ConsoleKey.A;  // A=0, B=1, C=2...
            if (index >= 0 && index < items.Count)
            {
                Actions.PickUp(w, w.player, items[index]);
                return;
            }
        }
    }

    static void HandleDrop(World w)
    {
        // preguntar: cual id dropear? luego Action.Drop(player, id)
    }

    static void HandleInventory(World w) { }
    static void HandleMenu(World w) { }
}