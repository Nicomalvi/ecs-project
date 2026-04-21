public static class SightDetectionSystem
{ // ahora mismo pueden todos ver a traves de las paredes !!!!
    public static void Run(World w)
    {
        for (int i = 0; i < w.sight.dense.Count; i++)
        { // asumo que habra mas enemigos con vision que posicion, necesito ambas para calcular
            int id = w.sight.valid_ids[i];
            if (w.turn_order[w.current_turn]!=id || // si no es mi turno..
            !w.position.Has(id) || w.sight.dense[i].sight == false || // si no tengo pos o vision...
            (w.player != -1 && id == w.player)){continue;} //si soy el player.. (la vision es el render!)

            List<int> seen_enemies =  Pathfinding.BFS_Detect(
                w.game_map, 
                w.aux_map,
                w.position.Get(id),
                // =========================================================================================
                // AHORA MISMO llamo al bfs y me guardo solamente las entidades de otra raza
                entity_id => w.race.Has(entity_id) && w.race.Has(id) && w.race.Get(entity_id) != w.race.Get(id),

                // ahora mismo el sight detection arma lista de enemigos que veo nomas
                // la IA se encarga de trabajar con dicha lista
                // cambiar? trabajar juntos? 
                // componente state o attitude, si no estoy alerta no busco a nadie por ejemplo

                w.sight.dense[i].range);
            // se actualiza cada turno, no hace falta limpiar
            w.detected_enemies.Add(id, seen_enemies);
        }
    }
}