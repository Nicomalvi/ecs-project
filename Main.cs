using static ComponentFlags;
Console.WriteLine("Inicia el juego!!");

static void game_loop()
{
    List<int>[,] game_map; // considerar hacerlo global
    game_map = new List<int>[Config.WIDTH,Config.HEIGHT];//[x,y] = lista de id's
    for (int i = 0; i<Config.WIDTH; i++)
    {
        for(int j = 0; j<Config.HEIGHT; j++)
        {
            game_map[i,j] = new List<int>();
        }
    }
    Components c = new Components();
    int player = EntityTemplates.create_human(c,game_map,5,5);
    c.AddPlayerInput(player);
    c.AddHasTurn(player);
    int sword = EntityTemplates.create_item(c,game_map,7,7,5,'/');

    while (true)
    {
        RenderSystem.run(game_map,c);
        InputSystem.run(c);
        //InventorySystem.run(game_map,c);
        MovementSystem.run(game_map,c);
    }
}

game_loop();