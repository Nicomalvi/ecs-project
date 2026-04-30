using Raylib_cs;

public static class Config
{
    public const int MAX_ENTITIES = 2000;
    // TrimExcess puede ayudar en sparse
    public const short WIDTH = 20;
    public const short HEIGHT = 20;
    public const short FLOORS = 20;
    public const short PAGE_SIZE = 64;
    public const short MESSAGE_LINES = 8;
    public const short INPUT_Y = HEIGHT + 1 + MESSAGE_LINES + 1;

    public const int CELL_SIZE = 32;

    public static int UP_KEY =          (int) KeyboardKey.W;
    public static int LEFT_KEY =        (int) KeyboardKey.A;
    public static int RIGHT_KEY =       (int) KeyboardKey.D;
    public static int DOWN_KEY =        (int) KeyboardKey.S;
}