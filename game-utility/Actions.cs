public static class Actions
{
    public static void PickUp(World w, int actor_id, int item_id)
    // actor agarra item que esté en su mismo espacio físico
    {   
        string actor = "someone nameless";
        if (w.name.Has(actor_id)) actor = w.name.Get(actor_id);
        string item = "something nameless";
        if (w.name.Has(item_id)) item = w.name.Get(item_id);

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
                w.holding.Get(actor_id).Add(id);
                MapUtils.RemoveFromMap(w,id);
                w.held_by.Add(id, actor_id);
                
                w.announcement_list.Add(actor + " picks up " + item);
                return;
            }
        }
        w.announcement_list.Add(actor + " tried to pick up " + item + " but it's not there!");
    }

    // PickUpFrom: actor agarra item de una celda, sin necesariamente estar parado ahí. (con llamado correcto harian lo mismo)
    public static void Drop(World w, int actor_id, int item_id)
    {   
        string actor = "someone nameless";
        if (w.name.Has(actor_id)) actor = w.name.Get(actor_id);
        string item = "something nameless";
        if (w.name.Has(item_id)) item = w.name.Get(item_id);

        if(!w.holding.Has(actor_id) || !w.position.Has(actor_id)) 
        {
            w.announcement_list.Add(actor + " physically can't drop anything!");
            return; // fisicamente imposible!
        }
        List<int> actor_inventory = w.holding.Get(actor_id);
        if(actor_inventory.Count == 0 || !actor_inventory.Contains(item_id))
        {
            w.announcement_list.Add(actor + " can't drop " + item);
            return; // NAda que dropear! No puedes dropear eso!
        }
        
        var pos = w.position.Get(actor_id);
        MapUtils.AddToMap(w, item_id, pos);
        w.held_by.Remove(item_id);
        actor_inventory.Remove(item_id); // clases se pasan por ref. no hace falta pasar de vuelta el inventario!
        w.announcement_list.Add(actor + " dropped " + item);
    }
}