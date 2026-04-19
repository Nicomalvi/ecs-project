public static class DeathSystem {
    public static void Run(World w)
    {
        // una entidad muere si su health baja a 0
        // (casos adicionales? wand of death? desintegrar? veremos)
        for (int i = 0; i < w.health.dense.Count; i++)
        {
            int id = w.health.valid_ids[i];
            var health = w.health.dense[i]; // = a get(id)
            if (health.current <= 0)
            {
                if (!w.death_effect.Has(id))
                {
                    // si no tengo ningun death effect, lo normal es borrar todo al carajo
                    w.destroy_entity(id);
                }
            }
        }
    }
}