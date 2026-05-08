using Raylib_cs;
using System.Numerics;
public static class RenderSystem
{
    public static void Run(World W)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        for (int i = 0; i < W.sprite.dense.Count; i++)
        {
            int id = W.sprite.valid_ids[i];
            var p = W.PhysicsComponent.Get(id);
            var sprite = W.sprite.dense[i];

            AuxTypes.FacingDirection dir = p.facing;
            int widthSign = dir == AuxTypes.FacingDirection.right ? 1 : -1;

            Texture2D texture = W.textures[sprite.textureID];
            Rectangle source = new Rectangle(sprite.textureX, sprite.textureY, sprite.textureWidth * widthSign, sprite.textureHeight);
            // rectangulo = pos, dimensiones
            Vector2 position = new Vector2(p.x, p.y);
            Raylib.DrawTextureRec(texture, source, position, Color.White);
        }

        Raylib.EndDrawing();
    }
}