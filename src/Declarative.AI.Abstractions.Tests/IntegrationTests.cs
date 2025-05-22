using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;

namespace Declarative.AI.Abstractions.Tests;

[TestClass]
public class IntegrationTests
{
    [TestMethod]
    public void DeclarativeFlow_Succeeds()
    {
        // Arrange

        // Create a test prompt with a system message
        var content = "This is a test prompt.";
        TextContent aiContent = new(content);
        ChatMessage message = new(ChatRole.System, [aiContent]);
        ChatOptions options = new();
        DeclarativePrompt declarativePrompt = new FakeDeclarativePrompt(options, message);

        // Act
        DeclarativeBinder binder = new FakeBinder();
        var prompt = binder.Bind(declarativePrompt);

        // Assert
        Assert.AreEqual(1, prompt.Messages.Count());
    }

    [TestMethod]
    public async Task SelfImprovingFlow_Succeeds()
    {
        // Arrange
        int calledFeedback = 0;
        var content = "This is a test prompt.";
        TextContent aiContent = new(content);
        ChatMessage message = new(ChatRole.System, [aiContent]);
        ChatOptions options = new();
        PromptOptimizer optimizer = new FakePromptOptimizer(
            applyFeedback: feedback => Task.FromResult(calledFeedback++)
        );
        IEvaluator evaluator = new FakeEvaluator();
        DeclarativePrompt declarativePrompt = new FakeDeclarativePrompt(options, message);

        // Act
        await using (TrainingScope scope = new(optimizer, new TrainingContext()))
        {
            // Bind the prompt to a context and engine
            DeclarativeBinder binder = new FakeBinder();

            IChatClient chatClient = new FakeChatClient();
            var chatResponse = await chatClient.GetResponseAsync(
                declarativePrompt, binder);

            var result = await evaluator.EvaluateAsync(
                declarativePrompt, chatResponse, null, null, default);

            PromptOptimizer.Feedback feedback = new(
                declarativePrompt,
                result,
                scope.Context);
            scope.CollectFeedback(feedback);
            await scope.CommitAsync();
        }

        var improvedDeclaractivePrompt = await optimizer.OptimizeAsync(declarativePrompt);

        // Assert
        Assert.AreEqual(1, calledFeedback);
        Assert.AreNotEqual(declarativePrompt, improvedDeclaractivePrompt);
    }

    public async Task TrainablePrompt_Succeeds()
    {
        // Arrange
        int calledFeedback = 0;
        var content = "This is a test prompt.";
        TextContent aiContent = new(content);
        ChatMessage message = new(ChatRole.System, [aiContent]);
        ChatOptions options = new();
        PromptOptimizer optimizer = new FakePromptOptimizer(
            applyFeedback: feedback => Task.FromResult(calledFeedback++)
        );
        IEvaluator evaluator = new FakeEvaluator();
        TrainablePrompt trainablePrompt = new(
            optimizer,
            evaluator,
            options,
            message);

        // Act
        IChatClient client = new FakeChatClient();
        var result = await trainablePrompt.TrainAsync(
            client: client,
            chatConfiguration: new(client),
            additionalContext: null,
            cancellationToken: default);

        // Assert
        Assert.AreEqual(1, calledFeedback);
        Assert.DoesNotContain(message, trainablePrompt.Messages);
    }

}
