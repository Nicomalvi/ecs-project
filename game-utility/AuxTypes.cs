public static class AuxTypes
{
    // LOS STRUCT NO SE MODIFICAN DE UNA, DEBO SETEARLOS DE VUELTA
    public enum Alingment
    {
        chaotic,
        neutral,
        lawful
    }
    public enum Race
    {
        human,
        orc,
        elf,
        other
    }
    public struct Attributes
    {
        public short strength;
        public short constitution;
        public short intelligence;
    }
    public struct Health
    {
        public short current;
        public short max;
        public short regeneration;
    }
    public struct Energy
    {// LOS STRUCT NO SE MODIFICAN DE UNA, DEBO SETEARLOS DE VUELTA
        public short current;
        public short max;
        public short regeneration;
    }
    public struct Damage
    {// LOS STRUCT NO SE MODIFICAN DE UNA, DEBO SETEARLOS DE VUELTA
        public string type;
        public short amount;
    }
    public struct Sight
    {
        public bool sight;
        public bool see_invis;
        public short range;
    }
}