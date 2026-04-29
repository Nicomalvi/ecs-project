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
            while(!finished)
            {
                if (w.ai_behaviour.dense[i] == AuxTypes.AiState.chase && finished == false) 
                {
                    if (!w.position.Has(id) || !w.detected_enemies.Has(id) || w.detected_enemies.Get(id).Count == 0) 
                    {
                        w.ai_behaviour.Set(id,AuxTypes.AiState.idle);
                        continue;
                    }
                    int target = w.detected_enemies.Get(id)[0];  // el enemigo a perseguir más cercano
                    if (!w.position.Has(target)) {
                        w.ai_behaviour.Set(id,AuxTypes.AiState.idle);
                        continue;       // perdio pos. desde que lo detecté?
                    }
                    var (father, found) = Pathfinding.BFS(
                        w.game_map, w.aux_map,
                        w.position.Get(id),
                        (pos, map) => Math.Abs(pos.x - w.position.Get(target).x) <= 1    // condicion: llegar a 1 celda del target
                                && Math.Abs(pos.y - w.position.Get(target).y) <= 1,   // condicion: llegar a 1 celda del target 
                        w.sight.Get(id).range
                    );

                    if (found == (-1, -1)) 
                    {
                        finished = true;
                        continue;  // no encontro camino
                    }
                    var path = Pathfinding.GetPath(father, w.position.Get(id), found);
                    if (path.Count == 1)
                    {   //estoy al lado
                        w.ai_behaviour.Set(id, AuxTypes.AiState.melee_attack); // usar dense quizas sea mas directo?
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
                if (w.ai_behaviour.dense[i] == AuxTypes.AiState.melee_attack && finished == false)
                {
                    int target = w.detected_enemies.Get(id)[0];  // el enemigo a perseguir más cercano
                    if (!w.position.Has(target)) {
                        w.ai_behaviour.Set(id, AuxTypes.AiState.idle);
                        continue;
                    }       // perdio pos. desde que lo detecté?
                    w.pending_actions.Add(id, new AuxTypes.PendingAction
                    {
                        type = AuxTypes.ActionType.melee_attack,
                        target_ids = new List<int> { target }
                    });
                    finished = true;
                    w.ai_behaviour.dense[i] = AuxTypes.AiState.idle; // inconsistencia, esto puede ser un Set. pero esto es mas rapido?
                }
                    
                if (w.ai_behaviour.dense[i] == AuxTypes.AiState.idle && finished == false){
                    if(w.detected_enemies.Has(id) && w.detected_enemies.Get(id).Count > 0)
                    {
                        w.ai_behaviour.dense[i] = AuxTypes.AiState.chase;      // inconsistencia, esto puede ser un Set. pero esto es mas rapido?      
                    } else
                    {
                        Random rng = new Random();
                        short dx = (short) rng.Next(-1,2);
                        short dy = (short) rng.Next(-1,2);
                        w.movement.Add(id, (dx, dy));
                        finished = true;          
                    }
                }
            }
        }
    }
}