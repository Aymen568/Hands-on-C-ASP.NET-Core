namespace CodePracticePlatform.Api.Services;

/// <summary>
/// Service interface for Git operations and test execution
/// </summary>
public interface IGitService
{
    Task<string> CloneRepositoryAsync(string repositoryUrl, string submissionId);
    Task<TestResult> RunTestsAsync(string repositoryPath, int problemId);
    Task<bool> ValidateRepositoryUrlAsync(string repositoryUrl);
    Task CleanupRepositoryAsync(string repositoryPath);
}

