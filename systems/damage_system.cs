public static class DamageSystem
{
    public static void Run(World w)
    {
        // idea general
        // una entidad de daño -> lastima a los targets de manera "type" y listo

        // atacar con la espada          -> genero 1 entidad damage fisco 
        // atacar con espada de fuego    -> genero 1 entidad damage fisico, 1 entidad damage fuego
        // lanzar hechizo bola de fuego  -> genero 1 entidad de damage fuego, 1 entidad de damage magico
        //                                  por cada tile que explote la bola de fuego
        int damage_entities = w.damage.dense.Count; 
        List<int> damage_ids = new();                 
        for (int i = 0; i < damage_entities; i++)
        {
            int id = w.damage.valid_ids[i];
            var damage = w.damage.dense[i];
            if (!w.attack_targets.Has(id)) continue; // no tengo a quien atacar
            foreach (int target_id in w.attack_targets.Get(id)){
                if (!w.health.Has(target_id)) continue; // si ataco a alguien sin vida, no pasa nada
                var target_health = w.health.Get(target_id);
                target_health.current -= damage.amount;        // MODIFICACIONES A STRUCT NO PERSISTEN
                w.health.Set(target_id, target_health);        // guardar de vuelta

                if (target_health.current <= 0)
                {   // CHEQUEO TE MATE
                    // ej. kill counter? damage deberia tener source tambien
                }
                

            }
            damage_ids.Add(id);
        }
        foreach (int damage_id in damage_ids)
        {
            w.damage.Remove(damage_id);
            w.attack_targets.Remove(damage_id);
            // si los voy a usar a la vez, sirve que sean separados?
            // quizas se puede reusar attack_targets con habilidades como stun, freeze?
            IDManager.destroy(damage_id);
            // si me aseguro que las entidades damage solo sean daño, puedo liberar la ID
            // quizas un goblin se vuelve daño hacia un mago (seria gracioso), reconsiderar
        }
    }
}