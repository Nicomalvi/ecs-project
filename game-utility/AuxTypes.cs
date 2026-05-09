using Raylib_cs;

public static class AuxTypes
{
    // STRUCT =/= CLASS
    // ============================================================
    // FISICAS
    // ============================================================
    public struct PhysicsComponent
    {
        public float x;
        public float y;

        public float deltaX;
        public float deltaY;

        // para hitbox
        public float width;
        public float height;
        public float offsetX;
        public float offsetY;

        public FacingDirection facing;
        public bool hasMoved;
    }
    public struct MovementComponent
    {
        public float vx;
        public float vy;
    }
    // ============================================================
    // RENDERIZADO
    // ============================================================
    public struct AnimationComponent
    {   // importante: frame duration = 1 / animationFPS (que deberia ser lo mismo que gameFPS obviamente)
        public int textureRow;          // describe mi pos. en la textura

        public int textureWidth;
        public int textureHeight;

        public float frameTime;         // cuanto tiempo paso desde que comenzó este frame?

        public int currentFrame;          
        public int maxFrame;            // aca termina la animacion
    }
    public enum FacingDirection
    {
        left,
        right
    }
    public enum EntityStates
    {   // cambiara con cada accion o fin de accion
        // animation, input, ia trabajan con esto
        idle,
        walk,
        falling,
        jump
    }
    public struct EntityStateComponent
    {
        public EntityStates state; // en que estado esta la entidad?
        public float lockTimer;    // por cuanto tiempo no puede hacer nada mas?
    }
    public struct SpriteComponent
    {
        public int textureID;           // el world maneja las texturas, spriteCompone chequea cual agarrar
        public int textureX;            // describen dimensiones EN LA TEXTURA que voy a renderizar
        public int textureY;
        public int textureWidth;
        public int textureHeight;
    }
    // ============================================================
    // ============================================================
}