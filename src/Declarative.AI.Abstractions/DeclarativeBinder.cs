namespace Declarative.AI.Abstractions;

public abstract class DeclarativeBinder(DeclarativeBinderOptions? options = null)
{
    public DeclarativeBinderOptions Options { get; } = options ?? new();

    public abstract AIContent Bind(DeclarativeAIContent declarativeType, BindingContext? context = null);
    public abstract ChatMessage Bind(DeclarativeChatMessage declarativeType, BindingContext? context = null);
    public abstract Prompt Bind(DeclarativePrompt declarativeType, BindingContext? context = null);
}
