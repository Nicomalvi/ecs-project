using Raylib_cs;
using System.Numerics;
public static class RenderSystem
{
    public static void Run(World W)
    {
        // recordar: esto empieza a dibujar en la esquina izq. superior de la pantalla
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        //Raylib.DrawTexture(W.background,0,0,Color.White);
    
        // PLACEHOLDER
        // dibujo todas las hitbox 
        for (int i = 0; i < W.Hitbox.dense.Count; i++)
        {
            int id = W.Hitbox.valid_ids[i];
            if(W.Sprite.Has(id)){continue;} // PLACEHOLDER si algo no tiene sprite, dibujo la hitbox
            var hitbox = W.Hitbox.dense[i];
            Raylib.DrawRectangle(
                (int)hitbox.x,
                Config.HEIGHT - (int)hitbox.height - (int)hitbox.y,
                (int)hitbox.width,
                (int)hitbox.height,
                Color.White);
        }

        for (int i = 0; i < W.Sprite.dense.Count; i++)
        {
            int id = W.Sprite.valid_ids[i];
            var p = W.Hitbox.Get(id);
            var sprite = W.Sprite.dense[i];

            //Components.FacingDirection dir = p.facing;
            //int widthSign = dir == Components.FacingDirection.right ? 1 : -1;
            int widthSign = 1;
            
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