using System.Numerics;
using Raylib_cs;

Raylib.InitWindow(Config.WIDTH, Config.HEIGHT, "DEVMODE");
Raylib.SetTargetFPS(60);

World W = new World();
// ============================================================================================
// ENTIDADES init
// ============================================================================================
// entidad jugador - cuadrado que se mueve
int player = IDManager.get_id();
W.Player = player;
W.PhysicsComponent.Add(player, new AuxTypes.PhysicsComponent 
{ 
    x = 100, y = 100, 
    width = 32, height = 50 
});
W.MovementComponent.Add(player, new AuxTypes.MovementComponent { vx = 0, vy = 0 });
AuxTypes.AnimationComponent playerAnimation = new AuxTypes.AnimationComponent
{
    textureRow = 0,
    textureHeight = 32,
    textureWidth = 32,
    frameTime = 0,
    currentFrame = 0,
    maxFrame = 4
};
W.AnimationComponent.Add(player, playerAnimation);
AuxTypes.EntityStateComponent playerState = new AuxTypes.EntityStateComponent
{
    state = AuxTypes.EntityStates.idle,
    lockTimer = 0 
};
W.StateComponent.Add(player, playerState);
AuxTypes.SpriteComponent playerSprite = new AuxTypes.SpriteComponent
{
    textureID = 0,
    textureHeight = 32,
    textureWidth = 32,
    textureX = 0,
    textureY = 0
};
W.sprite.Add(player, playerSprite);


W.Gravity.Add(player, true);
MapUtils.AddPhysicalToMap(W, player);

// ============================================================================================
// mapa init
// ============================================================================================
int platform1 = IDManager.get_id();
W.PhysicsComponent.Add(platform1, new AuxTypes.PhysicsComponent 
{ 
    x = 300, y = 100, 
    width = 32, height = 32 
});
MapUtils.AddPhysicalToMap(W, platform1);

int platform2 = IDManager.get_id();
W.PhysicsComponent.Add(platform2, new AuxTypes.PhysicsComponent 
{ 
    x = 120, y = 350, 
    width = 64, height = 32 
});
MapUtils.AddPhysicalToMap(W, platform2);

int platform3 = IDManager.get_id();
W.PhysicsComponent.Add(platform3, new AuxTypes.PhysicsComponent 
{ 
    x = 150, y = 210, 
    width = 64, height = 32 
});
MapUtils.AddPhysicalToMap(W, platform3);

int floor = IDManager.get_id();
W.PhysicsComponent.Add(floor, new AuxTypes.PhysicsComponent
{
    x = 64, y = 64,
    width = Config.WIDTH-Config.CELL_SIZE, height = 1
});
MapUtils.AddPhysicalToMap(W, floor);

int ceiling = IDManager.get_id();
W.PhysicsComponent.Add(ceiling, new AuxTypes.PhysicsComponent
{
    x = 64, y = Config.HEIGHT-Config.CELL_SIZE-32,
    width = Config.WIDTH-Config.CELL_SIZE, height = 1
});
MapUtils.AddPhysicalToMap(W, ceiling);

int wall1 = IDManager.get_id();
W.PhysicsComponent.Add(wall1, new AuxTypes.PhysicsComponent
{
    x = 64, y = 64,
    width = 1, height = Config.HEIGHT-Config.CELL_SIZE
});
MapUtils.AddPhysicalToMap(W, wall1);

int wall2 = IDManager.get_id();
W.PhysicsComponent.Add(wall2, new AuxTypes.PhysicsComponent
{
    x = Config.WIDTH-Config.CELL_SIZE-32, y = 64,
    width = 1, height = Config.HEIGHT-Config.CELL_SIZE
});
MapUtils.AddPhysicalToMap(W, wall2);

W.StateComponent.Add(platform1, playerState);
W.StateComponent.Add(platform2, playerState);
W.StateComponent.Add(platform3, playerState);
W.StateComponent.Add(wall1, playerState);
W.StateComponent.Add(wall2, playerState);
W.StateComponent.Add(ceiling, playerState);
W.StateComponent.Add(floor, playerState);
// ============================================================================================
// LOOP
// ============================================================================================
Random rng = new Random();
int testVel = 1;
while (!Raylib.WindowShouldClose())
{   
    W.MovementComponent.Add(platform1, new AuxTypes.MovementComponent { vx = testVel*100, vy = 0 });

    //
    AnimationSystem.Run(W); //elijo para los que tienen animacion, cual es el prox. sprite a renderizar

    RenderSystem.Run(W);

    InputSystem.Run(W);
    GravitySistem.Run(W); // luego de decidir donde se mueve alguien, se le aplica gravedad
    MovementSystem.Run(W);
    W.Tick ++;

    if (W.PhysicsComponent.Get(platform1).x <= 66 || W.PhysicsComponent.Get(platform1).x >= Config.WIDTH - 97) {testVel *= -1;}

    Console.WriteLine(W.StateComponent.Get(W.Player).state);
}
Raylib.CloseWindow();