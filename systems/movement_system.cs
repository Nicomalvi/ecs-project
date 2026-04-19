public static class MovementSystem
{
    public static void Run(World w)
    {
        var mov = w.movement;
        for (int i = 0; i < mov.dense.Count; i++) // estoy iterando sobre los que tienen w.movement, no hace fala chequear
        {
            int id = mov.valid_ids[i];
            if (!w.position.Has(id))
            // se podria evitar si asumo que w.movement implica w.position
            // costoso? branches?
                continue;
            var pos = w.position.Get(id);
            var m = mov.dense[i];
            int newX = pos.x + m.x;
            int newY = pos.y + m.y;
            if (newX >= Config.WIDTH || newX < 0 || newY >= Config.HEIGHT || newY < 0) continue;
            if (w.aux_map[newX, newY].blocks_movement > 0) continue; // si choco no me muevo, esto podria cambiar (abrir puertas, ataque, swap)
            MapUtils.MoveOnMap(w, id, (short)newX, (short)newY);
            w.movement.Set(id, (0, 0));
        }
    }
}