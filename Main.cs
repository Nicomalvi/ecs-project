using Raylib_cs;

Raylib.InitWindow(800, 600, "Hola");
Raylib.SetTargetFPS(60);

World w = new World();
MapUtils.MapInit(w);
// ============================================================================================
// ENTIDADES init
// ============================================================================================
int goblin = Prefabs.Goblin(w);
int goblin2 = Prefabs.Goblin(w);
int goblin3 = Prefabs.Goblin(w);
int human = Prefabs.Human(w);

int sword = IDManager.get_id();
w.ascii.Add(sword, '/');
w.name.Add(sword, "sword");
w.equipment_type.Add(sword, AuxTypes.EquipmentType.melee_weapon);
AuxTypes.Attributes sword_att = new AuxTypes.Attributes
    {
        strength = 5,
        constitution = 0,
        intelligence = 0
    };
w.attributes.Add(sword, sword_att);

int shield = IDManager.get_id();
w.ascii.Add(shield, '0');
w.name.Add(shield, "shield");
w.equipment_type.Add(shield, AuxTypes.EquipmentType.melee_weapon);
AuxTypes.Attributes shield_att = new AuxTypes.Attributes
    {
        strength = 0,
        constitution = 5,
        intelligence = 0
    };
w.attributes.Add(shield, shield_att);

int dagger = IDManager.get_id();
w.ascii.Add(dagger, '+');
w.name.Add(dagger, "dagger");
w.equipment_type.Add(dagger, AuxTypes.EquipmentType.melee_weapon);
AuxTypes.Attributes dagger_att = new AuxTypes.Attributes
    {
        strength = 3,
        constitution = 0,
        intelligence = 0
    };
w.attributes.Add(dagger, dagger_att);

//w.map_blocks.Add(goblin, (false,false)); Con esto puedo agarrar y llevar al goblin !!!!
MapUtils.AddToMap(w,sword,(17,17));
MapUtils.AddToMap(w,shield,(17,17));
MapUtils.AddToMap(w,dagger,(17,17));

MapUtils.AddToMap(w,goblin,(1,1));
MapUtils.AddToMap(w,goblin2,(2,1));
MapUtils.AddToMap(w,goblin3,(1,2));

MapUtils.AddToMap(w,human,(17,15));
// ============================================================================================
// LOOP
// ============================================================================================
Random rng = new Random();
Console.CursorVisible = false;
w.player = human;
w.ai_behaviour.Remove(human);

while (!Raylib.WindowShouldClose())
{   
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
    ActionSystem.Run(w);
    MovementSystem.Run(w);

    // DAMAGE y DEATH pueden ir en un loop ya que ambos pueden generar más daño (ataco a un enemigo con pinches o exploto al morir y causo daño)
    // y normalmente (intentaré) cuando termine de usar una entidad damage o on death, éste valor se debería remover
    DamageSystem.Run(w);
    DeathSystem.Run(w);
    Thread.Sleep(50);
    w.tick ++;
}
Raylib.CloseWindow();