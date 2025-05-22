using System.Collections.Immutable;

namespace Declarative.AI.Abstractions;

public class DeclarativePrompt(ChatOptions? options, params ChatMessage[] messages)
    : AITool, IPrompt
{
    public IImmutableDictionary<string, ParameterDeclaration> Parameters => Messages
        .Where(m => m.AdditionalProperties is not null)
        .SelectMany(m => m.AdditionalProperties!)
        .Where(kvp => kvp.Value is ParameterDeclaration { })
        .ToImmutableDictionary(kvp => kvp.Key, kvp => (ParameterDeclaration)kvp.Value!);
    public IEnumerable<ChatMessage> Messages { get; } = Array.AsReadOnly(messages);
    public ChatOptions Options { get; } = options ?? new ChatOptions();
}
