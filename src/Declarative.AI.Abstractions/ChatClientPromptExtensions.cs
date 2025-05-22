namespace Declarative.AI.Abstractions;

public static class ChatClientPromptExtensions
{
    public static Task<ChatResponse> GetResponseAsync(
        this IChatClient client,
        Prompt prompt,
        CancellationToken cancellationToken = default)
        => client.GetResponseAsync(
            prompt.Messages,
            prompt.Options,
            cancellationToken);
    public static IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        this IChatClient client,
        Prompt prompt,
        CancellationToken cancellationToken = default)
        => client.GetStreamingResponseAsync(
            prompt.Messages,
            prompt.Options,
            cancellationToken);
}
