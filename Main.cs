using Raylib_cs;

Raylib.InitWindow(800, 600, "Hola");
Raylib.SetTargetFPS(60); // util para que si varian los frames los juegos no varien (ej velocidades)

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
W.Gravity.Add(player, true);
MapUtils.AddPhysicalToMap(W, player);

// entidad pared - cuadrado estático a la derecha
int wall = IDManager.get_id();
W.PhysicsComponent.Add(wall, new AuxTypes.PhysicsComponent 
{ 
    x = 300, y = 100, 
    width = 32, height = 32 
});
MapUtils.AddPhysicalToMap(W, wall);

int floor = IDManager.get_id();
W.PhysicsComponent.Add(floor, new AuxTypes.PhysicsComponent 
{ 
    x = 0, y = 50, 
    width = 320, height = 32 
});
MapUtils.AddPhysicalToMap(W, floor);// ============================================================================================
// LOOP
// ============================================================================================
Random rng = new Random();
while (!Raylib.WindowShouldClose())
{   
    RenderSystem.Run(W);
    InputSystem.Run(W);

    GravitySistem.Run(W); // luego de decidir donde se mueve alguien, se le aplica gravedad
    MovementSystem.Run(W);
    W.Tick ++;
}
Raylib.CloseWindow();