public static class StateSystem
{
    public static void Run(World w)
    //  chequeo si debe comenzar otra animacion, loopear la actual... etc
    //  por ahora solo sincroniza estado real con animacion
    {
        for (int i = 0; i<w.StateComponent.dense.Count; i++)
        {
            int id = w.StateComponent.valid_ids[i];
            if(w.AnimationComponent.Has(id)){
                var stateComponent = w.StateComponent.dense[i];
                var animationComponent = w.AnimationComponent.Get(id);

                int currentAnimationState = animationComponent.textureRow;
                int currentState = (int) stateComponent.state;

                if (currentAnimationState != currentState)
                // la animacion no puede cambiar de estado, al reves sí
                // hubo cambio de estado -> DEBE haber cambio de animacion
                {
                    animationComponent.currentFrame = 0;
                    animationComponent.frameTime = 0;
                    animationComponent.textureRow = currentState;
                    // asumo por ahora que todas las animaciones duran lo mismo, miden lo mismo
                    w.AnimationComponent.Set(id, animationComponent);
                }
            }
        }
        
    }
}