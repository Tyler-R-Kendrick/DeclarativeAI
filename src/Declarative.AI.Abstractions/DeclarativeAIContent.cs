using Microsoft.Extensions.AI;

namespace Declarative.AI.Abstractions;

// Declarative AIContent with parameters
public class DeclarativeAIContent(Dictionary<string, ParameterDeclaration> parameters) : AIContent
{
    public Dictionary<string, ParameterDeclaration> Parameters { get; } = parameters;
}
