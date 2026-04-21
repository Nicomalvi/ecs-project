public static class RenderSystem
// parece ser de lo que más gasta memoria?
// FUTURA OPTIMIZACION ==========================================
// En vez de calcular que hay en cada celda pasando por todo item, puedo armar un mapa de que imprimir
// se actualiza gracias a map utils
{
    public static void Run(World w)
    {
        Console.SetCursorPosition(0, 0);
        var output = new System.Text.StringBuilder();
        // imprimo la primer entidad con ia y ascii que encuentre. sino, la primera con ascii. sino, el piso.
        for (int y = 0; y < Config.HEIGHT; y++)
        {
            for (int x = 0; x < Config.WIDTH; x++)
            {
                char current_cell = '_';
                bool found_ai = false;    // se resetean por celda
                bool found_ascii = false;
                for (int i = 0; i < w.game_map[x, y].Count; i++)
                {
                    int id = w.game_map[x, y][i];
                    if (id == w.player && w.ascii.Has(id))  // 1: jugador, prioridad maxima
                    {
                        current_cell = w.ascii.Get(id);
                        break;
                    }
                    if (w.ascii.Has(id) && w.ai_behaviour.Has(id) && !found_ai)  // 2: entidad con IA
                    {
                        found_ai = true;
                        current_cell = w.ascii.Get(id);
                        // no break: puede haber el jugador mas adelante
                    }
                    if (w.ascii.Has(id) && !w.ai_behaviour.Has(id) && !found_ascii && id != w.player)  // 3: ascii solo
                    {
                        found_ascii = true;
                        if (!found_ai) current_cell = w.ascii.Get(id);
                        // no break: puede haber IA o jugador mas adelante
                    }
                }
                output.Append(current_cell);
            }
        output.AppendLine();
        }

        // mensajes
        if (w.announcement_list.Count > Config.MESSAGE_LINES)
        {
            int excess = w.announcement_list.Count - Config.MESSAGE_LINES;
            w.announcement_list.RemoveRange(0, excess);
            //hago rotacion: si no hay lugar para mensajes, saco los mas viejos
        }
        output.AppendLine();
        for (int i = 0; i < Config.MESSAGE_LINES; i++)
                {
        string line = i < w.announcement_list.Count ? w.announcement_list[i] : "";
        output.AppendLine(line.PadRight(Config.WIDTH));
                }

        // stats
        output.AppendLine();
        if (w.player != -1)
        output.AppendLine($"Tick: {w.tick}  HP: {w.health.Get(w.player).current}        ");
        else
        output.AppendLine("PLAYER DIED :(:()                                         ");

        Console.Write(output.ToString());
    }
}