namespace Declarative.AI.Abstractions;

public static class ChatClientDeclarativePromptExtensions
{
    public static Task<ChatResponse> GetResponseAsync(
        this IChatClient client,
        DeclarativePrompt promptDeclaration,
        DeclarativeBinder binder,
        BindingContext? context = null,
        CancellationToken cancellationToken = default)
    {
        var prompt = binder.Bind(promptDeclaration, context ?? new BindingContext(null));
        return client.GetResponseAsync(prompt, cancellationToken);
    }

    public static IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        this IChatClient client,
        DeclarativePrompt promptDeclaration,
        DeclarativeBinder binder,
        BindingContext? context = null,
        CancellationToken cancellationToken = default)
    {
        var prompt = binder.Bind(promptDeclaration, context ?? new BindingContext(null));
        return client.GetStreamingResponseAsync(prompt, cancellationToken);
    }
}
