public static class Prefabs
{
    public static int Goblin (World w)
    {
        int goblin = IDManager.get_id();
        w.ascii.Add(goblin, 'g');
        w.map_blocks.Add(goblin, (true,false));

        //w.energy.Add(goblin, (5,0,2));
        w.ai_behaviour.Add(goblin, "chase");

        w.race.Add(goblin, AuxTypes.Race.orc);
        w.alignment.Add(goblin, AuxTypes.Alingment.chaotic);

        AuxTypes.Attributes attributes = new AuxTypes.Attributes
        {
            strength = 4,
            constitution = 2,
            intelligence = 2
        };
        w.attributes.Add(goblin, attributes);
        AuxTypes.Health goblin_health = new AuxTypes.Health
        {
            current = 5,
            max = 5,
            regeneration = 1 // si es por turno deberia ser poca y max mucha...
        };
        w.health.Add(goblin, goblin_health);

        AuxTypes.Sight sight = new AuxTypes.Sight
        {
            sight = true,
            see_invis = false,
            range = 10
        };
        w.sight.Add(goblin,sight);
        w.holding.Add(goblin, []); // tener ese componente -> puedo agarrar cosas
        return goblin;
    }

    public static int Human (World w)
    {
        int human = IDManager.get_id();
        w.ascii.Add(human, '@');
        w.map_blocks.Add(human, (true,false));

        //w.energy.Add(human, (5,0,3));
        w.ai_behaviour.Add(human, "chase");

        w.race.Add(human, AuxTypes.Race.human);
        w.alignment.Add(human, AuxTypes.Alingment.neutral);
        AuxTypes.Attributes attributes = new AuxTypes.Attributes
        {
            strength = 5,
            constitution = 3,
            intelligence = 3
        };
        w.attributes.Add(human, attributes);
        AuxTypes.Health human_health = new AuxTypes.Health
        {
            current = 10,
            max = 10,
            regeneration = 2 // si es por turno deberia ser poca y max mucha...
        };
        w.health.Add(human, human_health);
        AuxTypes.Sight sight = new AuxTypes.Sight
        {
            sight = true,
            see_invis = false,
            range = 30
        };
        w.sight.Add(human, sight);
        w.holding.Add(human, []); // tener ese componente -> puedo agarrar cosas

        return human;
    }
}