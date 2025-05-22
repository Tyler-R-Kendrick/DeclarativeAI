using Microsoft.Extensions.AI.Evaluation;

namespace Declarative.AI.Abstractions;

public class TrainablePrompt(
    PromptOptimizer optimizer,
    IEvaluator evaluator,
    ChatOptions options,
    params ChatMessage[] messages)
    : Prompt(options, messages)
{
    private readonly Queue<Prompt> _promptHistory = [];

    public async Task<TrainingResult> TrainAsync(
        IChatClient client,
        ChatConfiguration? chatConfiguration = null,
        IEnumerable<EvaluationContext>? additionalContext = null,
        CancellationToken cancellationToken = default)
    {
        var chatResponse = await client.GetResponseAsync(this, cancellationToken);
        var result = await evaluator.EvaluateAsync(
            this, chatResponse, chatConfiguration, additionalContext, cancellationToken);

        await using (TrainingScope scope = new(optimizer, new TrainingContext()))
        {
            PromptOptimizer.Feedback feedback = new(
                this,
                result,
                scope.Context);
            scope.CollectFeedback(feedback);
            await scope.CommitAsync(cancellationToken);
        }

        var improvedDeclaractivePrompt = await optimizer.OptimizeAsync(this, cancellationToken);
        _promptHistory.Enqueue(improvedDeclaractivePrompt);
        _messages = improvedDeclaractivePrompt.Messages;
        _options = improvedDeclaractivePrompt.Options;

        return new TrainingResult(
            chatResponse,
            result);
    }

    public bool TryRollback()
    {
        if (_promptHistory.Count == 0)
        {
            return false;
        }

        var prompt = _promptHistory.Dequeue();
        _messages = prompt.Messages;
        _options = prompt.Options;
        return true;
    }
}
