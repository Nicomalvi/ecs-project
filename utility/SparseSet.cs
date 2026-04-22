public class SparseSet<T>{
    // INVESTIGAR VIEW, BUILD, REBUILD
    // list vs array
    public int[][] sparse; //usare paginacion
    public List<T> dense;  //cuando itero solo me importa dense, sparse es para acceso de id en O(1)
    public List<int> valid_ids;

    public SparseSet(){
        int pageCount = (Config.MAX_ENTITIES + Config.PAGE_SIZE - 1) / Config.PAGE_SIZE; // recuerdo: redondea para arriba
        sparse = new int[pageCount][];
        dense = new List<T>();
        valid_ids = new List<int>();
    }

    // add = si tiene actualiza, si no tiene agrega
    // set = si tiene actualiza, si no tiene no hagas nada

    public void Add(int id, T data)
    {
        int page = id / Config.PAGE_SIZE;
        int index_in_page = id % Config.PAGE_SIZE;
        
        if (sparse[page] != null && sparse[page][index_in_page] != -1)
        {
            dense[sparse[page][index_in_page]] = data;  // update directo
            return;
        }

        if (sparse[page] == null)
        {
            sparse[page] = new int[Config.PAGE_SIZE];
            Array.Fill(sparse[page], -1); // si cree una pagina, la lleno de -1 pq no esta inicializada
        }
        sparse[page][index_in_page] = dense.Count; // el dato en sparse apunta al proximo lugar libre de la dense
        dense.Add(data);
        valid_ids.Add(id);
    }
    public void Set(int id, T data)
    {
        int page = id / Config.PAGE_SIZE;
        int index_in_page = id % Config.PAGE_SIZE;
        if (Has(id)) //poco optimo?
            dense[sparse[page][index_in_page]] = data;
    }
    public bool Has(int id)
    {
        int page = id / Config.PAGE_SIZE;
        int index_in_page = id % Config.PAGE_SIZE;
        return sparse[page] != null && sparse[page][index_in_page] != -1;
        // por c#, si la primer mitad del && se rompe no leo la 2da, evitando acceder a puntero null
    }
    public T Get(int id)
    {
        int page = id / Config.PAGE_SIZE;
        int index_in_page = id % Config.PAGE_SIZE;
        int denseIndex = sparse[page][index_in_page];
        return dense[denseIndex];
    }
    public void Remove(int id)
    {   // TrimExcess puede ayudar
        if (!Has(id)) return; // si no existe, no hago nada
        int page = id / Config.PAGE_SIZE;
        int index_in_page = id % Config.PAGE_SIZE;
        int denseIndex = sparse[page][index_in_page];

        int lastId = valid_ids[valid_ids.Count - 1];
        int lastPage = lastId / Config.PAGE_SIZE;
        int lastIndexInPage = lastId % Config.PAGE_SIZE;

        // cambio el elemento a borrar por el ultimo en valid_ids y en dense
        dense[denseIndex] = dense[dense.Count - 1];
        valid_ids[denseIndex] = lastId;
        
        sparse[lastPage][lastIndexInPage] = denseIndex; // aca termino de hacer el cambio

        // luego del cambio, el elemento a borrar esta al final en las 2 y borro en las 2
        dense.RemoveAt(dense.Count - 1);
        valid_ids.RemoveAt(valid_ids.Count - 1);
        // marco la pagina de sparse como id no valido
        sparse[page][index_in_page] = -1;
    }
}