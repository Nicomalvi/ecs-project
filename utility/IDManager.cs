public static class IDManager
// static = viven en esta clase nomas, no existen instancias de otro IDManager
{
    static int max_id = 0;
    static Queue<int> free_ids = new Queue<int>();

    public static int get_id()
    {
        if (free_ids.Count == 0)
            return max_id++;
        return free_ids.Dequeue();
    }

    public static void destroy(int id, ref uint component_mask) // pasar ref esta piola
    {
        component_mask = 0;
        free_ids.Enqueue(id);
    }
}