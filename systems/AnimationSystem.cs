using Raylib_cs;
public static class AnimationSystem
{   // este sistema solo avanza las animaciones
    // deja preparada la textura para el render system
    //      elegir cuando ejecutar una nueva animacion: movement system, input system...
    //      chequear si puedo interrumpir una animacion: movement system, input system...
    public static void Run(World w)
    {
        float dt = Raylib.GetFrameTime();

        for (int i = 0; i < w.AnimationComponent.dense.Count; i++)
        {
            int id = w.AnimationComponent.valid_ids[i];
            var animationComponent = w.AnimationComponent.dense[i];
            var sprite = w.sprite.Get(id);

            animationComponent.frameTime += dt;
            if (animationComponent.frameTime >= Config.FRAME_DURATION)
            {
                // debo pasar de frame!!
                animationComponent.frameTime =- Config.FRAME_DURATION; // si tarde 0.18, quiero que el prox comience en 0.2
                animationComponent.currentFrame++;  
            }

            if (animationComponent.currentFrame >= animationComponent.maxFrame)
            {
                animationComponent.currentFrame = 0; 
                // por las dudas, igualmente loopear una anim. no se deberia solucionar aqui
            }

            int spriteX = animationComponent.currentFrame * animationComponent.textureWidth;
            int spriteY = animationComponent.textureRow * animationComponent.textureHeight;

            sprite.textureX = spriteX;
            sprite.textureY = spriteY;

            w.AnimationComponent.Set(id,animationComponent);
            w.sprite.Set(id,sprite);
        }
    }
}