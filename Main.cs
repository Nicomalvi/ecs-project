using System.Numerics;
using Raylib_cs;

Raylib.InitWindow(Config.WIDTH, Config.HEIGHT, "DEVMODE");
Raylib.SetTargetFPS(60);

World w = new World();
// ============================================================================================
// ENTIDADES init
// ============================================================================================
// entidad jugador - cuadrado que se mueve
int player = IDManager.get_id();
w.Player = player;
w.Movement2.Add(player, new Components.Movement2{vx = 0, vy = 0, max = 1000});
w.Position.Add(player,new Components.Position{x = 50, y = 50});
w.Hitbox.Add(player, new Components.Hitbox{x = 50, y = 50, height = 16, width = 16});
w.Gravity.Add(player, true);
MapUtils.AddToHitboxMap(w,player);

int box = IDManager.get_id();
w.Position.Add(box,new Components.Position{x = 300, y = 200});
w.Hitbox.Add(box, new Components.Hitbox{x = 300, y = 200, height = 32, width = 32});
MapUtils.AddToHitboxMap(w,box);
// ============================================================================================
// LOOP
// ============================================================================================
Random rng = new Random();
while (!Raylib.WindowShouldClose())
{   
    //AnimationSystem.Run(w); //elijo para los que tienen animacion, cual es el prox. sprite a renderizar
    RenderSystem.Run(w);
    InputSystem.Run(w);

    GravitySistem.Run(w);   // luego de decidir donde se mueve alguien, se le aplica gravedad
    MovementSystemV2.Run(w);
    CollisionSystem.Run(w);
    //StateSystem.Run(w);     // los estados se actualizan con info. del movimiento
    w.Tick ++;
    Console.WriteLine(w.Position.Get(w.Player).x);
}
Raylib.CloseWindow();