using Raylib_cs;
using System.Numerics;
public static class RenderSystem
{
    public static void Run(World W, Texture2D texture)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        for (int i = 0; i < W.PhysicsComponent.dense.Count; i++)
        {
            int id = W.PhysicsComponent.valid_ids[i];
            var p = W.PhysicsComponent.Get(id);
            if(id != W.Player)
            {
                Color color = id == W.Player ? Color.Green : Color.Red;
                Raylib.DrawRectanglePro(
                new Rectangle(p.x, 600 - p.y - p.height, p.width, p.height),
                new Vector2(0, 0),  // origin
                0f,                 // rotation
                color
                );
            }
            if (id == W.Player)
            {   
                var vector = new Vector2(p.x,600-p.y-p.height);
                var rectangle = new Rectangle(0,0,new Vector2(32,32));
                
                if(W.Tick % 20 < 5){rectangle.Position = new Vector2(0,0);}
                if(W.Tick % 20 > 5 && W.Tick % 20 < 10){rectangle.Position = new Vector2(32,0);}
                if(W.Tick % 20 > 10 && W.Tick % 20 < 15){rectangle.Position = new Vector2(0,32);}
                if(W.Tick % 20 > 15 && W.Tick % 20 < 20){rectangle.Position = new Vector2(32,32);}
                Raylib.DrawTextureRec(texture,rectangle,vector,Color.White);
            }
        }

        Raylib.EndDrawing();
    }
}