using CodePracticePlatform.Api.Models;

namespace CodePracticePlatform.Api.Strategies;

/// <summary>
/// Strategy interface for evaluation (Strategy Pattern)
/// </summary>
public interface IEvaluationStrategy
{
    Task<Evaluation> EvaluateAsync(Submission submission);
}

