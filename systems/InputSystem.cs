using Raylib_cs;
public static class InputSystem
{
    public static void Run(World w)
    {
        int playerSpeed = 175;
        var currentMoveComponent = w.Movement.Get(w.Player);
        if(Raylib.IsKeyDown((KeyboardKey)Config.RIGHT_KEY))
        {
            currentMoveComponent.velX += playerSpeed;
            currentMoveComponent.currentlyMoving = true;
            w.Movement.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.UP_KEY) && MovementSystem.GetGroundId(w,w.Physics.Get(w.Player),w.Player) != -1)
        {
            currentMoveComponent.velY += 1800;
            currentMoveComponent.currentlyMoving = true;
            w.Movement.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.LEFT_KEY))
        {
            currentMoveComponent.velX += -playerSpeed;
            currentMoveComponent.currentlyMoving = true;
            w.Movement.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.DOWN_KEY))
        {
            currentMoveComponent.velY += -playerSpeed;
            currentMoveComponent.currentlyMoving = true;
            w.Movement.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown(KeyboardKey.H))
        { // debug
            currentMoveComponent.currentlyMoving = true;
            var phys = w.Physics.Get(w.Player);
            MapUtils.RemovePhysicalFromMap(w,w.Player);
            phys.x = 300;
            phys.y = 300;
            w.Physics.Set(w.Player,phys);
            MapUtils.AddPhysicalToMap(w,w.Player);
        }
    }
}