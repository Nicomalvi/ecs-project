using static ComponentFlags;

public static class MovementSystem
{
    public static void run(List<int>[,] game_map, Components c)
    {
        // copia la lista para poder modificarla durante la iteración
        List<int> entities_to_move = new List<int>(c.will_move.entities);

        foreach (int id in entities_to_move)
        {
            (short dx, short dy)   = c.movement.Get(id);
            (short x, short y)  = c.position.Get(id);

            (short new_x, short new_y) = ((short)(x + dx), (short)(y + dy));
            if (new_x >= 0 && new_x < Config.WIDTH && new_y >= 0 && new_y < Config.HEIGHT)
            {
                // si no me fui del mapa, muevo la entidad
                c.position.Add(id, ((short,short))(dx + x, dy + y));
                game_map[x,y].Remove(id);
                game_map[x + dx, y + dy].Add(id);
            }
            //independientemente de si me movi, reseteo intents
            c.RemoveMoveIntent(id);
            c.RemoveMovement(id);
        }
    }
}