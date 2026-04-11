/*using static ComponentFlags;

public static class InventorySystem
{   
    public static void run(List<int>[,] game_map, Components c)
    {
        pickups(game_map, c);
        // drops? throws? spawns?
    }
    private static void pickups(List<int>[,] game_map, Components c)
    {
        for (int i = 0; i<Config.MAX_ENTITIES; i++)
        {
            if ((c.component_mask[i] & HAS_PICKUP_INTENT) == 0) continue;
            // continue sale del for y pasa al sig. i
            bool tiene_pos = (c.component_mask[i] & HAS_POSITION) != 0;
            bool tiene_inv = (c.component_mask[i] & HAS_INVENTORY) != 0;

            if (!tiene_pos && !tiene_inv) continue; // no tiene nada, ignorar
            if (!tiene_pos) { continue; }
            if (!tiene_inv) { continue; }

            // caso quiero agarrar, tengo posicion, tengo inventario
            // debo ver si hay items donde estoy parado
            List<int> ids_in_cell;
            (short x, short y) = c.position[i];
            ids_in_cell = game_map[x,y];
            int item_id = -1;
            foreach (int id in ids_in_cell)
            {
                if ((c.component_mask[id] & IS_ITEM) != 0)
                {
                    item_id = id;
                    c.inventory[i].Add(item_id);
                    c.inventory_weight[i]+=c.weight[item_id];
                    // quizas un "quien me lleva?"
                    c.component_mask[item_id] &= ~HAS_POSITION; // SIEMPRE ACOMPAÑADO DE GAME MAP
                    game_map[x,y].Remove(item_id);
                    c.component_mask[item_id] &= ~CAN_MOVE; // si por alguna razon agarre un item que se podia mover...

                    c.component_mask[i] &= ~HAS_PICKUP_INTENT; //limpio ganas de agarrar
                    break;
                }
            }
            if (item_id == -1) {  } // caso nada para agarrar
            c.component_mask[i] &= ~HAS_PICKUP_INTENT; //limpio ganas de agarrar
        }
    }
}
*/