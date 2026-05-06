public static class AuxTypes
{
    // RECORDAR: STRUCT =/= CLASS, NO SE PASAN POR REF
    public struct PhysicsComponent
    {
        public float x;
        public float y;

        // para hitbox
        public float width;
        public float height;
        public float offsetX;
        public float offsetY;
    }
    public struct MovementComponent
    {
        public float vx;
        public float vy;
    }

    public struct AnimationComponent
    {
        public int textureX;            // describen mi pos. en la textura
        public int textureY;
        public int textureWidth;
        public int textureHeight;

        public float frameTime;         // cuanto tiempo paso desde que comenzó este frame?

        public int currentFrame;          
        public int maxFrame;            // aca termina la animacion
        public string nextAnimation;    // que archivo cargar una vez que termino este
    }

    public struct SpriteComponent
    {
        public int textureX;            // describen mi pos. en la textura
        public int textureY;
        public int textureWidth;
        public int textureHeight;
    }
}