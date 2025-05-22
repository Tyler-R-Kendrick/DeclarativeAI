namespace Declarative.AI.Abstractions;

public class Prompt(
    ChatOptions? options,
    params IEnumerable<ChatMessage> messages)
    : AITool, IPrompt
{
    protected IEnumerable<ChatMessage> _messages = messages ?? [];
    public IEnumerable<ChatMessage> Messages => _messages;

    protected ChatOptions _options = options ?? new ChatOptions();
    public ChatOptions Options => _options;
}
