using Raylib_cs;
public static class InputSystem
{
    public static void Run(World W)
    {
        int playerSpeed = 5;
        // ver que teclas estan siendo apretadas -> cambiar lo que deba cambiar
        if(Raylib.IsKeyDown((KeyboardKey)Config.RIGHT_KEY))
        {
            var currentMoveComponent = new AuxTypes.MovementComponent{vx = 0,vy = 0};
            if(W.MovementComponent.Has(W.Player)){currentMoveComponent = W.MovementComponent.Get(W.Player);}
            currentMoveComponent.vx += playerSpeed;
            W.MovementComponent.Add(W.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.UP_KEY))
        {
            var currentMoveComponent = new AuxTypes.MovementComponent{vx = 0,vy = 0};
            if(W.MovementComponent.Has(W.Player)){currentMoveComponent = W.MovementComponent.Get(W.Player);}
            currentMoveComponent.vy += playerSpeed;
            W.MovementComponent.Add(W.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.LEFT_KEY))
        {
            var currentMoveComponent = new AuxTypes.MovementComponent{vx = 0,vy = 0};
            if(W.MovementComponent.Has(W.Player)){currentMoveComponent = W.MovementComponent.Get(W.Player);}
            currentMoveComponent.vx += -playerSpeed;
            W.MovementComponent.Add(W.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.DOWN_KEY))
        {
            var currentMoveComponent = new AuxTypes.MovementComponent{vx = 0,vy = 0};
            if(W.MovementComponent.Has(W.Player)){currentMoveComponent = W.MovementComponent.Get(W.Player);}
            currentMoveComponent.vy += -playerSpeed;
            W.MovementComponent.Add(W.Player,currentMoveComponent);
        }
    }
}