public static class Actions
{
    //=============================================================================================
    // Funciones utiles
    //=============================================================================================
    static string Name(World w, int id)
    {
        return w.name.Has(id) ? w.name.Get(id) : "something nameless";
    }
    static void MoveToInventory(World w, int actor, int item)
    {
        var inv = w.holding.Get(actor);
        if (!inv.Contains(item)) inv.Add(item);
        w.held_by.Add(item, actor);
    }
    static void RemoveFromInventory(World w, int actor, int item)
    {
        w.holding.Get(actor).Remove(item);
        w.held_by.Remove(item);
    }
    //=============================================================================================
    // Interacciones con items
    //=============================================================================================
     public static void PickUp(World w, int actor_id, int item_id)
    // actor agarra item que esté en su mismo espacio físico
    {   
        actor_id = GetRoot(w, actor_id); // si parte del cuerpo llama a acción que necesita data del cerebro...
        string actor = Name(w, actor_id);
        string item  = Name(w, item_id);
        if(!w.holding.Has(actor_id) || !w.position.Has(actor_id)) 
        {
            w.announcement_list.Add(actor + " tried, but he can't grab anything!");
            return; // si no tengo forma de agarrar ni posicion...
        }
        var pos = w.position.Get(actor_id);
        List<int> items_in_cell = new List<int>(w.game_map[pos.x, pos.y]); // debo copiar la lista sino la edito mientras itero sobre ella
        foreach (int id in items_in_cell)
        {
            if (id == actor_id) continue;
            if (id == item_id)
            {
                var inventory = w.holding.Get(actor_id);
                if (inventory.Contains(id)) return;
                MoveToInventory(w, actor_id, id);
                MapUtils.RemoveFromMap(w,id);
                w.announcement_list.Add(actor + " picks up " + item);
                return;
            }
        }
        w.announcement_list.Add(actor + " tried to pick up " + item + " but it's not there!");
    }
    // PickUpFrom: actor agarra item de una celda, sin necesariamente estar parado ahí. (con llamado correcto harian lo mismo)
    public static void Drop(World w, int actor_id, int item_id)
    {   
        actor_id = GetRoot(w, actor_id); // si parte del cuerpo llama a acción que necesita data del cerebro...
        string actor = Name(w, actor_id);
        string item  = Name(w, item_id);
        if(!w.holding.Has(actor_id) || !w.position.Has(actor_id)) 
        {
            w.announcement_list.Add(actor + " physically can't drop anything!");
            return; // fisicamente imposible!
        }
        var inventory = w.holding.Get(actor_id);
        if(inventory.Count == 0 || !inventory.Contains(item_id))
        {
            w.announcement_list.Add(actor + " can't drop " + item);
            return; // NAda que dropear! No puedes dropear eso!
        }
        var pos = w.position.Get(actor_id);
        MapUtils.AddToMap(w, item_id, pos);
        RemoveFromInventory(w, actor_id, item_id);
        w.announcement_list.Add(actor + " dropped " + item);
    }
   public static void Equip(World w, int actor_id, int item_id) 
        // agrego a ids equipadas del cerebro, quito del inventario, agrego a slot de parte del cuerpo, agrego ref. item a parte del cuerpo,
        // agrego de vuelta al inventario.
    {
        actor_id = GetRoot(w, actor_id); // si parte del cuerpo llama a acción que necesita data del cerebro...
        string actor = Name(w, actor_id);
        string item  = Name(w, item_id);
        // CHEQUEOS ============================================
        if (!w.equipment_type.Has(item_id)) return; // NO ES UN ITEM EQUIPABLE
        if (!w.holding.Has(actor_id)) return; // no puedo equipar nada si no tengo inventario // ============ POSIBLE CAMBIO ============
        if (!w.equipped_ids.Has(actor_id)) return; // si no tengo lista de equipamiento, no puedo equipar
        if (w.equipped_in.Has(item_id)) return; // ysi el item tiene dueño...
        // =====================================================
        var inventory = w.holding.Get(actor_id);
        if (!inventory.Contains(item_id)) return; // no puedo equipar algo que no tengo en el inventario
        var type = w.equipment_type.Get(item_id);
        List<int> equippable = ChildrenWhoEquip(w, actor_id, type);
        if (equippable.Count == 0) return; // NO PUEDES EQUIPAR ESTO (ya sea espacio ocupado o no hay espacio)
        int body_part = equippable[0]; // elijo la primera, en el futuro puedo permitir seleccion (brazo izq o tentaculo derecho ??)
        if (!w.equipment.Has(body_part)) return; // safety barato
        var slots = w.equipment.Get(body_part);
        // chequear en cual slot meter
        for (int i = 0; i<slots.Count; i++)
        {
            if (slots[i].type == type && slots[i].item == -1)
            {
                RemoveFromInventory(w, actor_id, item_id);
                var equipped_list = w.equipped_ids.Get(actor_id);
                if (!equipped_list.Contains(item_id)) equipped_list.Add(item_id);
                w.equipped_in.Add(item_id, body_part);
                var new_slot = slots[i];
                new_slot.item = item_id;
                slots[i] = new_slot;
                w.announcement_list.Add(actor + " equips " + item);
                return;                
            }
        }
    }
    public static void Unequip(World w, int actor_id, int item_id)
    {   // quito de lista de equipados del cerebro, quito del slot de la parte del cuerpo, quito referencia del item a la parte del cuerpo
        actor_id = GetRoot(w, actor_id); // si parte del cuerpo llama a acción que necesita data del cerebro...
        string actor = Name(w, actor_id);
        string item  = Name(w, item_id);
        if (!w.equipped_in.Has(item_id)) return; // si item ya tiene dueño..
        if (!w.equipped_ids.Has(actor_id)) return; // si no tengo lista de equipamiento...
        if (!w.holding.Has(actor_id)) return; // si no tengo inventario...
        int owning_body_part = w.equipped_in.Get(item_id);
        if (!w.equipment.Has(owning_body_part)) return; // si el dueño del item es otra parte del cuerpo...
        var equipment = w.equipment.Get(owning_body_part);
        for (int i = 0; i<equipment.Count; i++)
        {
            if (equipment[i].item == item_id)
            {
                var used_slot = equipment[i];
                used_slot.item = -1;
                equipment[i] = used_slot;
                break;
            }
        }
        var equipped_list = w.equipped_ids.Get(actor_id);
        equipped_list.Remove(item_id);
        w.equipped_in.Remove(item_id);
        MoveToInventory(w, actor_id, item_id);
        w.announcement_list.Add(actor + " unequips " + item);
    }
    //============================================================================================
    // Funciones que trabajan con el árbol de un cuerpo 
    //============================================================================================
    public static List<int> ChildrenWhoEquip(World w, int root, AuxTypes.EquipmentType type)
    {//devuelve lista de ids hijas con equipamiento disponible
        List<int> res = new List<int>();
        void DFS(int id)
        {
            if (w.equipment.Has(id)) // parte del cuerpo actual puede equipar??
            {
                var equipment = w.equipment.Get(id);
                foreach (var slot in equipment)
                {
                    if (slot.type == type && slot.item == -1)
                    {
                        res.Add(id);
                        break; // esta parte ya sirve, no hace falta seguir
                    }
                }
            }
            // alguna parte del cuerpo conectada a mi puede equipar?
            if (w.children.Has(id))
            {
                foreach (int child in w.children.Get(id))
                {
                    DFS(child);
                }
            }
        }
        DFS(root);
        return res;
    }
    public static List<int> GetAllEquippedItems(World w, int root)
    {
        // devuelve TODOS los items equipados en cualquier parte del cuerpo
        List<int> res = new List<int>();
        if (!w.children.Has(root) && !w.equipment.Has(root))
            return res; // no tiene cuerpo ni slots
        void DFS(int id)
        {
            // si esta parte del cuerpo tiene slots
            if (w.equipment.Has(id))
            {
                var slots = w.equipment.Get(id);
                foreach (var slot in slots)
                {
                    if (slot.item != -1)
                        res.Add(slot.item);
                }
            }
            // recorrer hijos
            if (w.children.Has(id))
            {
                foreach (int child in w.children.Get(id))
                {
                    DFS(child);
                }
            }
        }
        DFS(root);
        return res;
    }
    public static List<int> GetAllBodyParts(World w, int root)
    {
        // devuelve todas las partes del cuerpo (incluyendo la raiz)
        List<int> res = new List<int>();
        // chequeo barato
        if (!w.children.Has(root))
        {
            res.Add(root);
            return res;
        }
        void DFS(int id)
        {
            res.Add(id);
            if (w.children.Has(id))
            {
                foreach (int child in w.children.Get(id))
                {
                    DFS(child);
                }
            }
        }
        DFS(root);
        return res;
    }
    public static int GetRoot(World w, int id)
    {
        // asumo que la raíz es el nodo que NO tiene parent
        // normalmente la raiz sera el "mainframe" de las entidades (la parte con ia, inventario, stats...)
        // y todos los nodos que no sean la raiz seran partes del cuerpo
        while (w.parent.Has(id))
        {
            id = w.parent.Get(id);
        }
        return id;
    }
    //=============================================================================================
    // Ataques
    //=============================================================================================
    // solo hay que elegir cantidad de daño
    // Empecemos por Strength + Daño Arma (mas daelante item_effects, ej. guantes de poder, o eso va en on_equip?)
    public static void MeleeAttack(World w, int actor_id, int target_id)
    {
        short actor_strength = w.attributes.Get(actor_id).strength;
        List<int> weapon_list = GetAllEquippedItems(w,actor_id)
            .Where(id => w.equipment_type.Get(id)==AuxTypes.EquipmentType.melee_weapon)
            .ToList();
        short weapon_strength = 0;
        for (int i = 0; i<weapon_list.Count; i++)
        {
            if(w.attributes.Has(weapon_list[i])){weapon_strength+=w.attributes.Get(weapon_list[i]).strength;}
            // sumo a la fuerza del actor la fuerza de todas sus armas melee
        }
        int damage = IDManager.get_id();
        AuxTypes.Damage damage_done = new AuxTypes.Damage
        {
            amount = (short)(actor_strength+weapon_strength),
            type = "phys"
        };
        w.damage.Add(damage, damage_done);
        w.attack_targets.Add(damage, new List<int> { target_id });
        w.announcement_list.Add(Name(w,actor_id) + " attacks " + Name(w,target_id) + " for " + damage_done.amount);
    }
}