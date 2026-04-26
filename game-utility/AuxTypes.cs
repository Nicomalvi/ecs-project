public static class AuxTypes
{
    
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
    public enum AiState
    {
        idle,
        chase,
        melee_attack // accion?
    }
    public enum EquipmentType
    {
        head,
        torso,
        legs,
        feet,
        arms,
        hands,
        melee_weapon
    }
    public enum Size
    {
        tiny,       // rata
        average,    // orco
        giant       // dragon
    }
    public enum DeathEffect
    {
        none
    }
    public enum ActionType {
    pickup, drop, equip, unequip, melee_attack
    }
    // RECORDAR: STRUCT =/= CLASS, NO SE PASAN POR REF
    public struct EquipmentSlot
    {
        public EquipmentType type;
        public int item;
    }
    public struct PendingAction
    {
        public List<int> target_ids;
        public ActionType type;
        
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