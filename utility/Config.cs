using Raylib_cs;

public static class Config
{
    // ========================================================================
    // configuracion arquitectura
    // ========================================================================
    public const int MAX_ENTITIES = 2000;
    // TrimExcess puede ayudar en sparse
    public const short WIDTH = 800;
    public const short HEIGHT = 600;
    public const short FLOORS = 20;
    public const short PAGE_SIZE = 64;
    // ========================================================================
    // info para animaciones, sprites
    // ========================================================================
    public const int CELL_SIZE = 32;
    public const float FRAME_DURATION = 1/60;
    // quiero que un frame (armo las aniamciones con frames) dure 1frame/60frames por segundo = 0.16 segundos
    // de esta manera tengo velocidad de animacion medida en segundos y no en frames,
    // alguien que corre el juego mas rapido ve las mismas animaciones
    public const int TEXTURE_AMOUNT = 1;
    // ========================================================================
    // inputs que en un futuro se podran cambiar de tecla
    // ========================================================================
    public static int UP_KEY =          (int) KeyboardKey.W;
    public static int LEFT_KEY =        (int) KeyboardKey.A;
    public static int RIGHT_KEY =       (int) KeyboardKey.D;
    public static int DOWN_KEY =        (int) KeyboardKey.S;
}