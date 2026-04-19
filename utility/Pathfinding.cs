public static class Pathfinding
{
    // se allocan una sola vez, se reusan cada llamada
    static short[,]        visited_gen = new short[Config.WIDTH, Config.HEIGHT];
    static short[,]        distance    = new short[Config.WIDTH, Config.HEIGHT];
    static (short x, short y)[,] father = new (short, short)[Config.WIDTH, Config.HEIGHT];
    static short           current_gen = 0;

    static readonly (short dx, short dy)[] dirs8 = new (short, short)[]
    {
        (1,0), (-1,0), (0,1), (0,-1),
        (1,1), (1,-1), (-1,1), (-1,-1)
    };

    // BFS FRENA CUANDO ENCUENTRA EL TARGET SEGUN CONDITION
    public static ((short,short)[,] , (short x, short y) target) BFS(
        List<int>[,] game_map,
        (int blocks_movement, int blocks_vision)[,] aux_map,
        (short x, short y) source,
        Func<(short x, short y), List<int>[,], bool> condition,
        short maxDistance)
    {
        // avanzar generacion: "limpiar" visited sin tocar el array
        current_gen++;
        if (current_gen == short.MaxValue)
        {
            // overflow
            Array.Clear(visited_gen, 0, visited_gen.Length);
            current_gen = 1;
        }

        var queue = new Queue<(short x, short y)>();

        visited_gen[source.x, source.y] = current_gen;
        distance[source.x, source.y]    = 0;
        father[source.x, source.y]      = (-1, -1);

        // caso borde: ya estamos en el destino
        if (condition(source, game_map))
            return (father, source);

        queue.Enqueue(source);

        while (queue.Count > 0)
        {
            var u = queue.Dequeue();

            if (distance[u.x, u.y] >= maxDistance)
                continue;

            foreach (var (dx, dy) in dirs8)
            {
                short nx = (short)(u.x + dx);
                short ny = (short)(u.y + dy);

                if (nx < 0 || ny < 0 || nx >= Config.WIDTH || ny >= Config.HEIGHT)
                    continue;

                if (aux_map[nx, ny].blocks_movement > 0)
                    continue;

                if (visited_gen[nx, ny] == current_gen)
                    continue;

                visited_gen[nx, ny] = current_gen;
                distance[nx, ny]    = (short)(distance[u.x, u.y] + 1);
                father[nx, ny]      = u;

                // chequear condicion al encolar: si el destino esta bloqueado
                // lo detectamos aca en vez de explorar todo el mapa
                if (condition((nx, ny), game_map))
                {
                    queue.Clear();
                    return (father, (nx, ny));
                }

                queue.Enqueue((nx, ny));
            }
        }

        return (father, (-1,-1));
    }

    // BFS CARGA UNA LISTA DE ENTIDADES QUE CUMPLEN CONDITION
    public static List<int> BFS_Detect(
    List<int>[,] game_map,
    (int blocks_movement, int blocks_vision)[,] aux_map,
    (short x, short y) source,
    Func<int, bool> should_detect,   // recibe un id, devuelve si lo agrego
    short maxDistance)
    {
        current_gen++;
        if (current_gen == short.MaxValue) { Array.Clear(visited_gen, 0, visited_gen.Length); current_gen = 1; }

        var results = new List<int>();
        var queue = new Queue<(short x, short y)>();

        visited_gen[source.x, source.y] = current_gen;
        distance[source.x, source.y] = 0;
        queue.Enqueue(source);

        while (queue.Count > 0)
        {
            var u = queue.Dequeue();
            if (distance[u.x, u.y] >= maxDistance) continue;

            foreach (var (dx, dy) in dirs8)
            {
                short nx = (short)(u.x + dx);
                short ny = (short)(u.y + dy);

                if (nx < 0 || ny < 0 || nx >= Config.WIDTH || ny >= Config.HEIGHT) continue; // me fui de pantlalla
                if (visited_gen[nx, ny] == current_gen) continue;   // ya visite
                if (aux_map[nx, ny].blocks_vision > 0) continue;    // vision cortada

                visited_gen[nx, ny] = current_gen;
                distance[nx, ny] = (short)(distance[u.x, u.y] + 1);

                foreach (int id in game_map[nx, ny])
                    if (should_detect(id))
                        results.Add(id);

                queue.Enqueue((nx, ny));
            }
        }

        return results;
    }

    public static List<(short, short)> GetPath(
        (short, short)[,] father,
        (short, short) start,
        (short, short) target)
    {
        // caso borde: ya estamos en el destino
        if (start == target)
            return new List<(short, short)> { start };

        if (father[target.Item1, target.Item2] == (-1, -1))
            return [];   // destino inalcanzable

        var path = new List<(short, short)>();
        var current = target;

        while (current != start)
        {
            path.Add(current);
            current = father[current.Item1, current.Item2];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }
}