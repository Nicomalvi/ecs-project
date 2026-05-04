using Raylib_cs;

Raylib.InitWindow(Config.WIDTH, Config.HEIGHT, "Hola");
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
W.MovementComponent.Add(player, new AuxTypes.MovementComponent { vx = 0, vy = 0 });
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

int platform = IDManager.get_id();
W.PhysicsComponent.Add(platform, new AuxTypes.PhysicsComponent 
{ 
    x = 0, y = 50, 
    width = 320, height = 32 
});
MapUtils.AddPhysicalToMap(W, platform);

int floor = IDManager.get_id();
W.PhysicsComponent.Add(floor, new AuxTypes.PhysicsComponent
{
    x = 32, y = 32,
    width = Config.WIDTH-Config.CELL_SIZE, height = 1
});
MapUtils.AddPhysicalToMap(W, floor);

int ceiling = IDManager.get_id();
W.PhysicsComponent.Add(ceiling, new AuxTypes.PhysicsComponent
{
    x = 32, y = Config.HEIGHT-Config.CELL_SIZE,
    width = Config.WIDTH-Config.CELL_SIZE, height = 1
});
MapUtils.AddPhysicalToMap(W, ceiling);

int wall1 = IDManager.get_id();
W.PhysicsComponent.Add(wall1, new AuxTypes.PhysicsComponent
{
    x = 32, y = 32,
    width = 1, height = Config.HEIGHT-Config.CELL_SIZE
});
MapUtils.AddPhysicalToMap(W, wall1);

int wall2 = IDManager.get_id();
W.PhysicsComponent.Add(wall2, new AuxTypes.PhysicsComponent
{
    x = Config.WIDTH-32, y = 32,
    width = 1, height = Config.HEIGHT-Config.CELL_SIZE
});
MapUtils.AddPhysicalToMap(W, wall2);

// ============================================================================================
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