namespace CampusNet.Modelo; // Representa una relacion dirigida entre dos usuarios en el grafo social.

// Representa una relacion dirigida entre dos usuarios en el grafo social. Cada relacion conecta un usuario "FromId" con otro usuario "ToId". El Id de cada usuario no puede ser nulo, vacio o solo espacios. Ademas, ambos Ids deben ser diferentes.
public sealed class Edge
{
    // Crea una nueva relacion dirigida entre dos usuarios. El Id de cada usuario no puede ser nulo, vacio o solo espacios. Ademas, ambos Ids deben ser diferentes.
    public Edge(string fromId, string toId)
    {
        FromId = ValidateRequired(fromId, nameof(fromId));
        ToId = ValidateRequired(toId, nameof(toId));

        if (FromId.Equals(ToId, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Una relacion dirigida no puede conectar un usuario consigo mismo.");
        }
    }

    // El Id del usuario que inicia la relacion. 
    public string FromId { get; }

    // El Id del usuario al que se dirige la relacion.
    public string ToId { get; }

    // Retorna una representacion legible de la relacion dirigida, por ejemplo: "U01 -> U02".
    public override string ToString()
    {
        return $"{FromId} -> {ToId}";
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
}
