public static class AIBehaviourSystem
{   
    public static void Run(World w)

    {
        for (int i = 0; i<w.ai_behaviour.dense.Count; i++)
        {
            int id = w.ai_behaviour.valid_ids[i];
            bool finished = false;      // cuando una IA logre el paso que queria, finished sera true
                                        // util por si estoy en chase -> me doy cuenta que estoy al lado -> paso a attack -> use 2 estados en 1 turno
            // AI BEHAVIOUR CHASE =====================================================================================================================
            // elijo a la entidad que puedo ver mas cerca
            // si estoy al lado          -> genero damage
            // si estoy a más de un paso -> genero 1 paso del camino mínimo
            if (w.ai_behaviour.dense[i] == "chase" && finished == false) 
            {
                if (!w.position.Has(id) || !w.movement.Has(id) || !w.detected_enemies.Has(id) || w.detected_enemies.Get(id).Count == 0) continue;

                int target = w.detected_enemies.Get(id)[0];  // el enemigo a perseguir más cercano
                if (!w.position.Has(target)) continue;       // perdio pos. desde que lo detecté?

                var (father, found) = Pathfinding.BFS(
                    w.game_map, w.aux_map,
                    w.position.Get(id),
                    (pos, map) => Math.Abs(pos.x - w.position.Get(target).x) <= 1    // condicion: llegar a 1 celda del target
                               && Math.Abs(pos.y - w.position.Get(target).y) <= 1,   // condicion: llegar a 1 celda del target 
                    w.sight.Get(id).range
                );

                if (found == (-1, -1)) continue;  // no encontro camino

                var path = Pathfinding.GetPath(father, w.position.Get(id), found);
                if (path.Count == 1)
                {   
                    int damage = IDManager.get_id();
                    AuxTypes.Damage damage_done = new AuxTypes.Damage // PLACEHOLDER
                    {
                        amount = 1,
                        type = "phys"
                    };
                    w.damage.Add(damage, damage_done);
                    w.attack_targets.Add(damage, new List<int> { target });

                    string actor = "someone nameless";
                    if (w.name.Has(id)) actor = w.name.Get(id);
                    string victim = "someone nameless";
                    if (w.name.Has(target)) victim = w.name.Get(target);
                    w.announcement_list.Add(actor + " attacks " + victim + "!");
                    // CASO ESTOY AL LADO ! ! ! ! ! */
                } else
                {
                    // CASO ESTOY A MAS DE 1 CELDA ! ! ! !
                    var (newX, newY)= path[1];  // path[0] es donde ya estoy
                    w.movement.Set(id, ((short)(newX - w.position.Get(id).x),
                                    (short)(newY - w.position.Get(id).y)));
                    finished = true;
                }
                
            }
            // AI BEHAVIOUR ATTACK =====================================================================================================================
            if (w.ai_behaviour.dense[i] == "melee_attack" && finished == false)
            {
                //
            }
        }
    }
}