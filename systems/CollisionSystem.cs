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
                if(collisionVector == (0, -1))
                {
                    hitbox.y += 20;
                    MapUtils.RemoveFromHitboxMap(w,id);
                    w.Hitbox.Set(id,hitbox);
                    position.y += 20;
                    w.Position.Set(id, position);
                    MapUtils.AddToHitboxMap(w,id);
                }
            }
        }
    }
}