using Raylib_cs;

public static class GravitySistem
{
    public static void Run(World w)
    {
        foreach (int id in w.Gravity.valid_ids)
        {
            var dt = Raylib.GetFrameTime();
            var Movement = w.Movement.Get(id);
            Movement.velY += -150;
            w.Movement.Set(id,Movement);
        }
    }
}