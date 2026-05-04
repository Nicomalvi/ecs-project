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
            w.MovementComponent.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.UP_KEY))
        {
            currentMoveComponent.vy += playerSpeed;
            w.MovementComponent.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.LEFT_KEY))
        {
            currentMoveComponent.vx += -playerSpeed;
            w.MovementComponent.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.DOWN_KEY))
        {
            currentMoveComponent.vy += -playerSpeed;
            w.MovementComponent.Add(w.Player,currentMoveComponent);
        }
    }
}