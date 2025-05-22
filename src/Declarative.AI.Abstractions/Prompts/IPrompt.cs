namespace Declarative.AI.Abstractions;

public interface IPrompt
{
    IEnumerable<ChatMessage> Messages { get; }
    ChatOptions Options { get; }
}
