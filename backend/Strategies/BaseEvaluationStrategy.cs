using CodePracticePlatform.Api.Models;

namespace CodePracticePlatform.Api.Strategies;

/// <summary>
/// Base evaluation strategy using Template Method pattern
/// </summary>
public abstract class BaseEvaluationStrategy : IEvaluationStrategy
{
    public async Task<Evaluation> EvaluateAsync(Submission submission)
    {
        // Template method - defines the algorithm structure
        var evaluation = new Evaluation
        {
            SubmissionId = submission.Id,
            EvaluatedAt = DateTime.UtcNow
        };

        // Step 1: Prepare evaluation
        await PrepareEvaluationAsync(submission, evaluation);

        // Step 2: Execute evaluation
        await ExecuteEvaluationAsync(submission, evaluation);

        // Step 3: Process results
        await ProcessResultsAsync(submission, evaluation);

        // Step 4: Finalize evaluation
        await FinalizeEvaluationAsync(submission, evaluation);

        return evaluation;
    }

    protected virtual Task PrepareEvaluationAsync(Submission submission, Evaluation evaluation)
    {
        // Default implementation - can be overridden
        return Task.CompletedTask;
    }

    protected abstract Task ExecuteEvaluationAsync(Submission submission, Evaluation evaluation);

    protected virtual Task ProcessResultsAsync(Submission submission, Evaluation evaluation)
    {
        evaluation.CalculateScore();
        evaluation.GenerateFeedback();
        return Task.CompletedTask;
    }

    protected virtual Task FinalizeEvaluationAsync(Submission submission, Evaluation evaluation)
    {
        // Default implementation - can be overridden
        return Task.CompletedTask;
    }
}

