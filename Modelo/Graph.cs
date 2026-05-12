namespace CampusNet.Modelo; // Representa el grafo social de CampusNet, donde cada vértice es un usuario y cada arista dirigida representa una relación de seguimiento entre usuarios. El grafo se implementa utilizando un diccionario de vértices y un diccionario de listas de adyacencia para representar las relaciones dirigidas entre los usuarios. El grafo proporciona métodos para agregar, eliminar y actualizar usuarios y relaciones, así como para realizar recorridos y consultas sociales.

// Representa el grafo social de CampusNet, donde cada vértice es un usuario y cada arista dirigida representa una relación de seguimiento entre usuarios. El grafo se implementa utilizando un diccionario de vértices y un diccionario de listas de adyacencia para representar las relaciones dirigidas entre los usuarios. El grafo proporciona métodos para agregar, eliminar y actualizar usuarios y relaciones, así como para realizar recorridos y consultas sociales.
public sealed class Graph
{
    // Diccionario que mapea cada Id de usuario a su objeto Vertex asociado en el grafo social. El diccionario se inicializa con un comparador de cadenas que ignora mayúsculas y minúsculas para garantizar que los Ids se manejen de manera consistente sin importar su formato.
    private readonly Dictionary<string, Vertex> vertices = new(StringComparer.OrdinalIgnoreCase);
    
    // Diccionario que representa la lista de adyacencia del grafo social, donde cada clave es el Id de un usuario y el valor es una lista de Ids de los usuarios a los que sigue. El diccionario se inicializa con un comparador de cadenas que ignora mayúsculas y minúsculas para garantizar que los Ids se manejen de manera consistente sin importar su formato.
    private readonly Dictionary<string, List<string>> adjacencyList = new(StringComparer.OrdinalIgnoreCase);

    // Retorna una representación de solo lectura del diccionario de vértices del grafo social, donde cada clave es el Id de un usuario y el valor es el objeto Vertex asociado a ese Id. El diccionario de vértices se construye a partir del diccionario interno "vertices", convirtiendo cada valor en un objeto de solo lectura para garantizar la inmutabilidad desde el exterior.
    public IReadOnlyDictionary<string, Vertex> Vertices => vertices;

    // Retorna una representación de solo lectura de la lista de adyacencia del grafo social, donde cada clave es el Id de un usuario y el valor es una lista de Ids de los usuarios a los que sigue. La lista de adyacencia se construye a partir del diccionario interno "adjacencyList", convirtiendo cada lista de vecinos en una lista de solo lectura para garantizar la inmutabilidad desde el exterior.
    public IReadOnlyDictionary<string, IReadOnlyList<string>> AdjacencyList =>
        adjacencyList.ToDictionary(
            item => item.Key,
            item => (IReadOnlyList<string>)item.Value.AsReadOnly(),
            StringComparer.OrdinalIgnoreCase);

    // Retorna el número total de usuarios (vértices) en el grafo social, calculado como la cantidad de entradas en el diccionario de vértices.
    public int VertexCount => vertices.Count;

    // Retorna el número total de relaciones dirigidas en el grafo social, calculado sumando la cantidad de vecinos (seguidos) de cada usuario en la lista de adyacencia.
    public int EdgeCount => adjacencyList.Values.Sum(neighbors => neighbors.Count);

    // Agrega un nuevo usuario al grafo social. Retorna true si el usuario se agrego exitosamente, o false si ya existia un usuario con el mismo Id en el grafo social. El Id del usuario se normaliza eliminando espacios al inicio o final, y validando que no sea nulo, vacio o solo espacios.
    public bool AddVertex(Vertex vertex)
    {
        ArgumentNullException.ThrowIfNull(vertex);

        if (vertices.ContainsKey(vertex.Id))
        {
            return false;
        }

        vertices.Add(vertex.Id, vertex);
        adjacencyList.Add(vertex.Id, new List<string>());
        return true;
    }

    // Elimina un usuario del grafo social, junto con todas las relaciones dirigidas que lo involucran. Retorna true si el usuario existia y se elimino exitosamente, o false si no existia un usuario con ese Id en el grafo social. El Id se normaliza eliminando espacios al inicio o final, y validando que no sea nulo, vacio o solo espacios.
    public bool RemoveVertex(string id)
    {
        string normalizedId = NormalizeId(id);

        if (!vertices.Remove(normalizedId))
        {
            return false;
        }

        adjacencyList.Remove(normalizedId);

        foreach (List<string> neighbors in adjacencyList.Values)
        {
            neighbors.RemoveAll(neighbor => neighbor.Equals(normalizedId, StringComparison.OrdinalIgnoreCase));
        }

        return true;
    }

    // Actualiza el nombre y/o rol del usuario con el Id dado en el grafo social. Retorna true si el usuario existia y se actualizo exitosamente, o false si no existia un usuario con ese Id en el grafo social. El Id se normaliza eliminando espacios al inicio o final, y validando que no sea nulo, vacio o solo espacios. El nombre y rol se actualizan solo si los parametros "name" y "role" son distintos de null, respectivamente.
    public bool UpdateVertex(string id, string? name = null, string? role = null)
    {
        string normalizedId = NormalizeId(id);

        if (!vertices.TryGetValue(normalizedId, out Vertex? vertex))
        {
            return false;
        }

        vertex.Update(name, role);
        return true;
    }

    // Agrega una relacion dirigida entre dos usuarios existentes en el grafo social. Retorna true si la relacion se agrego exitosamente, o false si no se pudo agregar porque alguno de los usuarios no existe, o porque ya existia una relacion dirigida entre esos usuarios. Ambos Ids se normalizan eliminando espacios al inicio o final, y validando que no sean nulos, vacios o solo espacios.
    public bool AddEdge(Edge edge)
    {
        ArgumentNullException.ThrowIfNull(edge);

        if (!vertices.ContainsKey(edge.FromId) || !vertices.ContainsKey(edge.ToId))
        {
            return false;
        }

        List<string> neighbors = adjacencyList[edge.FromId];

        if (neighbors.Any(neighbor => neighbor.Equals(edge.ToId, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        neighbors.Add(edge.ToId);
        return true;
    }

    // Elimina la relacion dirigida entre los usuarios con Id "fromId" y "toId" en el grafo social. Retorna true si la relacion existia y se elimino exitosamente, o false si no existia una relacion dirigida entre esos usuarios. Ambos Ids se normalizan eliminando espacios al inicio o final, y validando que no sean nulos, vacios o solo espacios.
    public bool RemoveEdge(string fromId, string toId)
    {
        string normalizedFromId = NormalizeId(fromId);
        string normalizedToId = NormalizeId(toId);

        if (!adjacencyList.TryGetValue(normalizedFromId, out List<string>? neighbors))
        {
            return false;
        }

        int removed = neighbors.RemoveAll(neighbor => neighbor.Equals(normalizedToId, StringComparison.OrdinalIgnoreCase));
        return removed > 0;
    }

    // Retorna una lista de los usuarios sin seguidores en el grafo social, es decir, aquellos con grado de entrada (cantidad de seguidores) igual a cero. La lista se ordena por Id de usuario en orden alfabético.
    public IReadOnlyList<string> BreadthFirstSearch(string startId)
    {
        string normalizedStartId = NormalizeId(startId);

        if (!vertices.ContainsKey(normalizedStartId))
        {
            return Array.Empty<string>();
        }

        List<string> visitOrder = new();
        HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase);
        Queue<string> pending = new();

        visited.Add(normalizedStartId);
        pending.Enqueue(normalizedStartId);

        while (pending.Count > 0)
        {
            string currentId = pending.Dequeue();
            visitOrder.Add(currentId);

            foreach (string neighborId in adjacencyList[currentId])
            {
                if (visited.Add(neighborId))
                {
                    pending.Enqueue(neighborId);
                }
            }
        }

        return visitOrder;
    }

    // Realiza un recorrido en profundidad (DFS) completo del grafo social, visitando todos los vértices y detectando ciclos. Retorna un objeto DfsResult que incluye el orden de descubrimiento de los vértices y las ciclos encontrados durante el recorrido.
    public DfsResult DepthFirstSearch()
    {
        Dictionary<string, VisitState> states = vertices.Keys.ToDictionary(
            id => id,
            _ => VisitState.NotVisited,
            StringComparer.OrdinalIgnoreCase);
        Dictionary<string, string?> parents = vertices.Keys.ToDictionary(
            id => id,
            _ => (string?)null,
            StringComparer.OrdinalIgnoreCase);
        List<string> discoveryOrder = new();
        List<IReadOnlyList<string>> cycles = new();
        HashSet<string> cycleKeys = new(StringComparer.OrdinalIgnoreCase);

        foreach (string vertexId in vertices.Keys)
        {
            if (states[vertexId] == VisitState.NotVisited)
            {
                Visit(vertexId, states, parents, discoveryOrder, cycles, cycleKeys);
            }
        }

        return new DfsResult(discoveryOrder, cycles);
    }

    // Retorna una lista de los usuarios sin seguidores en el grafo social, es decir, aquellos con grado de entrada (cantidad de seguidores) igual a cero. La lista se ordena por Id de usuario en orden alfabético.
    public IReadOnlyList<Vertex> GetUsersWithoutFollowers()
    {
        Dictionary<string, int> inDegrees = GetInDegrees();

        return vertices.Values
            .Where(vertex => inDegrees[vertex.Id] == 0)
            .OrderBy(vertex => vertex.Id, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    // Retorna una lista de los usuarios mas influyentes en el grafo social, es decir, aquellos con mayor grado de entrada (cantidad de seguidores). Si hay varios usuarios empatados con el mismo grado de entrada mas alto, se incluyen todos ellos en la lista. La lista se ordena por Id de usuario en orden alfabético.
    public IReadOnlyList<UserDegree> GetMostInfluentialUsers()
    {
        Dictionary<string, int> inDegrees = GetInDegrees();
        int highestInDegree = inDegrees.Count == 0 ? 0 : inDegrees.Values.Max();

        return inDegrees
            .Where(item => item.Value == highestInDegree)
            .Select(item => new UserDegree(vertices[item.Key], item.Value))
            .OrderBy(item => item.User.Id, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    // Retorna una lista de los usuarios mas activos en el grafo social, es decir, aquellos con mayor grado de salida (cantidad de seguidos). Si hay varios usuarios empatados con el mismo grado de salida mas alto, se incluyen todos ellos en la lista. La lista se ordena por Id de usuario en orden alfabético.
    public IReadOnlyList<UserDegree> GetMostActiveUsers()
    {
        int highestOutDegree = adjacencyList.Count == 0 ? 0 : adjacencyList.Values.Max(neighbors => neighbors.Count);

        return adjacencyList
            .Where(item => item.Value.Count == highestOutDegree)
            .Select(item => new UserDegree(vertices[item.Key], item.Value.Count))
            .OrderBy(item => item.User.Id, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    //  Retorna true si existe un camino de relaciones dirigidas desde el usuario con Id "fromId" hasta el usuario con Id "toId" en el grafo social, o false si no existe tal camino. Ambos Ids se normalizan eliminando espacios al inicio o final, y validando que no sean nulos, vacios o solo espacios.
    public bool CanReach(string fromId, string toId)
    {
        string normalizedFromId = NormalizeId(fromId);
        string normalizedToId = NormalizeId(toId);

        if (!vertices.ContainsKey(normalizedFromId) || !vertices.ContainsKey(normalizedToId))
        {
            return false;
        }

        return BreadthFirstSearch(normalizedFromId)
            .Any(id => id.Equals(normalizedToId, StringComparison.OrdinalIgnoreCase));
    }

    // Retorna el grado de entrada (cantidad de seguidores) del usuario con el Id dado en el grafo social. El Id se normaliza eliminando espacios al inicio o final, y validando que no sea nulo, vacio o solo espacios.
    public int GetInDegree(string id)
    {
        string normalizedId = NormalizeId(id);
        Dictionary<string, int> inDegrees = GetInDegrees();

        return inDegrees.TryGetValue(normalizedId, out int degree) ? degree : 0;
    }

    // Retorna el grado de salida (cantidad de seguidos) del usuario con el Id dado en el grafo social. El Id se normaliza eliminando espacios al inicio o final, y validando que no sea nulo, vacio o solo espacios.
    public int GetOutDegree(string id)
    {
        string normalizedId = NormalizeId(id);

        return adjacencyList.TryGetValue(normalizedId, out List<string>? neighbors) ? neighbors.Count : 0;
    }
    
    // Retorna el vértice asociado al Id dado, o null si no existe un vértice con ese Id en el grafo social. El Id se normaliza eliminando espacios al inicio o final, y validando que no sea nulo, vacio o solo espacios.
    public Vertex? FindVertex(string id)
    {
        string normalizedId = NormalizeId(id);
        return vertices.GetValueOrDefault(normalizedId);
    }

    // Retorna un diccionario que mapea cada Id de usuario a su grado de entrada (cantidad de seguidores) en el grafo social. El grado de entrada se calcula contando cuántas relaciones dirigidas llegan a cada usuario desde otros usuarios.
    private Dictionary<string, int> GetInDegrees()
    {
        Dictionary<string, int> inDegrees = vertices.Keys.ToDictionary(
            id => id,
            _ => 0,
            StringComparer.OrdinalIgnoreCase);

        foreach (List<string> neighbors in adjacencyList.Values)
        {
            foreach (string neighborId in neighbors)
            {
                inDegrees[neighborId]++;
            }
        }

        return inDegrees;
    }

    // Realiza una visita recursiva en profundidad (DFS) desde el vértice actual, actualizando los estados de visita, el diccionario de padres, el orden de descubrimiento y los ciclos encontrados. El parámetro "cycleKeys" se utiliza para evitar agregar ciclos duplicados al resultado.
    private void Visit(
        string currentId,
        IDictionary<string, VisitState> states,
        IDictionary<string, string?> parents,
        ICollection<string> discoveryOrder,
        ICollection<IReadOnlyList<string>> cycles,
        ISet<string> cycleKeys)
    {
        states[currentId] = VisitState.Visiting;
        discoveryOrder.Add(currentId);

        foreach (string neighborId in adjacencyList[currentId])
        {
            if (states[neighborId] == VisitState.NotVisited)
            {
                parents[neighborId] = currentId;
                Visit(neighborId, states, parents, discoveryOrder, cycles, cycleKeys);
            }
            else if (states[neighborId] == VisitState.Visiting)
            {
                IReadOnlyList<string> cycle = BuildCycle(currentId, neighborId, parents);
                string cycleKey = string.Join("->", cycle);

                if (cycleKeys.Add(cycleKey))
                {
                    cycles.Add(cycle);
                }
            }
        }

        states[currentId] = VisitState.Visited;
    }

    // Construye un ciclo detectado durante el recorrido en profundidad (DFS) a partir del vértice actual, el vértice repetido y el diccionario de padres. Retorna el ciclo como una lista de Ids de vértices en orden desde el vértice repetido hasta el vértice actual, y luego de regreso al vértice repetido.
    private static IReadOnlyList<string> BuildCycle(
        string currentId,
        string repeatedId,
        IDictionary<string, string?> parents)
    {
        List<string> reversedCycle = new() { currentId };
        string? walker = parents[currentId];

        while (walker is not null && !walker.Equals(repeatedId, StringComparison.OrdinalIgnoreCase))
        {
            reversedCycle.Add(walker);
            walker = parents[walker];
        }

        if (walker is not null)
        {
            reversedCycle.Add(walker);
        }

        reversedCycle.Reverse();
        reversedCycle.Add(repeatedId);
        return reversedCycle;
    }

    // Normaliza un Id eliminando espacios al inicio o final, y validando que no sea nulo, vacio o solo espacios. Retorna el Id normalizado.
    private static string NormalizeId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("El Id no puede estar vacio.", nameof(id));
        }

        return id.Trim();
    }

    // Representa el estado de visita de un vértice durante el recorrido en profundidad (DFS).
    private enum VisitState
    {
        NotVisited,
        Visiting,
        Visited
    }
}

// Representa el resultado del recorrido en profundidad (DFS) en el grafo social, incluyendo el orden de descubrimiento de los vertices y las ciclos encontrados durante el recorrido.
public sealed record DfsResult(IReadOnlyList<string> DiscoveryOrder, IReadOnlyList<IReadOnlyList<string>> Cycles);

// Representa un usuario y su grado de entrada o salida en el grafo social, utilizado para consultas sociales como usuarios sin seguidores, usuarios influyentes y usuarios mas activos.
public sealed record UserDegree(Vertex User, int Degree); 
