public class World
{
    public int Player = -1; // por ahora puedo controlar 1 entidad nomas
    // componentes

    //=====================================================================================================================================
    // Componentes Físicos
    //=====================================================================================================================================
    public SparseSet<AuxTypes.PhysicsComponent> PhysicsComponent = new();         // pos, hitbox
    public SparseSet<AuxTypes.MovementComponent> MovementComponent = new();        // se le sumara a la pos para actualizarla
    public SparseSet<char> ascii = new();                                          // placeholder
    public SparseSet<bool> solid = new();                                          // bloqueo movimiento/vision?
    //=====================================================================================================================================
    // Componentes de entidades conscientes / interactuables
    //=====================================================================================================================================
    public SparseSet<string> name = new();                                         // claridad para el player
    //=====================================================================================================================================
    // Informacion global útil por nivel
    //=====================================================================================================================================
    public List<int>[,] GameMap;
    public int Tick = 0;
    public World()
    {
        // inicializacion de un nivel
        GameMap = new List<int>[Config.WIDTH, Config.HEIGHT];

        for (int i = 0; i < Config.WIDTH; i++)
            for (int j = 0; j < Config.HEIGHT; j++)
                GameMap[i, j] = new List<int>();
    }
    /*public void destroy_entity(int id)
    {
        MapUtils.RemoveFromMap(this, id);  
        // limpia GameMap y aux_map, recordar que son diferencias con resto de comps para pathfinding

        position.Remove(id);
        movement.Remove(id);
        energy.Remove(id);
        ai_behaviour.Remove(id);
        has_turn.Remove(id);
        speed.Remove(id);
        name.Remove(id);

        parent.Remove(id);
        children.Remove(id);
        equipment.Remove(id);
        size.Remove(id);
        equipped_ids.Remove(id);
        equipped_in.Remove(id);
        pending_actions.Remove(id);
        
        ascii.Remove(id);
        map_blocks.Remove(id);

        alignment.Remove(id);
        race.Remove(id);
        attributes.Remove(id);
        health.Remove(id);

        detected_enemies.Remove(id);
        sight.Remove(id);
        attack_targets.Remove(id);
        damage.Remove(id);
        death_effect.Remove(id);

        holding.Remove(id);
        held_by.Remove(id);

        IDManager.destroy(id);
        if (id == player) 
            {player = -1;}
    }
    */

}