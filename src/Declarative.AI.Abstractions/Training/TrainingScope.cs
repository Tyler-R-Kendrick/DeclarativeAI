namespace Declarative.AI.Abstractions;
public class TrainingScope(
    PromptOptimizer optimizer,
    TrainingContext context)
    : IAsyncDisposable
{
    private readonly Queue<PromptOptimizer.Feedback[]> _feedbackCollectionHistory = [];
    private readonly List<PromptOptimizer.Feedback> _feedbackCollection = [];
    // 0 = not disposed, 1 = disposed
    private int _disposed;

    public TrainingContext Context { get; } = context;

    public void CollectFeedback(PromptOptimizer.Feedback feedback)
        => _feedbackCollection.Add(feedback);

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore(
        bool disposing = true,
        CancellationToken cancellationToken = default)
    {
        // Ensure only one thread runs disposal logic
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        _feedbackCollection.Clear();
        _feedbackCollectionHistory.Clear();
        await optimizer.ClearAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await optimizer.ClearAsync(cancellationToken);
        var feedback = _feedbackCollectionHistory.Dequeue();
        await optimizer.ApplyFeedbackAsync(cancellationToken, feedback);
        //await optimizer.MutateParametersAsync();
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        var feedback = _feedbackCollection.ToArray();
        await optimizer.ClearAsync(cancellationToken);
        await optimizer.ApplyFeedbackAsync(cancellationToken, feedback);
        //await optimizer.MutateParametersAsync();
        _feedbackCollectionHistory.Enqueue(feedback);
        _feedbackCollection.Clear();
    }

    ~TrainingScope()
        => DisposeAsyncCore(disposing: false)
            .AsTask()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
}
