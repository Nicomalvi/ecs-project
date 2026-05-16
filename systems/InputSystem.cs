using Raylib_cs;
public static class InputSystem
{
    public static void Run(World w)
    {
        int playerSpeed = 500;
        var currentMoveComponent = w.Movement2.Get(w.Player);
        if(Raylib.IsKeyDown((KeyboardKey)Config.RIGHT_KEY))
        {
            currentMoveComponent.vx += playerSpeed;
            w.Movement2.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.UP_KEY))
        {
            currentMoveComponent.vy += playerSpeed;
            w.Movement2.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.LEFT_KEY))
        {
            currentMoveComponent.vx -= playerSpeed;
            w.Movement2.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.DOWN_KEY))
        {
            currentMoveComponent.vy -= playerSpeed;
            w.Movement2.Add(w.Player,currentMoveComponent);
        }
    }
}