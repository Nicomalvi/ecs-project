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
                if (delta == (0, 0)) { w.movement.Set(w.player, (0,0)); ClearInputZone(20); return; }

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
    // ESTAS COSAS DEBERIAN PODERLAS HACER TODOS, NO SOLO EL PLAYER ==========================================================================
    static void HandlePickup(World w)
    {
        ClearInputZone(20);
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
        ClearInputZone(20);     
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
        Console.Write("Drop what?".PadRight(Console.WindowWidth));
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

    static void HandleInventory(World w)
    {
        ClearInputZone(20);
        if (!w.holding.Has(w.player))
        {
            Console.SetCursorPosition(0, Config.INPUT_Y);
            Console.Write("No inventory");
            return;
        } 
        var inventory = w.holding.Get(w.player);
        Console.SetCursorPosition(0, Config.INPUT_Y);
        Console.Write("You have:");
        // FALTA CASO INVENTARIO VACIO
        for(int i = 0; i<inventory.Count; i++)
        {
            string name = "a nameless thing";
            if(w.name.Has(inventory[i])) {name = w.name.Get(inventory[i]);}
            Console.SetCursorPosition(0, Config.INPUT_Y+1+i);
            Console.Write(name);
        }
        return;
    }
    static void HandleMenu(World w) { }
    static void HandleEquip(World w)
    {
        ClearInputZone(20); 
        // listo los items equipables
        // pregunto cual queres equipar
        // recien ahi chequeo si es posible (mas adelante se puede resolver antes)
        if(!w.holding.Has(w.player))
        {
            Console.SetCursorPosition(0, Config.INPUT_Y);
            Console.Write("No inventory, can't store equipment!"); //raro, quizas podria equipar cosas del piso
            return;
        }
        // armar lista de items en el inventario
        var items = new List<int>();
        foreach (int id in w.holding.Get(w.player))
        {
            if (w.equipment_type.Has(id)) items.Add(id);  // solo items equipables
        }

        if (items.Count == 0)
        {

            Console.SetCursorPosition(0, Config.INPUT_Y);
            Console.Write("No equippable items!"); //raro, quizas podria equipar cosas del piso
            return;
        }

        if (items.Count == 1)  // si hay solo 1, intento equipar
        {
            int item_id = items[0];
            var possible_body_parts = Actions.ChildrenWhoEquip(w,w.player,w.equipment_type.Get(item_id));
            if (possible_body_parts.Count == 0)
            {
                Console.SetCursorPosition(0, Config.INPUT_Y);
                Console.Write("No available body parts".PadRight(Console.WindowWidth));
                return;
            }
            // sino, puedo equipar
            Actions.Equip(w, w.player, items[0]);
            Console.SetCursorPosition(0, Config.INPUT_Y);
            Console.Write("You equip the item".PadRight(Console.WindowWidth)); //placeholder
            return;
        }
        /*
        // escribir menu
        Console.SetCursorPosition(0, Config.INPUT_Y);
        Console.Write("Drop what?".PadRight(Console.WindowWidth));
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
        */
    }
    static void HandleUnequip(World w)
    {
        ClearInputZone(20); 
        // listo los items equipables
        // pregunto cual queres equipar
        // recien ahi chequeo si es posible (mas adelante se puede resolver antes)
        if(!w.holding.Has(w.player))
        {
            Console.SetCursorPosition(0, Config.INPUT_Y);
            Console.Write("No inventory, can't store equipment!"); //raro, quizas podria equipar cosas del piso
            return;
        }
        // PLACEHOLDER desequipo si tengo 1 item
        List<int> equipped_items = w.equipped_ids.Get(w.player);
        Actions.Unequip(w,w.player,equipped_items[0]);
    }
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