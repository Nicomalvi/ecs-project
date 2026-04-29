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
                if (delta == (0, 0)) { w.movement.Set(w.player, (0,0)); ClearInputZone(20); return; }

                var pos = w.position.Get(w.player);
                short nx = (short)(pos.x + delta.dx);
                short ny = (short)(pos.y + delta.dy);
                if (w.aux_map[nx, ny].blocks_movement > 0) {
                    foreach (int cell_id in w.game_map[nx, ny]){
                        if ((w.race.Has(cell_id) && !(w.race.Get(cell_id) == w.race.Get(w.player))) || 
                        (w.alignment.Has(cell_id) && !(w.alignment.Get(cell_id) == w.alignment.Get(w.player))))
                        {
                            Actions.MeleeAttack(w,w.player,cell_id);
                            return;
                        }
                    }
                    // si me intente mover hacia una celda ocupada y no tiene enemigos, no consumo el turno
                    continue;
                }
                w.movement.Set(w.player, delta);
                ClearInputZone(20);
                return;
            }
            // si llegue aca, la tecla no fue de movimiento
            // ACCIONES
            // deberian devolver un bool, que determina si consumir turno o no (asi drop cancelado no consume)
            if (key == ConsoleKey.G) { HandlePickup(w);    return; }   // consume turno 
            if (key == ConsoleKey.H) { HandleDrop(w);    return; }     // consume turno
            if (key == ConsoleKey.J) { HandleEquip(w);    return; }     // consume turno
            if (key == ConsoleKey.K) { HandleUnequip(w);    return; }     // consume turno
            if (key == ConsoleKey.I) { HandleInventory(w); continue; } // no consume
            if (key == ConsoleKey.Escape) { HandleMenu(w); continue; } // no consume
        }
    }
    static void HandlePickup(World w)
    {
        var pos = w.position.Get(w.player);
        var items = new List<int>();
        foreach (int id in w.game_map[pos.x, pos.y])
        {
            if (id == w.player) continue;
            if (w.ascii.Has(id)) items.Add(id); // solo puedo agarrar cosas que se ven
        }
        if (items.Count == 0)
        {
            w.announcement_list.Add("There is nothing here to pick up.");
            return;
        }
        int chosen = items.Count == 1 ? items[0] : ChooseFromList(w, items, "Pick up what?");
        // por las dudas 
        // condicion ? caso true : caso false
        if (chosen == -1) return;
        w.pending_actions.Add(w.player, new AuxTypes.PendingAction
        {
            type = AuxTypes.ActionType.pickup,
            target_ids = new List<int> { chosen }
        });
    }

    static void HandleDrop(World w)
    {
        if (!w.holding.Has(w.player)) return; // no tenes inventario
        var items = w.holding.Get(w.player)
            .Where(id => w.ascii.Has(id))
            .ToList();
        if (items.Count == 0)
        {
            w.announcement_list.Add("There is nothing to drop.");
            return;
        }
        int chosen = items.Count == 1 ? items[0] : ChooseFromList(w, items, "Drop what?");
        if (chosen == -1) return;
        w.pending_actions.Add(w.player, new AuxTypes.PendingAction
        {
            type = AuxTypes.ActionType.drop,
            target_ids = new List<int> { chosen }
        });
    }

   static void HandleInventory(World w)
    {
        ClearInputZone(20);
        if (!w.holding.Has(w.player))
        {
            WriteLine(0, "No inventory");
            return;
        }
        var inventory = w.holding.Get(w.player);
        WriteLine(0, "You have:");
        if (inventory.Count == 0)
        {
            WriteLine(1, "  (empty)");
        }
        else
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                WriteLine(1 + i, "  " + GetName(w, inventory[i]));
            }
        }
        if (!w.equipped_ids.Has(w.player)) return;
        var equipped = w.equipped_ids.Get(w.player);
        int offset = 1 + Math.Max(inventory.Count, 1);
        WriteLine(offset, "You have equipped:");
        if (equipped.Count == 0)
        {
            WriteLine(offset + 1, "  (nothing)");
        }
        else
        {
            for (int i = 0; i < equipped.Count; i++)
            {
                WriteLine(offset + 1 + i, "  " + GetName(w, equipped[i]));
            }
        }
    }
    static void HandleMenu(World w) { }
    static void HandleEquip(World w)
    {
        if (!w.holding.Has(w.player)) return; // no tenes inventario
        var items = w.holding.Get(w.player).Where(id => w.equipment_type.Has(id)).ToList();
        if (items.Count == 0)
        {
            ClearInputZone(1);
            WriteLine(0, "No equippable items!");
            return;
        }
        int chosen = items.Count == 1 ? items[0] : ChooseFromList(w, items, "Equip what?");
        if (chosen == -1) return;
        var possible = Actions.ChildrenWhoEquip(w, w.player, w.equipment_type.Get(chosen));
        if (possible.Count == 0)
        {
            ClearInputZone(1);
            WriteLine(0, "No available body parts");
            return;
        }
        w.pending_actions.Add(w.player, new AuxTypes.PendingAction
        {
            type = AuxTypes.ActionType.equip,
            target_ids = new List<int> { chosen }
        });
    }
    static void HandleUnequip(World w)
    {
        if (!w.equipped_ids.Has(w.player)) return;
        var items = w.equipped_ids.Get(w.player);
        if (items.Count == 0) return;
        int chosen = items.Count == 1 ? items[0] : ChooseFromList(w, items, "Unequip what?");
        if (chosen == -1) return;
        w.pending_actions.Add(w.player, new AuxTypes.PendingAction
        {
            type = AuxTypes.ActionType.unequip,
            target_ids = new List<int> { chosen }
        });
    }
    //=========================================================================================================
    // funciones UI
    //=========================================================================================================
    static void ClearInputZone(int lines)
    {
        for (int i = 0; i < lines; i++)
        {
            Console.SetCursorPosition(0, Config.INPUT_Y + i);
            Console.Write(new string(' ', Console.WindowWidth));
        }
    }
    static int ChooseFromList(World w, List<int> items, string title)
    {
        ClearInputZone(20);
        Console.SetCursorPosition(0, Config.INPUT_Y);
        Console.Write(title.PadRight(Console.WindowWidth));
        for (int i = 0; i < items.Count; i++)
        {
            string name = GetName(w, items[i]);
            Console.SetCursorPosition(0, Config.INPUT_Y + 1 + i);
            Console.Write($"  {(char)('a' + i)}) {name}".PadRight(Console.WindowWidth));
        }
        while (true)
        {
            var key = Console.ReadKey(intercept: true).Key;
            if (key == ConsoleKey.Escape)
            {
                ClearInputZone(items.Count + 1);
                return -1;
            }
            int index = key - ConsoleKey.A;
            if (index >= 0 && index < items.Count)
            {
                ClearInputZone(items.Count + 1);
                return items[index];
            }
        }
    }
    static void WriteLine(int offset, string text)
    {
        Console.SetCursorPosition(0, Config.INPUT_Y + offset);
        Console.Write(text.PadRight(Console.WindowWidth));
    }

    static string GetName(World w, int id)
    {
        return w.name.Has(id) ? w.name.Get(id) : "a nameless thing";
    }
}