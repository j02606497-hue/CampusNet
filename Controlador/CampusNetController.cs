using CampusNet.Modelo; // Importa el espacio de nombres que contiene las clases del modelo, como Graph, Vertex y Edge, para su uso en el controlador.
using CampusNet.Vista; // Importa el espacio de nombres que contiene la clase ConsoleView, que se encarga de mostrar la información en la consola, para su uso en el controlador.

// Controlador principal que gestiona la interacción entre el modelo (grafo) y la vista (consola), ejecutando los casos de uso definidos en el taller integrador.
namespace CampusNet.Controlador;

// Controlador principal que gestiona la interacción entre el modelo (grafo) y la vista (consola), ejecutando los casos de uso definidos en el taller integrador.
public sealed class CampusNetController
{
    // Campos para almacenar el grafo, la vista y los resultados de las operaciones CRUD, permitiendo su uso a lo largo del controlador.
    private readonly Graph graph;
    private readonly ConsoleView view;
    private bool crudUserAdded;
    private bool crudRelationAdded;
    private bool crudUserUpdated;
    private bool crudRelationRemoved;
    private bool crudUserRemoved;

    // Constructor privado para asegurar que la instancia del controlador se cree a través del método Run, garantizando la inicialización adecuada del grafo y la vista.
    private CampusNetController(Graph graph, ConsoleView view)
    {
        this.graph = graph;
        this.view = view;
    }

    // Punto de entrada principal del programa, creando una instancia del controlador y ejecutando el flujo principal.
    public static void Run()
    {
        CampusNetController controller = new(new Graph(), new ConsoleView());
        controller.Execute();
    }

    // Ejecuta el flujo principal del programa, incluyendo la construcción del grafo, la ejecución de recorridos, consultas sociales y operaciones CRUD, mostrando los resultados en la consola.
    private void Execute()
    {
        view.PrintHeader("CampusNet - Taller Integrador: Grafos Dirigidos MVC");

        BuildGraph();
        view.PrintGraph(graph);

        ExecuteTraversals();
        ExecuteSocialQueries();
        ExecuteCrudOperations();
    }

    // Construye el grafo inicial con usuarios, roles y relaciones dirigidas, mostrando un mensaje al finalizar la construccion.
    private void BuildGraph()
    {
        view.PrintHeader("Caso de uso 1: Construccion del grafo");

        AddUser("U01", "Ana Torres", "estudiante");
        AddUser("U02", "Bruno Salazar", "profesor");
        AddUser("U03", "Camila Rojas", "egresado");
        AddUser("U04", "Daniel Mejia", "estudiante");
        AddUser("U05", "Elena Castro", "profesor");
        AddUser("U06", "Felipe Gomez", "estudiante");
        AddUser("U07", "Gabriela Marin", "egresado");
        AddUser("U08", "Hector Vargas", "profesor");
        AddUser("U09", "Isabella Ruiz", "estudiante");
        AddUser("U10", "Julian Perez", "egresado");
        AddUser("U11", "Laura Nieto", "estudiante");
        AddUser("U12", "Mateo Cardenas", "profesor");

        AddFollow("U01", "U02");
        AddFollow("U01", "U03");
        AddFollow("U01", "U04");
        AddFollow("U01", "U05");
        AddFollow("U02", "U03");
        AddFollow("U02", "U05");
        AddFollow("U03", "U04");
        AddFollow("U03", "U07");
        AddFollow("U04", "U02");
        AddFollow("U04", "U08");
        AddFollow("U05", "U06");
        AddFollow("U05", "U07");
        AddFollow("U06", "U01");
        AddFollow("U06", "U07");
        AddFollow("U06", "U08");
        AddFollow("U06", "U09");
        AddFollow("U07", "U09");
        AddFollow("U08", "U10");
        AddFollow("U09", "U10");
        AddFollow("U10", "U05");
        AddFollow("U11", "U02");
        AddFollow("U12", "U03");

        view.PrintMessage("Grafo dirigido inicial creado con usuarios, roles y relaciones sin duplicados.");
    }

    // Ejecuta recorridos del grafo, mostrando los resultados de cada recorrido.
    private void ExecuteTraversals()
    {
        view.PrintHeader("Caso de uso 2: Recorridos");

        view.PrintBfsResult("U01", graph.BreadthFirstSearch("U01"));
        view.PrintBfsResult("U06", graph.BreadthFirstSearch("U06"));
        view.PrintBfsResult("U11", graph.BreadthFirstSearch("U11"));
        view.PrintDfsResult(graph.DepthFirstSearch());
    }

    // Ejecuta consultas sociales como usuarios sin seguidores, usuarios mas influyentes, usuarios mas activos y alcance entre usuarios, mostrando los resultados de cada consulta.
    private void ExecuteSocialQueries()
    {
        view.PrintHeader("Caso de uso 3: Consultas sociales");

        view.PrintUsers("Usuarios sin seguidores (grado de entrada 0)", graph.GetUsersWithoutFollowers());
        view.PrintUserDegrees("Usuarios influyentes (mayor grado de entrada)", graph.GetMostInfluentialUsers());
        view.PrintUserDegrees("Usuarios mas activos (mayor grado de salida)", graph.GetMostActiveUsers());
        view.PrintReachability("U11", "U10", graph.CanReach("U11", "U10"));
        view.PrintReachability("U12", "U11", graph.CanReach("U12", "U11"));
    }

    // Ejecuta operaciones de creación, actualización y eliminación de usuarios y relaciones, mostrando el resultado de cada operación.
    private void ExecuteCrudOperations()
    {
        view.PrintHeader("Caso de uso 4: Operaciones CRUD");

        crudUserAdded = graph.AddVertex(new Vertex("U13", "Nicolas Arias", "estudiante"));
        view.PrintOperationResult("Agregar usuario U13 - Nicolas Arias", crudUserAdded);
        view.PrintGraph(graph);

        crudRelationAdded = graph.AddEdge(new Edge("U13", "U05"));
        view.PrintOperationResult("Agregar relacion dirigida U13 -> U05", crudRelationAdded);
        view.PrintGraph(graph);

        crudUserUpdated = graph.UpdateVertex("U13", name: "Nicolas Arias Molina", role: "egresado");
        view.PrintOperationResult("Actualizar usuario U13: nombre y rol", crudUserUpdated);
        view.PrintGraph(graph);

        crudRelationRemoved = graph.RemoveEdge("U02", "U05");
        view.PrintOperationResult("Eliminar relacion dirigida U02 -> U05", crudRelationRemoved);
        view.PrintGraph(graph);

        crudUserRemoved = graph.RemoveVertex("U13");
        view.PrintOperationResult("Eliminar usuario U13 y sus relaciones asociadas", crudUserRemoved);
        view.PrintGraph(graph);
    }

    // Agrega un nuevo usuario al grafo. Lanza excepcion si no se pudo agregar.
    private void AddUser(string id, string name, string role)
    {
        bool added = graph.AddVertex(new Vertex(id, name, role));

        if (!added)
        {
            throw new InvalidOperationException($"No se pudo agregar el usuario requerido: {id}.");
        }
    }

    // Agrega una relacion dirigida entre dos usuarios existentes en el grafo. Lanza excepcion si no se puede agregar.
    private void AddFollow(string fromId, string toId)
    {
        bool added = graph.AddEdge(new Edge(fromId, toId));

        if (!added)
        {
            throw new InvalidOperationException($"No se pudo agregar la relacion requerida: {fromId} -> {toId}.");
        }
    }
}
