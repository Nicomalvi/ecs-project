public static class ActionSystem
{
    public static void Run(World w){
        for (int i = 0; i< w.pending_actions.dense.Count; i++)
        {
            int id = w.pending_actions.valid_ids[i];
            var action = w.pending_actions.Get(id);
            switch (action.type)
            {
                case AuxTypes.ActionType.pickup:        Actions.PickUp(w, id, action.target_ids[0]); break;
                case AuxTypes.ActionType.drop:          Actions.Drop(w, id, action.target_ids[0]); break;
                case AuxTypes.ActionType.equip:         Actions.Equip(w, id, action.target_ids[0]); break;
                case AuxTypes.ActionType.unequip:       Actions.Unequip(w, id, action.target_ids[0]); break;
                case AuxTypes.ActionType.melee_attack:  Actions.MeleeAttack(w, id, action.target_ids[0]); break;
            }
            w.pending_actions.Remove(id);
        }
    }   
}