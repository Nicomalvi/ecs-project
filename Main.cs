using Raylib_cs;

Raylib.InitWindow(800, 600, "Hola");
Raylib.SetTargetFPS(60);

World W = new World();
//MapUtils.MapInit(w);
// ============================================================================================
// ENTIDADES init
// ============================================================================================
// entidad jugador - cuadrado que se mueve
int player = IDManager.get_id();
W.Player = player;
W.PhysicsComponent.Add(player, new AuxTypes.PhysicsComponent 
{ 
    x = 100, y = 100, 
    width = 32, height = 32 
});
W.MovementComponent.Add(player, new AuxTypes.MovementComponent { vx = 2, vy = 0 });
MapUtils.AddPhysicalToMap(W, player);

// entidad pared - cuadrado estático a la derecha
int wall = IDManager.get_id();
W.PhysicsComponent.Add(wall, new AuxTypes.PhysicsComponent 
{ 
    x = 300, y = 100, 
    width = 32, height = 32 
});
MapUtils.AddPhysicalToMap(W, wall);// ============================================================================================
// LOOP
// ============================================================================================
Random rng = new Random();
while (!Raylib.WindowShouldClose())
{   
    RenderSystem.Run(W);
    InputSystem.Run(W);
    MovementSystem.Run(W);
    W.Tick ++;
}
Raylib.CloseWindow();