using Microsoft.Extensions.AI.Evaluation;

namespace Declarative.AI.Abstractions;

public abstract class PromptOptimizer
{
    public record Feedback(
        IPrompt Prompt,
        EvaluationResult EvaluationResult,
        TrainingContext TrainingContext);
    internal Task ClearAsync(CancellationToken cancellationToken = default)
        => OnClearAsync(cancellationToken);
    protected abstract Task OnClearAsync(CancellationToken cancellationToken = default);

    internal Task ApplyFeedbackAsync(
        CancellationToken cancellationToken = default,
        params IEnumerable<Feedback> feedback)
        => OnApplyFeedbackAsync(cancellationToken, feedback);
    protected abstract Task OnApplyFeedbackAsync(
        CancellationToken cancellationToken = default,
        params IEnumerable<Feedback> feedback);

    internal Task MutateParametersAsync(
        Dictionary<string, ParameterDeclaration> parameters,
        CancellationToken cancellationToken = default)
        => OnMutateParametersAsync(parameters, cancellationToken);
    protected abstract Task OnMutateParametersAsync(
        Dictionary<string, ParameterDeclaration> parameters,
        CancellationToken cancellationToken = default);

    public abstract Task<Prompt> OptimizeAsync(
        Prompt prompt,
        CancellationToken cancellationToken = default);
    public abstract Task<DeclarativePrompt> OptimizeAsync(
        DeclarativePrompt prompt,
        CancellationToken cancellationToken = default);
}