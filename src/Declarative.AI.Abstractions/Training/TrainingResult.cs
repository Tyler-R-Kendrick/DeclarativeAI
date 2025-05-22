using Microsoft.Extensions.AI.Evaluation;

namespace Declarative.AI.Abstractions;

public record TrainingResult(
    ChatResponse ChatResponse,
    EvaluationResult EvaluationResult);
