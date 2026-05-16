using Raylib_cs;

public static class Components
{
    // STRUCT =/= CLASS
    // ============================================================
    // FISICAS
    // ============================================================
    public struct Movement2
    {
        public float vx;
        public float vy;
        public float max;
    }
    public struct Position
    {
        public float x;
        public float y;
    }
    public struct Hitbox
    {
        public float width;
        public float height;
        // misma x, y que position importa?
        public float x;
        public float y;
        public CollisionType collisionType;
    }
    public struct MovementData
    {
        public bool hasMoved;
        public bool isGrounded;             // termine de moverme arriba de alguna plataforma?
        public bool movedIndividuallyX;     // intenté moverme por mi cuenta y lo logré?
        public bool movedIndividuallyY;     // intenté moverme por mi cuenta y lo logré?
        public bool frictionMovementX;      // el movimiento por mi cuenta, fue mío o fricción?
        public bool frictionMovementY;      // el movimiento por mi cuenta, fue mío o fricción?
    }
    public enum CollisionType
    {
        platform,    // choco con TODO
        actor,       // choco con plataformas, otros actores
        item,        // choco solo con plataformas
        nothing      // no soy considerado en colisones
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
    public static void AddBasicPhysics(World w, int id, float x, float y, float width, float height)
    {
        Movement2 movement = new Movement2 {vx = 0, vy = 0, max = 1000};
        w.Movement2.Add(id,movement);
        Position position = new Position{x = x, y = y};
        w.Position.Add(id, position);
        Hitbox hitbox = new Hitbox{x = x, y = y, width = width, height = height};
        w.Hitbox.Add(id, hitbox);
        MovementData moveData = new MovementData{isGrounded = false, frictionMovementX = false, frictionMovementY = false,
        movedIndividuallyX = false, movedIndividuallyY = false, hasMoved = false};
        w.MovementData.Add(id, moveData);

        MapUtils.AddToHitboxMap(w,id);
    }
}