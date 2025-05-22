namespace Declarative.AI.Abstractions;

// Represents a parameter declaration with metadata
public record ParameterDeclaration
{
    public string Name { get; init; }
    public Type? Type { get; init; }
    public string Description { get; init; }
    public bool? Required { get; init; }
    public Dictionary<string, object> Metadata { get; init; }

    public ParameterDeclaration(
        string name,
        string description,
        Type? type = null,
        bool required = false,
        Dictionary<string, object>? metadata = null)
    {
        Name = name;
        Type = type ?? typeof(string);
        Description = description;
        Required = required;
        Metadata = metadata ?? [];
        Metadata[nameof(Name)] = Name;
        Metadata[nameof(Type)] = Type;
        Metadata[nameof(Description)] = Description;
        Metadata[nameof(Required)] = Required;
    }
}
