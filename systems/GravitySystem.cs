using Raylib_cs;

public static class GravitySistem
{
    public static void Run(World w)
    {
        foreach (int id in w.Gravity.valid_ids)
        {
            var dt = Raylib.GetFrameTime();
            var MovementComponent = w.MovementComponent.Get(id);
            MovementComponent.vy += -1000 * dt;
            w.MovementComponent.Set(id,MovementComponent);
        }
    }
}