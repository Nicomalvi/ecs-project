public static class CollisionSystem
{
    public static void Run(World w)
    {
        for(int i = 0; i < w.CollisionList.dense.Count; i++)
        {
            int id = w.CollisionList.valid_ids[i];
            var collisionList = w.CollisionList.dense[i];
            var hitbox = w.Hitbox.Get(id);
            var position = w.Position.Get(id);
            for (int j = 0; j < collisionList.Count; j++)
            {
                (int other, (int,int) collisionVector) = collisionList[j];
                //normalmente chequearia que tipo de colision sucede con other... 
                //PLACEHOLDER!!!!!!!!!!
                var otherHitbox = w.Hitbox.Get(other);
                MapUtils.RemoveFromHitboxMap(w,id);
                if(collisionVector == (0, -1))
                {   // vengo de abajo
                    hitbox.y = otherHitbox.y - hitbox.height;
                    w.Hitbox.Set(id,hitbox);
                    position.y = hitbox.y;
                    w.Position.Set(id, position);
                }
                if(collisionVector == (0, 1))
                {   // vengo de arriba
                    hitbox.y = otherHitbox.y + otherHitbox.height;
                    w.Hitbox.Set(id,hitbox);
                    position.y = hitbox.y;
                    w.Position.Set(id, position);
                }
                if(collisionVector == (-1, 0))
                {   // vengo de izq
                    hitbox.x = otherHitbox.x - hitbox.width;
                    w.Hitbox.Set(id,hitbox);
                    position.x = hitbox.x;
                    w.Position.Set(id, position);
                }
                if(collisionVector == (1, 0))
                {   // vengo de der
                    hitbox.x = otherHitbox.x + otherHitbox.width;
                    MapUtils.RemoveFromHitboxMap(w,id);
                    w.Hitbox.Set(id,hitbox);
                    position.x = hitbox.x;
                    w.Position.Set(id, position);
                }
                MapUtils.AddToHitboxMap(w,id);
            }
        }
    }
}