public static class Prefabs
{
    public static void Humanoid(World w, int id)
    {
        w.map_blocks.Add(id, (true, false));
        w.speed.Add(id, 5);
        w.ai_behaviour.Add(id, AuxTypes.AiState.idle);
        w.turn_order.Add(id);

        w.holding.Add(id, new List<int>());
        w.size.Add(id, AuxTypes.Size.average);        

        int torso = IDManager.get_id();
        int head  = IDManager.get_id();
        int arm1  = IDManager.get_id();
        int arm2  = IDManager.get_id();
        int legs  = IDManager.get_id();
        int feet  = IDManager.get_id();

        w.equipment.Add(torso, Slots(AuxTypes.EquipmentType.torso));
        w.equipment.Add(head,  Slots(AuxTypes.EquipmentType.head));
        w.equipment.Add(arm1,  Slots(AuxTypes.EquipmentType.arms, AuxTypes.EquipmentType.hands));
        w.equipment.Add(arm2,  Slots(AuxTypes.EquipmentType.arms, AuxTypes.EquipmentType.hands));
        w.equipment.Add(legs,  Slots(AuxTypes.EquipmentType.legs));
        w.equipment.Add(feet,  Slots(AuxTypes.EquipmentType.feet));

        w.equipped_ids.Add(id, new List<int>());

        // jerarquía
        w.children.Add(id, new List<int> { torso });
        w.parent.Add(torso, id);

        w.children.Add(torso, new List<int> { head, arm1, arm2, legs, feet });

        w.parent.Add(head, torso);
        w.parent.Add(arm1, torso);
        w.parent.Add(arm2, torso);
        w.parent.Add(legs, torso);
        w.parent.Add(feet, torso);

        // inicializar children vacíos en hojas
        w.children.Add(head, new List<int>());
        w.children.Add(arm1, new List<int>());
        w.children.Add(arm2, new List<int>());
        w.children.Add(legs, new List<int>());
        w.children.Add(feet, new List<int>());
    }
    public static int Goblin (World w)
    {
        int goblin = IDManager.get_id();
        Humanoid(w,goblin);
        w.ascii.Add(goblin, 'g');
        w.name.Add(goblin, "goblin");
        w.speed.Add(goblin, 5);
        w.race.Add(goblin, AuxTypes.Race.orc);
        w.alignment.Add(goblin, AuxTypes.Alingment.chaotic);
        AuxTypes.Attributes attributes = new AuxTypes.Attributes
        {
            strength = 4,
            constitution = 2,
            intelligence = 2
        };
        AuxTypes.Health goblin_health = new AuxTypes.Health
        {
            current = 5,
            max = 5,
            regeneration = 1 // si es por turno deberia ser poca y max mucha...
        };
        AuxTypes.Sight sight = new AuxTypes.Sight
        {
            sight = true,
            see_invis = false,
            range = 10
        };
        w.attributes.Add(goblin, attributes);
        w.health.Add(goblin, goblin_health);
        w.sight.Add(goblin,sight);
        return goblin;
    }

    public static int Human (World w)
    {
        int human = IDManager.get_id();
        Humanoid(w, human);
        w.ascii.Add(human, '@');
        w.name.Add(human, "human");

        w.speed.Set(human, 6);
        w.turn_order.Add(human); // estar 2 veces en la lista de turnos = actuar 2 veces por turno (no nec. seguidas)
        w.race.Add(human, AuxTypes.Race.human);
        w.alignment.Add(human, AuxTypes.Alingment.neutral);
        AuxTypes.Attributes attributes = new AuxTypes.Attributes
        {
            strength = 5,
            constitution = 3,
            intelligence = 3
        };
        AuxTypes.Health human_health = new AuxTypes.Health
        {
            current = 10,
            max = 10,
            regeneration = 2 // si es por turno deberia ser poca y max mucha...
        };
        AuxTypes.Sight sight = new AuxTypes.Sight
        {
            sight = true,
            see_invis = false,
            range = 30
        };
        w.attributes.Add(human, attributes);
        w.health.Add(human, human_health);
        w.sight.Add(human, sight);

        return human;
    }

    //inicialziar facil lista de equipment slots
    static List<AuxTypes.EquipmentSlot> Slots(params AuxTypes.EquipmentType[] types)
        {
            var list = new List<AuxTypes.EquipmentSlot>();
            foreach (var t in types)
                list.Add(new AuxTypes.EquipmentSlot { type = t, item = -1 });
            return list;
        }
}