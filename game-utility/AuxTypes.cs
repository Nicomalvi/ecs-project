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
        public float offset_x;
        public float offset_y;
    }
    public struct MovementComponent
    {
        public float vx;
        public float vy;
    }
}