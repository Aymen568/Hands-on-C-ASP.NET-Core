using CodePracticePlatform.Api.Models;

namespace CodePracticePlatform.Api.Services;

public interface IEvaluationService
{
    Task<Evaluation?> GetEvaluationBySubmissionIdAsync(int submissionId);
    Task<Evaluation> EvaluateSubmissionAsync(int submissionId);
}
