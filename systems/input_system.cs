public static class InputSystem
{
    public static void Run(World w)
    {
        if (w.player == -1 || !(w.turn_order[w.current_turn] == w.player)) return;

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
                if (w.aux_map[nx, ny].blocks_movement > 0) {
                    foreach (int cell_id in w.game_map[nx, ny]){
                        if ((w.race.Has(cell_id) && !(w.race.Get(cell_id) == w.race.Get(w.player))) || 
                        (w.alignment.Has(cell_id) && !(w.alignment.Get(cell_id) == w.alignment.Get(w.player))))
                        {
                            // si estoy intentando moverme hacia enemigo -> atacar
                            // CODIGO REPETIDO DE AI_BEHAVIOUR... SE PUEDE HACER UNA FUNCION ---------------------------------------------------
                            int damage = IDManager.get_id();
                            AuxTypes.Damage damage_done = new AuxTypes.Damage // PLACEHOLDER
                            {
                                amount = 1,
                                type = "phys"
                            };
                            w.damage.Add(damage, damage_done);
                            w.attack_targets.Add(damage, new List<int> { cell_id });

                            string actor = "you";
                            string victim = "someone nameless";
                            if (w.name.Has(cell_id)) victim = w.name.Get(cell_id);
                            w.announcement_list.Add(actor + " attacks " + victim + "!");
                            return;
                        }
                    }
                    // si me intente mover hacia una celda ocupada y no tiene enemigos, no consumo el turno
                    continue;
                }
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

       // escribir menu
        Console.SetCursorPosition(0, Config.INPUT_Y);
        Console.Write("Pick up what?".PadRight(Console.WindowWidth));
        Console.SetCursorPosition(0, Config.INPUT_Y + 1);
        for (int i = 0; i < items.Count; i++)
        {
            string name = w.name.Has(items[i]) ? w.name.Get(items[i]) : "unknown item";
            Console.Write($"  {(char)('a'+i)}) {name}".PadRight(Console.WindowWidth));
            Console.SetCursorPosition(0, Config.INPUT_Y + 2 + i);
        }

        // esperar seleccion
        while (true)
        {
            var key = Console.ReadKey(intercept: true).Key;
            if (key == ConsoleKey.Escape)
            {
                ClearInputZone(items.Count + 1);
                return;  // cancelar
            }

            int index = key - ConsoleKey.A;  // A=0, B=1, C=2...
            if (index >= 0 && index < items.Count)
            {
                Actions.PickUp(w, w.player, items[index]);
                ClearInputZone(items.Count + 1);
                return;
            }
        }
    }

    static void HandleDrop(World w)
    {        
        // armar lista de items en el inventario
        var items = new List<int>();
        foreach (int id in w.holding.Get(w.player))
        {
            if (w.ascii.Has(id)) items.Add(id);  // solo items con representacion visual
        }

        if (items.Count == 0)
        {
            w.announcement_list.Add("There is nothing to drop.");
            return;
        }

        if (items.Count == 1)  // si hay solo 1, agarralo directo sin preguntar
        {
            Actions.Drop(w, w.player, items[0]);
            return;
        }

        // escribir menu
        Console.SetCursorPosition(0, Config.INPUT_Y);
        Console.Write("Pick up what?".PadRight(Console.WindowWidth));
        Console.SetCursorPosition(0, Config.INPUT_Y + 1);
        for (int i = 0; i < items.Count; i++)
        {
            string name = w.name.Has(items[i]) ? w.name.Get(items[i]) : "unknown item";
            Console.Write($"  {(char)('a'+i)}) {name}".PadRight(Console.WindowWidth));
            Console.SetCursorPosition(0, Config.INPUT_Y + 2 + i);
        }

        // esperar seleccion
        while (true)
        {
            var key = Console.ReadKey(intercept: true).Key;
            if (key == ConsoleKey.Escape)
            {
                ClearInputZone(items.Count + 1);
                return;  // cancelar
            }

            int index = key - ConsoleKey.A;  // A=0, B=1, C=2...
            if (index >= 0 && index < items.Count)
            {
                Actions.Drop(w, w.player, items[index]);
                ClearInputZone(items.Count + 1);
                return;
            }
        }
    }

    static void HandleInventory(World w) { }
    static void HandleMenu(World w) { }
    // limpiar menu 
    static void ClearInputZone(int lines)
    {
        for (int i = 0; i < lines; i++)
        {
            Console.SetCursorPosition(0, Config.INPUT_Y + i);
            Console.Write(new string(' ', Console.WindowWidth));
        }
    }
}