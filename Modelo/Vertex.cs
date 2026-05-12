namespace CampusNet.Modelo; // Representa un usuario en el grafo social de CampusNet, con un Id único, un nombre y un rol. El Id del usuario se valida para asegurarse de que no sea nulo, vacio o solo espacios, y se normaliza eliminando espacios al inicio o final para garantizar la consistencia en el almacenamiento y uso del Id en el grafo social. El nombre del usuario se valida para asegurarse de que no sea nulo, vacio o solo espacios, y se normaliza eliminando espacios al inicio o final para garantizar la consistencia en el almacenamiento y uso del nombre en el grafo social. El rol del usuario se valida para asegurarse de que sea uno de los valores permitidos (estudiante, profesor o egresado), ignorando mayúsculas y minúsculas, y se normaliza a minúsculas para garantizar la consistencia en el almacenamiento y uso del rol en el grafo social.

// Representa un usuario en el grafo social de CampusNet, con un Id único, un nombre y un rol. El Id del usuario se valida para asegurarse de que no sea nulo, vacio o solo espacios, y se normaliza eliminando espacios al inicio o final para garantizar la consistencia en el almacenamiento y uso del Id en el grafo social. El nombre del usuario se valida para asegurarse de que no sea nulo, vacio o solo espacios, y se normaliza eliminando espacios al inicio o final para garantizar la consistencia en el almacenamiento y uso del nombre en el grafo social. El rol del usuario se valida para asegurarse de que sea uno de los valores permitidos (estudiante, profesor o egresado), ignorando mayúsculas y minúsculas, y se normaliza a minúsculas para garantizar la consistencia en el almacenamiento y uso del rol en el grafo social.
public sealed class Vertex
{
    // crea un usuario con un id unico que no puede ser nulo, vacio o solo espacios, un nombre que no puede ser nulo, vacio o solo espacios, y un rol que debe ser estudiante, profesor o egresado. El id se normaliza eliminando espacios al inicio o final, el nombre se normaliza eliminando espacios al inicio o final, y el rol se valida e normaliza a minúsculas para garantizar la consistencia en el almacenamiento y uso del usuario en el grafo social.
    public Vertex(string id, string name, string role)
    {
        Id = ValidateRequired(id, nameof(id));
        Name = ValidateRequired(name, nameof(name));
        Role = ValidateRole(role);
    }

    // El Id único del usuario. El Id se valida para asegurarse de que no sea nulo, vacio o solo espacios, y se normaliza eliminando espacios al inicio o final para garantizar la consistencia en el almacenamiento y uso del Id en el grafo social.
    public string Id { get; }

    // El nombre del usuario. El nombre se valida para asegurarse de que no sea nulo, vacio o solo espacios, y se normaliza eliminando espacios al inicio o final para garantizar la consistencia en el almacenamiento y uso del nombre en el grafo social.
    public string Name { get; private set; }

    // El rol del usuario, que puede ser "estudiante", "profesor" o "egresado". El rol se valida para asegurarse de que sea uno de los valores permitidos, ignorando mayúsculas y minúsculas, y se normaliza a minúsculas para garantizar la consistencia en el almacenamiento y uso del rol en el grafo social.
    public string Role { get; private set; }

    // Actualiza el nombre y/o rol del usuario. Si se proporciona un nuevo nombre, se valida que no sea nulo, vacio o solo espacios, y se actualiza el nombre del usuario. Si se proporciona un nuevo rol, se valida que sea uno de los valores permitidos (estudiante, profesor o egresado), ignorando mayúsculas y minúsculas, y se actualiza el rol del usuario. Si no se proporciona un nuevo nombre o rol, el usuario permanece sin cambios.
    public void Update(string? name = null, string? role = null)
    {
        if (name is not null)
        {
            Name = ValidateRequired(name, nameof(name));
        }

        if (role is not null)
        {
            Role = ValidateRole(role);
        }
    }

    // Retorna una representación legible del usuario, por ejemplo: "U01 - Juan Perez (estudiante)".
    public override string ToString()
    {
        return $"{Id} - {Name} ({Role})";
    }

    // Valida que el valor no sea nulo, vacio o solo espacios. Retorna el valor sin espacios al inicio o final.
    private static string ValidateRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("El valor no puede estar vacio.", fieldName);
        }

        return value.Trim();
    }

    // Valida que el rol sea uno de los valores permitidos (estudiante, profesor o egresado), ignorando mayúsculas y minúsculas. Retorna el rol normalizado en minúsculas si es válido, o lanza una excepción si no es válido.
    private static string ValidateRole(string role)
    {
        string normalizedRole = ValidateRequired(role, nameof(role)).ToLowerInvariant();

        return normalizedRole switch
        {
            "estudiante" => "estudiante",
            "profesor" => "profesor",
            "egresado" => "egresado",
            _ => throw new ArgumentException("El rol debe ser estudiante, profesor o egresado.", nameof(role))
        };
    }
}
