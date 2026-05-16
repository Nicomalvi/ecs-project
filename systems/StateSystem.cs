public static class StateSystem
{
    public static void Run(World w)
    //  chequeo si debe comenzar otra animacion, loopear la actual... etc
    //  por ahora solo sincroniza estado real con animacion
    {
        for (int i = 0; i<w.StateComponent.dense.Count; i++)
        {
            int id = w.StateComponent.valid_ids[i];
            if(w.Animation.Has(id)){
                var stateComponent = w.StateComponent.dense[i];
                var animation = w.Animation.Get(id);
                var movementData = w.MovementData.Get(id);
                bool finishedGrounded = movementData.isGrounded;
                bool movedIndividually = movementData.movedIndividuallyX;
                //bool movedFromFriction = movementData.movedFromFriction;
                bool affectedByGravity = false;
                if(w.Gravity.Has(id) && w.Gravity.Get(id)){affectedByGravity = true;}
                Components.EntityState finalState = new Components.EntityState {lockTimer = 0, state = Components.State.idle};
                // aca deberia ir chequeo de si puedo cambiar de estado, quizas estoy en un estado bloqueante 
                // if else mejor performance?
                // estoy en el aire, tengo gravedad -> FALLING
                if (!finishedGrounded && affectedByGravity){finalState.state = Components.State.falling;}
                // estoy en el aire, no me afecta la gravedad...
                if (!finishedGrounded && !affectedByGravity){
                    if(movedIndividually){finalState.state = Components.State.moving;}
                    if(!movedIndividually){finalState.state = Components.State.idle;}
                    }
                // estoy en el piso...
                if (finishedGrounded)
                {
                    // me quede quieto, o si me movi fue por la friccion -> idle
                    //if(!movedIndividually || (movedIndividually && movedFromFriction)){finalState.state = Components.State.idle;}
                    // me movi y fue por mi cuenta -> moving
                    //if(movedIndividually && !movedFromFriction){finalState.state = Components.State.moving;}
                }
                w.StateComponent.Set(id, finalState);

                int currentAnimationState = animation.textureRow;
                int currentState = (int) finalState.state;

                if (currentAnimationState != currentState)
                // la animacion no puede cambiar de estado, al reves sí
                // hubo cambio de estado -> DEBE haber cambio de animacion
                {
                    animation.currentFrame = 0;
                    animation.frameTime = 0;
                    animation.textureRow = currentState;
                    // asumo por ahora que todas las animaciones duran lo mismo, miden lo mismo
                    w.Animation.Set(id, animation);
                }
            }
        }
        
    }
}