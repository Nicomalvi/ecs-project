using Raylib_cs;

public static class Components
{
    // STRUCT =/= CLASS
    // ============================================================
    // FISICAS
    // ============================================================
    public struct Physics
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

        public bool grounded;

        public FacingDirection facing;
        public bool hasMoved;

        public bool solid;
    }
    public struct Movement
    {
        public float velX;
        public float maxVelX;
        public float velY;
        public float maxVelY;

        public bool currentlyMoving; 
        // para diferenciar si mi velocidad viene de intentar moverme o es rastro de velocidad alta vieja
    }
    public struct MovementData
    {
        public bool isGrounded;             // termine de moverme arriba de alguna plataforma?
        public bool movedIndividuallyX;     // intenté moverme por mi cuenta y lo logré?
        public bool movedFromFriction;      // el movimiento por mi cuenta, fue mío o fricción?
    }
    // ============================================================
    // RENDERIZADO
    // ============================================================
    public struct Animation
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
    public enum State
    {   // cambiara con cada accion o fin de accion
        // animation, input, ia trabajan con esto
        idle,       // QUIETO
        moving,     // MOVIENDOSE (POR VOLUNTAD PROPIA)
        falling    // EN EL AIRE / SIENDO AFECTADO POR GRAVEDAD
    }
    public struct EntityState
    {
        public State state; // en que estado esta la entidad?
        public float lockTimer;    // por cuanto tiempo no puede hacer nada mas?
    }
    public struct Sprite
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