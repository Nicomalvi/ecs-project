using System.ComponentModel;
using static ComponentFlags;

public partial class Components
{   
    public ulong[] component_flags;        // con O(1) veo las flags de una entidad

    // sparse set: por cada id asocio datos de un componente
    // recuerdo: cada sparse set guarda además qué ids contiene
    public SparseSet<(short x, short y)> position;
    public SparseSet<short> health;
    public SparseSet<char> ascii;
    public SparseSet<(short x, short y)> movement;
    public SparseSet<short> weight;
    public SparseSet<short> inventory_weight;
    public SparseSet<List<int>> inventory;

    // uso ComponentGroups para:
    // 1) agrupar entidades con componentes sin sparse list (como player input)
    // 2) agrupar entidades con ciertas combinaciones de componentes (ej: will_move

    public ComponentGroup player_input_group;
    public ComponentGroup move_intent_group;
    public ComponentGroup pickup_intent_group;
    public ComponentGroup has_turn_group;
    // grupos de mas de 1 componente
    public ComponentGroup will_move;

    public Components()
    {
        component_flags = new ulong[Config.MAX_ENTITIES];

        position = new SparseSet<(short, short)>();
        health = new SparseSet<short>();
        ascii = new SparseSet<char>();
        movement = new SparseSet<(short, short)>();
        weight = new SparseSet<short>();
        inventory_weight = new SparseSet<short>();
        inventory = new SparseSet<List<int>>();

        player_input_group    = new ComponentGroup(HAS_PLAYER_INPUT);
        has_turn_group        = new ComponentGroup(HAS_TURN);
        
        move_intent_group     = new ComponentGroup(HAS_MOVE_INTENT);
        pickup_intent_group   = new ComponentGroup(HAS_PICKUP_INTENT);

        // grupos de mas de 1 componente
        will_move             = new ComponentGroup(HAS_MOVE_INTENT|HAS_POSITION|HAS_MOVEMENT|CAN_MOVE);
    }
}
public partial class Components
{
// funciones temporales--------------------------------------------------------------------------------
    public void AddAscii(int id, char c)
    {
        component_flags[id] |= HAS_ASCII;
        ascii.Add(id, c);
    }
    public void RemoveAscii(int id)
    {
        component_flags[id] &= ~HAS_ASCII;
        ascii.Remove(id);
    }
    public void AddWeight(int id, short w)
    {
        component_flags[id] |= HAS_WEIGHT;
        weight.Add(id, w);
    }
    public void RemoveWeight(int id)
    {
        component_flags[id] &= ~HAS_WEIGHT;
        weight.Remove(id);
    }
}
public class ComponentGroup
{
    public ulong required_flags;
    public List<int> entities;

    public ComponentGroup(ulong flags)
    {
        required_flags = flags;
        entities = new List<int>();
    }
    public void AddFlag(ulong flag) {required_flags |= flag; } // probablemente innecesario
    public void AddEntity(int id)
    {
        if (entities.Contains(id)) return;
        entities.Add(id);
    }
    public void RemoveEntity(int id) { entities.Remove(id); }
    public bool Belongs(ulong entity_components) { return (required_flags & entity_components) == required_flags; }
}   // Belongs es O(1) gracias al uso de máscaras !

public static class ComponentFlags
{   
    // componentes con datos
    public const ulong HAS_POSITION        = 1ul << 0;
    public const ulong HAS_MOVEMENT        = 1ul << 3;
    public const ulong CAN_MOVE            = 1ul << 12;

    public const ulong HAS_HEALTH          = 1ul << 1;
    public const ulong HAS_ASCII           = 1ul << 2;

    public const ulong HAS_INVENTORY       = 1ul << 5;
    public const ulong HAS_INVENTORY_WEIGHT= 1ul << 6;

    public const ulong HAS_WEIGHT          = 1ul << 4;

    // flags puras
    public const ulong IS_ITEM             = 1ul << 8;

    public const ulong HAS_MOVE_INTENT     = 1ul << 7;
    public const ulong HAS_PICKUP_INTENT   = 1ul << 9;

    public const ulong HAS_PLAYER_INPUT    = 1ul << 10;

    public const ulong HAS_TURN            = 1ul << 11;
}