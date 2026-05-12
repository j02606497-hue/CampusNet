using CampusNet.Modelo; // Importa el espacio de nombres que contiene las clases del modelo, como Graph, Vertex y Edge, para su uso en la vista, que se encarga de mostrar la información en la consola.

// Importa el espacio de nombres que contiene las clases del modelo, como Graph, Vertex y Edge, para su uso en la vista, que se encarga de mostrar la información en la consola.
namespace CampusNet.Vista;

// Clase responsable de mostrar la información en la consola, incluyendo la representación del grafo social, los resultados de los recorridos, las consultas sociales y las operaciones CRUD, proporcionando una interfaz de usuario clara y legible para interactuar con el programa.
public sealed class ConsoleView
{  
    // Imprime un encabezado principal para la consola, proporcionando una introducción clara al programa y separando visualmente las diferentes secciones del programa con líneas de igual. El título se muestra en mayúsculas para resaltar su importancia.
    public void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 78));
        Console.WriteLine(title.ToUpperInvariant());
        Console.WriteLine(new string('=', 78));
    }
    
    // Imprime un subencabezado para secciones específicas dentro de la consola, proporcionando una separación visual clara entre diferentes partes del programa, como resultados de recorridos, consultas sociales o operaciones CRUD.
    public void PrintSubHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"--- {title} ---");
    }

    // Imprime la representación de la lista de adyacencia del grafo social, mostrando cada usuario con su nombre, rol y la lista de usuarios a los que sigue. Si un usuario no sigue a nadie, se indica que no tiene seguidos.
    public void PrintGraph(Graph graph)
    {
        PrintSubHeader($"Lista de adyacencia: {graph.VertexCount} usuarios, {graph.EdgeCount} relaciones");
        IReadOnlyDictionary<string, IReadOnlyList<string>> adjacencyList = graph.AdjacencyList;

        foreach (Vertex vertex in graph.Vertices.Values.OrderBy(vertex => vertex.Id, StringComparer.OrdinalIgnoreCase))
        {
            IReadOnlyList<string> neighbors = adjacencyList[vertex.Id];
            string formattedNeighbors = neighbors.Count == 0
                ? "sin seguidos"
                : string.Join(", ", neighbors);

            Console.WriteLine($"{vertex.Id} [{vertex.Name}, {vertex.Role}] -> {formattedNeighbors}");
        }
    }

    // Imprime el resultado de un recorrido BFS completo, mostrando el orden de visita de los vértices alcanzados desde un vértice inicial. Si no se alcanzan vértices, se indica que no hay vértices alcanzados. Si se alcanzan vértices, se muestra el orden de visita y la cantidad total de vértices alcanzados.
    public void PrintBfsResult(string startId, IReadOnlyList<string> visitOrder)
    {
        PrintSubHeader($"BFS desde {startId}");
        Console.WriteLine($"Orden de visita: {FormatList(visitOrder)}");
        Console.WriteLine($"Vertices alcanzados: {visitOrder.Count}");
    }

    // Imprime el resultado de un recorrido DFS completo, mostrando el orden de descubrimiento de los vértices y los ciclos detectados en el grafo social. Si no se detectan ciclos, se indica que no hay ciclos. Si se detectan ciclos, se muestra cada ciclo como una ruta legible de Ids de usuarios.
    public void PrintDfsResult(DfsResult result)
    {
        PrintSubHeader("DFS completo");
        Console.WriteLine($"Orden de descubrimiento: {FormatList(result.DiscoveryOrder)}");

        if (result.Cycles.Count == 0)
        {
            Console.WriteLine("Ciclos detectados: ninguno");
            return;
        }

        Console.WriteLine("Ciclos detectados:");

        foreach (IReadOnlyList<string> cycle in result.Cycles)
        {
            Console.WriteLine($"  {FormatPath(cycle)}");
        }
    }

    // Imprime una lista de usuarios con un título descriptivo. Si la lista de usuarios está vacía, se muestra un mensaje indicando que no hay usuarios para mostrar.
    public void PrintUsers(string title, IReadOnlyList<Vertex> users)
    {
        PrintSubHeader(title);

        if (users.Count == 0)
        {
            Console.WriteLine("No hay usuarios para mostrar.");
            return;
        }

        foreach (Vertex user in users)
        {
            Console.WriteLine(user);
        }
    }

    // Imprime el resultado de una consulta de usuarios con un grado específico (por ejemplo, usuarios sin seguidores o usuarios más influyentes), mostrando el usuario y su grado correspondiente.
    public void PrintUserDegrees(string title, IReadOnlyList<UserDegree> users)
    {
        PrintSubHeader(title);

        if (users.Count == 0)
        {
            Console.WriteLine("No hay usuarios para mostrar.");
            return;
        }

        foreach (UserDegree userDegree in users)
        {
            Console.WriteLine($"{userDegree.User} - grado: {userDegree.Degree}");
        }
    }

    // Imprime el resultado de una consulta de alcanzabilidad entre dos usuarios, indicando si el usuario "fromId" puede alcanzar al usuario "toId" mediante un recorrido BFS en el grafo social.
    public void PrintReachability(string fromId, string toId, bool canReach)
    {
        PrintSubHeader($"Alcanzabilidad {fromId} -> {toId}");
        Console.WriteLine(canReach
            ? $"{fromId} puede llegar a {toId} mediante BFS."
            : $"{fromId} no puede llegar a {toId} mediante BFS.");
    }

    // Imprime el resultado de una operación CRUD, indicando si la operación fue exitosa o no. Si la operación no fue exitosa, se imprime un mensaje de error indicando que la operación no se aplicó.
    public void PrintOperationResult(string operation, bool successful)
    {
        if (!successful)
        {      
            Console.WriteLine($"\n[ERROR] No Aplicada: {operation}");
        }
        else
        {
            Console.WriteLine($"\n{operation}");
        }
    }
    
    // Imprime un mensaje simple en la consola.
    public void PrintMessage(string message)
    {
        Console.WriteLine(message);
    }

    // Formatea una lista de Ids de usuarios como una lista legible, por ejemplo: "U01 -> U02 -> U03". Si la lista está vacía, retorna "(vacio)".
    private static string FormatList(IReadOnlyList<string> items)
    {
        return items.Count == 0 ? "(vacio)" : string.Join(" -> ", items);
    }

    // Formatea una lista de Ids de usuarios como una ruta legible, por ejemplo: "U01 -> U02 -> U03". Si la lista está vacía, retorna "(vacio)".
    private static string FormatPath(IReadOnlyList<string> items)
    {
        return items.Count == 0 ? "(vacio)" : string.Join(" -> ", items);
    }
}
