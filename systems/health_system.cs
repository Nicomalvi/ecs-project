public static class HealthSystem
{
    public static void Run(World w)
    {
        for (int i = 0; i < w.health.dense.Count; i++)
        {
            var h = w.health.dense[i];
            if (h.current < h.max)
                h.current += h.regeneration;
            if (h.current > h.max)
                h.current = h.max;
            w.health.dense[i] = h; // los struct no se actualizan solos
        }
    }
}