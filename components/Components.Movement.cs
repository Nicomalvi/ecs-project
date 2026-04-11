using static ComponentFlags;
public partial class Components
{
    // funciones que agregan flags y se fijan de meter en otros grupos
    public void AddMoveIntent(int id)
    {
        component_flags[id] |= HAS_MOVE_INTENT;
        move_intent_group.AddEntity(id);
        if (will_move.Belongs(component_flags[id]))
            will_move.AddEntity(id);
    }
    public void RemoveMoveIntent(int id)
    {
        // si tengo los bits necesarios para estar en el grupo, llamo a remove
        // puede que no esté por alguna razon pero chequear los bits con la mascara es O(1), mejor que remover de 1
        if(will_move.Belongs(component_flags[id])){ will_move.RemoveEntity(id); }
        component_flags[id] &= ~HAS_MOVE_INTENT;
        move_intent_group.RemoveEntity(id);
    }
    public void AddPosition(int id, (short x, short y) value)
    {
        position.Add(id, value);
        component_flags[id] |= HAS_POSITION;
        if (will_move.Belongs(component_flags[id]))
            will_move.AddEntity(id);
    }
    // REMOVE POSITION POR AHORA NO
    public void AddMovement(int id, (short x, short y) value)
    {
        movement.Add(id, value);
        component_flags[id] |= HAS_MOVEMENT;
        if (will_move.Belongs(component_flags[id]))
            will_move.AddEntity(id);
    }
    public void RemoveMovement(int id)
    {
        if(will_move.Belongs(component_flags[id])){ will_move.RemoveEntity(id); }
        movement.Remove(id);
        component_flags[id] &= ~HAS_MOVEMENT;
    }
    public void AddCanMove(int id)
    {
        component_flags[id] |= CAN_MOVE;
        if (will_move.Belongs(component_flags[id]))
        {
            will_move.AddEntity(id);
        }
    }
    public void RemoveCanMove(int id)
    {
        if (will_move.Belongs(component_flags[id]))
        {
            will_move.RemoveEntity(id);
        }
        component_flags[id] &= ~CAN_MOVE;
    }
}