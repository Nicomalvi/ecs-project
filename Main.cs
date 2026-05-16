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
Components.AddBasicPhysics(w,player,50,50,16,16);
w.Gravity.Add(player,true);
int box = IDManager.get_id();
Components.AddBasicPhysics(w,box,300,200,200,32);
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
    Components.Movement2 boxMoving = new Components.Movement2{vx = 20, vy = 0, max = 1000};
    w.Movement2.Set(box,boxMoving);
    MovementSystemV2.Run(w);
    //CollisionSystem.Run(w);
    //StateSystem.Run(w);     // los estados se actualizan con info. del movimiento
    w.Tick ++;
}
Raylib.CloseWindow();