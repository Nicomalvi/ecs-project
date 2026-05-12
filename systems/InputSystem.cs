using Raylib_cs;
public static class InputSystem
{
    public static void Run(World w)
    {
        int playerSpeed = 175;
        var currentMoveComponent = w.MovementComponent.Get(w.Player);
        if(Raylib.IsKeyDown((KeyboardKey)Config.RIGHT_KEY))
        {
            currentMoveComponent.vx += playerSpeed;
            currentMoveComponent.currentlyMoving = true;
            w.MovementComponent.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.UP_KEY) && MovementSystem.GetGroundId(w,w.PhysicsComponent.Get(w.Player),w.Player) != -1)
        {
            currentMoveComponent.vy += 1800;
            currentMoveComponent.currentlyMoving = true;
            w.MovementComponent.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.LEFT_KEY))
        {
            currentMoveComponent.vx += -playerSpeed;
            currentMoveComponent.currentlyMoving = true;
            w.MovementComponent.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.DOWN_KEY))
        {
            currentMoveComponent.vy += -playerSpeed;
            currentMoveComponent.currentlyMoving = true;
            w.MovementComponent.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown(KeyboardKey.H))
        { // debug
            currentMoveComponent.currentlyMoving = true;
            var phys = w.PhysicsComponent.Get(w.Player);
            MapUtils.RemovePhysicalFromMap(w,w.Player);
            phys.x = 300;
            phys.y = 300;
            w.PhysicsComponent.Set(w.Player,phys);
            MapUtils.AddPhysicalToMap(w,w.Player);
        }
    }
}