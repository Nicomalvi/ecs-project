using Raylib_cs;
using System.Numerics;
public static class RenderSystem
{
    public static void Run(World W)
    {
        // recordar: esto empieza a dibujar en la esquina izq. superior de la pantalla
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);
    
        // PLACEHOLDER
        // dibujo todas las hitbox 
        for (int i = 0; i < W.PhysicsComponent.dense.Count; i++)
        {
            int id = W.PhysicsComponent.valid_ids[i];
            if(W.sprite.Has(id)){continue;} // PLACEHOLDER si algo no tiene sprite, dibujo la hitbox
            var physics = W.PhysicsComponent.dense[i];
            Raylib.DrawRectangle(
                (int)physics.x,
                Config.HEIGHT - (int)physics.height - (int)physics.y,
                (int)physics.width,
                (int)physics.height,
                Color.White);
        }

        for (int i = 0; i < W.sprite.dense.Count; i++)
        {
            int id = W.sprite.valid_ids[i];
            var p = W.PhysicsComponent.Get(id);
            var sprite = W.sprite.dense[i];

            AuxTypes.FacingDirection dir = p.facing;
            int widthSign = dir == AuxTypes.FacingDirection.right ? 1 : -1;

            Texture2D texture = W.textures[sprite.textureID];
            Rectangle source = new Rectangle(
                sprite.textureX, 
                sprite.textureY, 
                sprite.textureWidth * widthSign, 
                sprite.textureHeight);
            // rectangulo = pos, dimensiones
            Vector2 position = new Vector2(p.x, Config.HEIGHT - p.height - p.y);
            Raylib.DrawTextureRec(texture, source, position, Color.White);
        }

        Raylib.EndDrawing();
    }
}