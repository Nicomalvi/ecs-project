using Raylib_cs;
using System.Numerics;
public static class RenderSystem
{
    public static void Run(World W)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        for (int i = 0; i < W.PhysicsComponent.dense.Count; i++)
        {
            int id = W.PhysicsComponent.valid_ids[i];
            var p = W.PhysicsComponent.Get(id);
            Color color = id == W.Player ? Color.Green : Color.Red;
            Raylib.DrawRectanglePro(
            new Rectangle(p.x, 600 - p.y - p.height, p.width, p.height),
            new Vector2(0, 0),  // origin
            0f,                 // rotation
            color
        );
        }

        Raylib.EndDrawing();
    }
}