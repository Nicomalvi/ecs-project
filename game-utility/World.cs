public class World
{
    public int player = -1; // por ahora puedo controlar 1 entidad nomas
    // componentes

    //=====================================================================================================================================
    // Componentes Físicos
    //=====================================================================================================================================
    public SparseSet<(short x, short y)> position = new();                         // pos en el mapa
    public SparseSet<(short x, short y)> movement = new();                         // se le sumara a la pos para actualizarla
    public SparseSet<char> ascii = new();                                          // como se ve si esta arriba de todo en el mapa (relacion con race?)
    public SparseSet<(bool blocks_move,bool blocks_vision)> map_blocks = new();    // bloqueo movimiento/vision?
    // se puede cambiar por si soy solido, pequeño, grande...

    //=====================================================================================================================================
    // Componentes de entidades conscientes / interactuables
    //=====================================================================================================================================
    public SparseSet<AuxTypes.Energy> energy = new();                              // energia
    public SparseSet<AuxTypes.AiState> ai_behaviour = new();                       // FSM?
    public SparseSet<AuxTypes.PendingAction> pending_actions = new();              // que voy a hacer este turno?
    public SparseSet<bool> has_turn = new();                                       // ia + me toca (+ tengo energia?) = actuar
    public SparseSet<short> speed = new();                                         // util para generar turnos
    public SparseSet<string> name = new();                                         // claridad para el player
    public SparseSet<AuxTypes.Alingment> alignment = new();                        // lawful neutral evil
    public SparseSet<AuxTypes.Race> race = new();                                  // raza
    public SparseSet<AuxTypes.Attributes> attributes = new();                      // fuerza, const, inteligencia...
    public SparseSet<AuxTypes.Health> health = new();                              // vida actual, máxima, regeneración

    //=====================================================================================================================================
    // Componentes relacionados con inventario, equipamiento, partes del cuerpo
    //=====================================================================================================================================
    public SparseSet<int> parent = new();                                          // para entidades con partes. si parent fuera lista...
    public SparseSet<List<int>> children = new();                                  // para entidades con partes
    public SparseSet<List<AuxTypes.EquipmentSlot>> equipment = new();              // [(tipo_slot, item que tengo), (tipo_slot, item que tengo)...]
    public SparseSet<List<int>> equipped_ids = new();                              // lista de ids de items que equipo
    public SparseSet<int> equipped_in = new();                                     // que parte del cuerpo me tiene equipado?
    public SparseSet<AuxTypes.EquipmentType> equipment_type = new();               // si soy un tipo de equipamiento, cual
    public SparseSet<AuxTypes.Size> size = new();                                  // combinado con otros comp., interacciones piola
    public SparseSet<List<int>> holding = new();                                   // cuales entidades estoy guardando? (no tener = no puedo llevar nada)
    public SparseSet <int> held_by = new();                                        // quien me tiene en su inventario? (si esto fuera una lista quizas varias personas tienen 1 item! interesante)

    //=====================================================================================================================================
    // Componentes que interactuan con IA/Input
    //=====================================================================================================================================
    public SparseSet<List<int>> detected_enemies = new();                          // enemigos que puedo percibir (ya sea rango de vision, telepatia)
    public SparseSet<AuxTypes.Sight> sight = new();                                // puedo ver? puedo ver invisibles? que tan lejos puedo ver?
    public SparseSet<List<int>> attack_targets = new();                            // estoy haciendo daño? a quien/quienes?
    public SparseSet<AuxTypes.Damage> damage = new();                              // cuanto daño hago?
    public SparseSet<AuxTypes.DeathEffect> death_effect = new();                   // que pasa cuando muero?
   
    // tipo de entidad que sea (counter, que hacer)? EJ. en 10 turnos devolver movement a alguien que le saque, en 4 turnos destrozar arma... etc

    // CONSIDERAR structs como AI BUNDLE o PHYSICS BUNDLE, SoA e iterar sobre eso para volver a la cache friendly

    // CONSIDERAR arrays con tamaños fijos para grupos? ej. , en movement[1000] a movement[3000] habra solo IDS que tambien tienen
    // position, IA, velocidad. De esta manera se puedo iterar de forma contigua varios arrays a la vez. 

    // CONSIDERAR un poco de cache locality sigue siendo mejor que nada :D

    //=====================================================================================================================================
    // Informacion global útil por nivel
    //=====================================================================================================================================
    public List<int>[,] game_map;
    public (int blocks_movement, int blocks_vision)[,] aux_map;
    public int tick = 0;
    public List<string> announcement_list = new(); // los mensajes que agregan todos los sistemas
    // ahora mismo AddMesagge es muy facil, en un futuro solo se deberia agregar lo que el jugador puede percibir
    public World()
    {
        // inicializacion de un nivel
        game_map = new List<int>[Config.WIDTH, Config.HEIGHT];
        aux_map  = new (int, int)[Config.WIDTH, Config.HEIGHT];

        for (int i = 0; i < Config.WIDTH; i++)
            for (int j = 0; j < Config.HEIGHT; j++)
                game_map[i, j] = new List<int>();
    }
    public void destroy_entity(int id)
    {
        MapUtils.RemoveFromMap(this, id);  
        // limpia game_map y aux_map, recordar que son diferencias con resto de comps para pathfinding

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

}