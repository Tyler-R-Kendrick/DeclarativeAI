namespace Declarative.AI.Abstractions;

public class BindingContext(IDictionary<object, object?>? contextData = null)
{
    public IDictionary<object, object?>? ContextData { get; }
        = contextData ?? new Dictionary<object, object?>();
}
