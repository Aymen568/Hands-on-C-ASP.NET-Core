using CodePracticePlatform.Api.Models;
using CodePracticePlatform.Api.Repositories;
using CodePracticePlatform.Api.Strategies;

namespace CodePracticePlatform.Api.Services;

public class EvaluationService : IEvaluationService
{
    private readonly IEvaluationRepository _evaluationRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IEvaluationStrategy _evaluationStrategy;
    private readonly ILogger<EvaluationService> _logger;

    public EvaluationService(
        IEvaluationRepository evaluationRepository,
        ISubmissionRepository submissionRepository,
        IEvaluationStrategy evaluationStrategy,
        ILogger<EvaluationService> logger)
    {
        _evaluationRepository = evaluationRepository;
        _submissionRepository = submissionRepository;
        _evaluationStrategy = evaluationStrategy;
        _logger = logger;
    }

    public async Task<Evaluation?> GetEvaluationBySubmissionIdAsync(int submissionId)
    {
        return await _evaluationRepository.GetBySubmissionIdAsync(submissionId);
    }

    public async Task<Evaluation> EvaluateSubmissionAsync(int submissionId)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId);
        if (submission == null)
        {
            throw new ArgumentException($"Submission with id {submissionId} not found");
        }

        // Update submission status to Evaluating
        submission.UpdateStatus(SubmissionStatus.Evaluating);
        await _submissionRepository.UpdateAsync(submission);

        _logger.LogInformation("Starting evaluation for submission {SubmissionId}", submissionId);

        try
        {
            // Use strategy to evaluate
            var evaluation = await _evaluationStrategy.EvaluateAsync(submission);

            // Save evaluation
            evaluation = await _evaluationRepository.CreateAsync(evaluation);

            // Update submission status based on evaluation
            if (evaluation.PassedTests == evaluation.TotalTests && evaluation.TotalTests > 0)
            {
                submission.UpdateStatus(SubmissionStatus.Passed);
            }
            else
            {
                submission.UpdateStatus(SubmissionStatus.Failed);
            }

            await _submissionRepository.UpdateAsync(submission);

            _logger.LogInformation(
                "Evaluation completed for submission {SubmissionId}: {Status}",
                submissionId,
                submission.Status
            );

            return evaluation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating submission {SubmissionId}", submissionId);
            submission.UpdateStatus(SubmissionStatus.Failed);
            await _submissionRepository.UpdateAsync(submission);
            throw;
        }
    }
}

