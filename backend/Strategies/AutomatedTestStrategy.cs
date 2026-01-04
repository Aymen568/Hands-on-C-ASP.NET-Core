using CodePracticePlatform.Api.Models;
using CodePracticePlatform.Api.Services;

namespace CodePracticePlatform.Api.Strategies;

/// <summary>
/// Strategy for automated testing evaluation (Strategy Pattern)
/// </summary>
public class AutomatedTestStrategy : BaseEvaluationStrategy
{
    private readonly IGitService _gitService;
    private readonly ILogger<AutomatedTestStrategy> _logger;

    public AutomatedTestStrategy(IGitService gitService, ILogger<AutomatedTestStrategy> logger)
    {
        _gitService = gitService;
        _logger = logger;
    }

    protected override async Task ExecuteEvaluationAsync(Submission submission, Evaluation evaluation)
    {
        _logger.LogInformation("Starting automated test evaluation for submission {SubmissionId}", submission.Id);

        try
        {
            // Clone the repository
            var repoPath = await _gitService.CloneRepositoryAsync(submission.GitRepositoryUrl, submission.Id.ToString());

            // Run tests
            var testResults = await _gitService.RunTestsAsync(repoPath, submission.ProblemId);

            // Parse test results
            evaluation.TestResults = testResults.Output;
            evaluation.TotalTests = testResults.TotalTests;
            evaluation.PassedTests = testResults.PassedTests;
            evaluation.FailedTests = testResults.FailedTests;

            _logger.LogInformation(
                "Automated test evaluation completed: {PassedTests}/{TotalTests} tests passed",
                evaluation.PassedTests,
                evaluation.TotalTests
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during automated test evaluation for submission {SubmissionId}", submission.Id);
            evaluation.TestResults = $"Error: {ex.Message}";
            evaluation.TotalTests = 0;
            evaluation.PassedTests = 0;
            evaluation.FailedTests = 0;
        }
    }
}



