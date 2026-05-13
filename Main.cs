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
W.Physics.Add(player, new Components.Physics 
{ 
    x = 100, y = 100, 
    width = 32, height = 32 , collisionType = Components.CollisionType.actor
});
W.Movement.Add(player, new Components.Movement { 
    velX = 0, maxVelX = 200,
    velY = 0, maxVelY = 1800,
    currentlyMoving = false});
Components.Animation playerAnimation = new Components.Animation
{
    textureRow = 0,
    textureHeight = 32,
    textureWidth = 32,
    frameTime = 0,
    currentFrame = 0,
    maxFrame = 4
};
W.Animation.Add(player, playerAnimation);
Components.EntityState playerState = new Components.EntityState
{
    state = Components.State.idle,
    lockTimer = 0 
};
W.StateComponent.Add(player, playerState);
Components.Sprite playerSprite = new Components.Sprite
{
    textureID = 0,
    textureHeight = 32,
    textureWidth = 32,
    textureX = 0,
    textureY = 0
};
W.Sprite.Add(player, playerSprite);


W.Gravity.Add(player, true);
MapUtils.AddPhysicalToMap(W, player);

// ============================================================================================
// mapa init
// ============================================================================================
int platform1 = IDManager.get_id();
W.Physics.Add(platform1, new Components.Physics 
{ 
    x = 300, y = 100, 
    width = 32, height = 32, hasMoved = false, collisionType = Components.CollisionType.platform
});
MapUtils.AddPhysicalToMap(W, platform1);

int platform2 = IDManager.get_id();
W.Physics.Add(platform2, new Components.Physics 
{ 
    x = 120, y = 350, 
    width = 64, height = 32, hasMoved = false, collisionType = Components.CollisionType.platform
});
MapUtils.AddPhysicalToMap(W, platform2);

int platform3 = IDManager.get_id();
W.Physics.Add(platform3, new Components.Physics 
{ 
    x = 150, y = 210, 
    width = 64, height = 32, hasMoved = false, collisionType = Components.CollisionType.platform
});
MapUtils.AddPhysicalToMap(W, platform3);


int itemOnPlatform3 = IDManager.get_id();
W.Physics.Add(itemOnPlatform3, new Components.Physics 
{ 
    x = 150, y = 500, 
    width = 16, height = 16, hasMoved = false, collisionType = Components.CollisionType.item
});
W.Gravity.Add(itemOnPlatform3,true);
W.Movement.Add(itemOnPlatform3, new Components.Movement { 
    velX = 0, maxVelX = 200,
    velY = 0, maxVelY = 1800,
    currentlyMoving = false});
MapUtils.AddPhysicalToMap(W, itemOnPlatform3);

int floor = IDManager.get_id();
W.Physics.Add(floor, new Components.Physics
{
    x = 64, y = 64,
    width = Config.WIDTH-Config.CELL_SIZE, height = 1, hasMoved = false, collisionType = Components.CollisionType.platform
});
MapUtils.AddPhysicalToMap(W, floor);

int ceiling = IDManager.get_id();
W.Physics.Add(ceiling, new Components.Physics
{
    x = 64, y = Config.HEIGHT-Config.CELL_SIZE-32,
    width = Config.WIDTH-Config.CELL_SIZE, height = 1, hasMoved = false, collisionType = Components.CollisionType.platform
});
MapUtils.AddPhysicalToMap(W, ceiling);

int wall1 = IDManager.get_id();
W.Physics.Add(wall1, new Components.Physics
{
    x = 64, y = 64,
    width = 1, height = Config.HEIGHT-Config.CELL_SIZE, hasMoved = false, collisionType = Components.CollisionType.platform
});
MapUtils.AddPhysicalToMap(W, wall1);

int wall2 = IDManager.get_id();
W.Physics.Add(wall2, new Components.Physics
{
    x = Config.WIDTH-Config.CELL_SIZE-32, y = 64,
    width = 1, height = Config.HEIGHT-Config.CELL_SIZE, hasMoved = false, collisionType = Components.CollisionType.platform
});
MapUtils.AddPhysicalToMap(W, wall2);

W.StateComponent.Add(platform1, playerState);
W.StateComponent.Add(platform2, playerState);
W.StateComponent.Add(platform3, playerState);
W.StateComponent.Add(wall1, playerState);
W.StateComponent.Add(wall2, playerState);
W.StateComponent.Add(ceiling, playerState);
W.StateComponent.Add(floor, playerState);
W.StateComponent.Add(itemOnPlatform3, playerState);
// ============================================================================================
// LOOP
// ============================================================================================
Random rng = new Random();
int testVel = 1;
while (!Raylib.WindowShouldClose())
{   
    W.Movement.Add(platform1, 
    new Components.Movement { velX = testVel*100, maxVelX = 200, velY = 0, maxVelY = 1800, currentlyMoving = true});
    W.Movement.Add(platform2, 
    new Components.Movement { velX = testVel*50, maxVelX = 200, velY = 0, maxVelY = 1800, currentlyMoving = true});

    AnimationSystem.Run(W); //elijo para los que tienen animacion, cual es el prox. sprite a renderizar
    RenderSystem.Run(W);

    InputSystem.Run(W);
    GravitySistem.Run(W);   // luego de decidir donde se mueve alguien, se le aplica gravedad
    MovementSystem.Run(W);
    StateSystem.Run(W);     // los estados se actualizan con info. del movimiento
    W.Tick ++;

    if (W.Physics.Get(platform1).x <= 66 || W.Physics.Get(platform1).x >= Config.WIDTH - 97) {testVel *= -1;}
}
Raylib.CloseWindow();