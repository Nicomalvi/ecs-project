using Raylib_cs;

public static class GravitySistem
{
    public static void Run(World w)
    {
        foreach (int id in w.Gravity.valid_ids)
        {
            float dt = Raylib.GetFrameTime();
            var movement = w.Movement2.Get(id);
            movement.vy -= 100;
            w.Movement2.Set(id,movement); 
        }
    }
}