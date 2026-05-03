using Raylib_cs;
public static class InputSystem
{
    public static void Run(World w)
    {
        int playerSpeed = 200;
        // ver que teclas estan siendo apretadas -> cambiar lo que deba cambiar
        if(Raylib.IsKeyDown((KeyboardKey)Config.RIGHT_KEY))
        {
            var currentMoveComponent = new AuxTypes.MovementComponent{vx = 0,vy = 0};
            if(w.MovementComponent.Has(w.Player)){currentMoveComponent = w.MovementComponent.Get(w.Player);}
            currentMoveComponent.vx += playerSpeed;
            w.MovementComponent.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.UP_KEY))
        {
            var currentMoveComponent = new AuxTypes.MovementComponent{vx = 0,vy = 0};
            if(w.MovementComponent.Has(w.Player)){currentMoveComponent = w.MovementComponent.Get(w.Player);}
            currentMoveComponent.vy += playerSpeed;
            w.MovementComponent.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.LEFT_KEY))
        {
            var currentMoveComponent = new AuxTypes.MovementComponent{vx = 0,vy = 0};
            if(w.MovementComponent.Has(w.Player)){currentMoveComponent = w.MovementComponent.Get(w.Player);}
            currentMoveComponent.vx += -playerSpeed;
            w.MovementComponent.Add(w.Player,currentMoveComponent);
        }
        if(Raylib.IsKeyDown((KeyboardKey)Config.DOWN_KEY))
        {
            var currentMoveComponent = new AuxTypes.MovementComponent{vx = 0,vy = 0};
            if(w.MovementComponent.Has(w.Player)){currentMoveComponent = w.MovementComponent.Get(w.Player);}
            currentMoveComponent.vy += -playerSpeed;
            w.MovementComponent.Add(w.Player,currentMoveComponent);
        }
    }
}