using Microsoft.Extensions.AI.Evaluation;

namespace Declarative.AI.Abstractions;

public static class EvaluatorExtensions
{
    public static ValueTask<EvaluationResult> EvaluateAsync(
        this IEvaluator evaluator,
        IPrompt prompt,
        ChatResponse modelResponse,
        ChatConfiguration? chatConfiguration = null,
        IEnumerable<EvaluationContext>? additionalContext = null,
        CancellationToken cancellationToken = default)
        => evaluator.EvaluateAsync(
            prompt.Messages,
            modelResponse,
            chatConfiguration,
            additionalContext,
            cancellationToken);
}
