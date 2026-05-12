using Raylib_cs;
public static class AnimationSystem
{   // este sistema solo avanza las animaciones
    // deja preparada la textura para el render system
    //      elegir cuando ejecutar una nueva animacion: movement system, input system...
    //      chequear si puedo interrumpir una animacion: movement system, input system...
    public static void Run(World w)
    {
        float dt = Raylib.GetFrameTime();

        for (int i = 0; i < w.Animation.dense.Count; i++)
        {
            int id = w.Animation.valid_ids[i];
            var Animation = w.Animation.dense[i];
            var sprite = w.Sprite.Get(id);

            Animation.frameTime += dt;
            if (Animation.frameTime >= Config.FRAME_DURATION)
            {
                // debo pasar de frame!!
                Animation.frameTime -= Config.FRAME_DURATION; // si tarde 0.18, quiero que el prox comience en 0.2
                Animation.currentFrame++;  
            }

            if (Animation.currentFrame > Animation.maxFrame)
            {
                Animation.currentFrame = 0; 
                // por las dudas, igualmente loopear una anim. no se deberia solucionar aqui
            }

            int spriteX = Animation.currentFrame * Animation.textureWidth;
            int spriteY = Animation.textureRow * Animation.textureHeight;

            sprite.textureX = spriteX;
            sprite.textureY = spriteY;

            w.Animation.Set(id,Animation);
            w.Sprite.Set(id,sprite);
        }
    }
}