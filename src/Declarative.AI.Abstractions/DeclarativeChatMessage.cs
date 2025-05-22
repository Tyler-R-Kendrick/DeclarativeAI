using Microsoft.Extensions.AI;

namespace Declarative.AI.Abstractions;

// Declarative ChatMessage abstraction
public class DeclarativeChatMessage(
    ChatRole role,
    IList<AIContent> contents,
    Dictionary<string, ParameterDeclaration> parameters)
    : ChatMessage(role, contents)
{
    public Dictionary<string, ParameterDeclaration> Parameters { get; } = parameters;
}
