
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;

namespace Declarative.AI.Abstractions.Tests;

public sealed class FakeChatClient : IChatClient
{
    public FakeChatClient() => GetServiceCallback = DefaultGetServiceCallback;

    public IServiceProvider? Services { get; set; }

    public Func<IEnumerable<ChatMessage>, ChatOptions?, CancellationToken, Task<ChatResponse>>? GetResponseAsyncCallback { get; set; }

    public Func<IEnumerable<ChatMessage>, ChatOptions?, CancellationToken, IAsyncEnumerable<ChatResponseUpdate>>? GetStreamingResponseAsyncCallback { get; set; }

    public Func<Type, object?, object?> GetServiceCallback { get; set; }

    private FakeChatClient? DefaultGetServiceCallback(Type serviceType, object? serviceKey) =>
        serviceType is not null && serviceKey is null && serviceType.IsInstanceOfType(this) ? this : null;

    public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        => GetResponseAsyncCallback?.Invoke(messages, options, cancellationToken)
            ?? Task.FromResult(new ChatResponse());

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = GetStreamingResponseAsyncCallback?.Invoke(messages, options, cancellationToken);
        if (response is not null)
        {
            await foreach (var update in response.WithCancellation(cancellationToken))
            {
                yield return update;
            }
        }
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
        => GetServiceCallback(serviceType, serviceKey);

    void IDisposable.Dispose()
    {
        // No resources need disposing.
    }
}

public class FakeDeclarativePrompt(
    ChatOptions options,
    params ChatMessage[] messages)
    : DeclarativePrompt(options, messages);

public class FakeTemplatedPrompt(
    string template,
    ITemplateEngine templateEngine,
    params ChatMessage[] messages)
    : DeclarativePrompt(new ChatOptions(), messages)
{
    public string Template { get; } = template;
    public ITemplateEngine TemplateEngine { get; } = templateEngine;
}

public class FakeBinder(DeclarativeBinderOptions? options = null)
    : DeclarativeBinder(options)
{
    public override AIContent Bind(DeclarativeAIContent declarativeType, BindingContext? context)
    {
        return new TextContent("Generated content");
    }

    public override ChatMessage Bind(DeclarativeChatMessage declarativeType, BindingContext? context)
    {
        return new ChatMessage(ChatRole.User, [new TextContent("Generated message")]);
    }

    public override Prompt Bind(DeclarativePrompt declarativeType, BindingContext? context)
    {
        return new Prompt(
            options: declarativeType.Options.Clone(),
            messages: [.. declarativeType.Messages.Select(m => m.Clone())]
        );
    }
}

public interface ITemplateEngine
{
    string Render(string template, IDictionary<object, object?> arguments);
}

internal class FakePromptOptimizer(
    Func<IEnumerable<PromptOptimizer.Feedback>, Task>? applyFeedback = null,
    Func<Task>? clear = null,
    Func<Dictionary<string, ParameterDeclaration>, Task>? mutateParameters = null)
    : PromptOptimizer
{
    protected override Task OnApplyFeedbackAsync(
        CancellationToken cancellationToken = default,
        params IEnumerable<Feedback> feedback)
        => applyFeedback?.Invoke(feedback) ?? Task.CompletedTask;

    protected override Task OnClearAsync(
        CancellationToken cancellationToken = default)
        => clear?.Invoke() ?? Task.CompletedTask;

    protected override Task OnMutateParametersAsync(
        Dictionary<string, ParameterDeclaration> parameters,
        CancellationToken cancellationToken = default)
        => mutateParameters?.Invoke(parameters) ?? Task.CompletedTask;

    public override Task<Prompt> OptimizeAsync(
        Prompt prompt,
        CancellationToken cancellationToken = default)
        => Task.FromResult(new Prompt(
            messages: [.. prompt.Messages.Select(m => m.Clone())],
            options: prompt.Options.Clone()));
    public override Task<DeclarativePrompt> OptimizeAsync(
        DeclarativePrompt prompt,
        CancellationToken cancellationToken = default)
        // Implement optimization logic here
        => Task.FromResult(new DeclarativePrompt(
            messages: [.. prompt.Messages.Select(m => m.Clone())],
            options: prompt.Options.Clone()));
}

internal class FakeEvaluator(
    Func<IEnumerable<ChatMessage>, ChatResponse, ChatConfiguration?, IEnumerable<EvaluationContext>?, CancellationToken, ValueTask<EvaluationResult>>? evaluateAsync = null) : IEvaluator
{
    public IReadOnlyCollection<string> EvaluationMetricNames => throw new NotImplementedException();

    public ValueTask<EvaluationResult> EvaluateAsync(IEnumerable<ChatMessage> messages, ChatResponse modelResponse, ChatConfiguration? chatConfiguration = null, IEnumerable<EvaluationContext>? additionalContext = null, CancellationToken cancellationToken = default)
    {
        return evaluateAsync?.Invoke(messages, modelResponse, chatConfiguration, additionalContext, cancellationToken) ?? ValueTask.FromResult(new EvaluationResult());
    }
}
