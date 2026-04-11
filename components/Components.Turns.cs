using static ComponentFlags;
public partial class Components
{
    public void AddPlayerInput(int id)
    {
        component_flags[id] |= HAS_PLAYER_INPUT;
        player_input_group.AddEntity(id);
    }
    public void RemovePlayerInput(int id)
    {
        component_flags[id] &= ~HAS_PLAYER_INPUT;
        player_input_group.RemoveEntity(id);
    }
    public void AddHasTurn(int id)
    {
        component_flags[id] |= HAS_TURN;
        has_turn_group.AddEntity(id);
    }
    public void RemoveHasTurn(int id)
    {
        component_flags[id] &= ~ HAS_TURN;
        has_turn_group.RemoveEntity(id);
    }
}