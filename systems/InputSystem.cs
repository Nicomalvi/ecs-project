using static ComponentFlags;

public static class InputSystem
{
    public static void run(Components c)
{
    Dictionary<char, (short,short)> key_to_movement = new() {
        {'w', (0,-1)}, {'W', (0,-1)}, {'8', (0,-1)},
        {'q', (-1,-1)}, {'Q', (-1,-1)}, {'7', (-1,-1)},
        {'e', (1,-1)}, {'E', (1,-1)}, {'9', (1,-1)},
        {'s', (0,1)}, {'S', (0,1)}, {'2', (0,1)},
        {'z', (-1,1)}, {'Z', (-1,1)}, {'1', (-1,1)},
        {'c', (1,1)}, {'C', (1,1)}, {'3', (1,1)},
        {'d', (1,0)}, {'D', (1,0)}, {'6', (1,0)},
        {'a', (-1,0)}, {'A', (-1,0)}, {'4', (-1,0)},
        {'.', (0,0)}, {'5', (0,0)},
    };

    Dictionary<ConsoleKey, (short,short)> arrow_to_movement = new() {
        {ConsoleKey.UpArrow,    (0,-1)},
        {ConsoleKey.DownArrow,  (0,1)},
        {ConsoleKey.LeftArrow,  (-1,0)},
        {ConsoleKey.RightArrow, (1,0)},
    };

    foreach (int id in c.player_input_group.entities)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            (short,short) delta;
            bool valid_key_movement = 
            arrow_to_movement.TryGetValue(keyInfo.Key, out delta) || key_to_movement.TryGetValue(keyInfo.KeyChar, out delta);

            if (valid_key_movement)
            {
                c.AddMovement(id,delta);
                c.AddMoveIntent(id);
            }
        }
}
}