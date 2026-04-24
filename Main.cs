Console.WriteLine("Hello, World!");

// ============================================================================================
// COMPONENTES (SOLO UNO POR TIPO)
// ============================================================================================
World w = new World();
// ============================================================================================
// MAPA
// ============================================================================================

for (int i = 0; i< Config.WIDTH; i++)
{
    int wall = IDManager.get_id();
    w.map_blocks.Add(wall, (true,true));
    w.ascii.Add(wall, '#');
    w.game_map[i, 0].Add(wall);
    w.aux_map[i, 0] = (w.aux_map[i,0].blocks_movement + 1, w.aux_map[i,0].blocks_vision + 1); // se podria hacer mas limpio

    int wall2 = IDManager.get_id();
    w.map_blocks.Add(wall2, (true,true));
    w.ascii.Add(wall2, '#');
    w.game_map[i, Config.HEIGHT - 1].Add(wall2);
    w.aux_map[i, Config.HEIGHT - 1] = (w.aux_map[i,Config.HEIGHT - 1].blocks_movement + 1, w.aux_map[i,Config.HEIGHT - 1].blocks_vision + 1);

    if (i == 18) continue;
    int wall3 = IDManager.get_id();
    w.map_blocks.Add(wall3, (true,true));
    w.ascii.Add(wall3, '#');
    w.game_map[i, 14].Add(wall3);
    w.aux_map[i, 14] = (w.aux_map[i,14].blocks_movement + 1, w.aux_map[i,14].blocks_vision + 1);
}

for (int i = 1; i< 6; i++)
{
    int wall = IDManager.get_id();
    w.map_blocks.Add(wall, (true,true));
    w.ascii.Add(wall, '#');
    w.game_map[i, 4].Add(wall);
    w.aux_map[i, 4] = (w.aux_map[i,4].blocks_movement + 1, w.aux_map[i,4].blocks_vision + 1); // se podria hacer mas limpio
}

for (int j = 0; j< Config.HEIGHT; j++)
{
    int wall = IDManager.get_id();
    w.map_blocks.Add(wall, (true,true));
    w.ascii.Add(wall, '#');
    w.game_map[0, j].Add(wall);
    w.aux_map[0, j] = (w.aux_map[0,j].blocks_movement + 1, w.aux_map[0,j].blocks_vision + 1);

    int wall2 = IDManager.get_id();
    w.map_blocks.Add(wall2, (true,true));
    w.ascii.Add(wall2, '#');
    w.game_map[Config.WIDTH - 1, j].Add(wall2);
    w.aux_map[Config.WIDTH - 1, j] = (w.aux_map[Config.WIDTH - 1,j].blocks_movement + 1, w.aux_map[Config.WIDTH - 1,j].blocks_vision + 1);
}

// ============================================================================================
// ENTIDADES
// ============================================================================================
int goblin = Prefabs.Goblin(w);
int human = Prefabs.Human(w);

int sword = IDManager.get_id();
w.ascii.Add(sword, '/');
w.name.Add(sword, "sword");
w.equipment_type.Add(sword, AuxTypes.EquipmentType.hands);
int shield = IDManager.get_id();
w.ascii.Add(shield, '0');
w.name.Add(shield, "shield");
w.equipment_type.Add(shield, AuxTypes.EquipmentType.hands);
int dagger = IDManager.get_id();
w.ascii.Add(dagger, '+');
w.name.Add(dagger, "dagger");
w.equipment_type.Add(dagger, AuxTypes.EquipmentType.hands);
//w.map_blocks.Add(goblin, (false,false)); Con esto puedo agarrar y llevar al goblin !!!!
MapUtils.AddToMap(w,sword,(17,17));
MapUtils.AddToMap(w,shield,(17,17));
MapUtils.AddToMap(w,dagger,(17,17));
MapUtils.AddToMap(w,goblin,(1,1));
MapUtils.AddToMap(w,human,(17,15));
// ============================================================================================
// LOOP
// ============================================================================================
Random rng = new Random();
Console.CursorVisible = false;
w.player = human;
w.ai_behaviour.Remove(human);

while (true)
{   
    if (w.current_turn == 0)
    {
        // 1. shuffle (Fisher-Yates)
        for (int i = w.turn_order.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (w.turn_order[i], w.turn_order[j]) = (w.turn_order[j], w.turn_order[i]);
        }
        // 2. sort por speed (desc)
        w.turn_order.Sort((a, b) =>
            w.speed.Get(b).CompareTo(w.speed.Get(a)));
    }
    
    RenderSystem.Run(w);
    if (w.tick % 10 == 0)
        HealthSystem.Run(w); // correcto? toda unidad se cura luego de 10 ticks, no turnos...
                             // deberia ser en base a turnos, regen, energy...
    SightDetectionSystem.Run(w);
    // EnergySystem.Run(w);
    // se deberian saltear si no tengo energia
    // entonces los ticks siguen pasando pero yo no tuve turno, lo mismo para la IA

    // input y AI pueden ir dentro de if existen turnos
    InputSystem.Run(w);
    AIBehaviourSystem.Run(w);
    MovementSystem.Run(w);

    // DAMAGE y DEATH pueden ir en un loop ya que ambos pueden generar más daño (ataco a un enemigo con pinches o exploto al morir y causo daño)
    // y normalmente (intentaré) cuando termine de usar una entidad damage o on death, éste valor se debería remover
    DamageSystem.Run(w);
    DeathSystem.Run(w);
    Thread.Sleep(50);
    w.tick ++;

    w.current_turn ++;
    if(w.current_turn == w.turn_order.Count) w.current_turn = 0;
}
