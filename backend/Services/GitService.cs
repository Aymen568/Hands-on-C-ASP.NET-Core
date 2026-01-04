using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace CodePracticePlatform.Api.Services;

/// <summary>
/// Service for Git operations and test execution
/// </summary>
public class GitService : IGitService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GitService> _logger;
    private readonly string _baseClonePath;

    public GitService(IConfiguration configuration, ILogger<GitService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _baseClonePath = _configuration["ApiSettings:GitClonePath"] ?? "./temp-repos";
        
        // Ensure base directory exists
        if (!Directory.Exists(_baseClonePath))
        {
            Directory.CreateDirectory(_baseClonePath);
        }
    }

    public async Task<string> CloneRepositoryAsync(string repositoryUrl, string submissionId)
    {
        var clonePath = Path.Combine(_baseClonePath, $"submission-{submissionId}");
        
        // Clean up if directory already exists
        if (Directory.Exists(clonePath))
        {
            Directory.Delete(clonePath, true);
        }

        _logger.LogInformation("Cloning repository {RepositoryUrl} to {ClonePath}", repositoryUrl, clonePath);

        var processInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = $"clone {repositoryUrl} {clonePath}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start git process");
        }

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync();
            throw new InvalidOperationException($"Git clone failed: {error}");
        }

        return clonePath;
    }

    public async Task<TestResult> RunTestsAsync(string repositoryPath, int problemId)
    {
        _logger.LogInformation("Running tests for problem {ProblemId} in {RepositoryPath}", problemId, repositoryPath);

        var testResult = new TestResult();

        try
        {
            // Check if package.json exists
            var packageJsonPath = Path.Combine(repositoryPath, "package.json");
            if (!File.Exists(packageJsonPath))
            {
                testResult.ErrorMessage = "package.json not found in repository";
                testResult.Success = false;
                return testResult;
            }

            // Install dependencies
            await RunNpmCommandAsync(repositoryPath, "install");

            // Run tests (assuming Jest or similar test runner)
            var testOutput = await RunNpmCommandAsync(repositoryPath, "test -- --json");

            // Parse test results
            testResult = ParseTestResults(testOutput);
            testResult.Success = testResult.FailedTests == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running tests");
            testResult.ErrorMessage = ex.Message;
            testResult.Success = false;
            testResult.Output = $"Error: {ex.Message}";
        }

        return testResult;
    }

    public async Task<bool> ValidateRepositoryUrlAsync(string repositoryUrl)
    {
        if (string.IsNullOrWhiteSpace(repositoryUrl))
            return false;

        if (!Uri.IsWellFormedUriString(repositoryUrl, UriKind.Absolute))
            return false;

        // Check if it's a Git repository URL
        if (!repositoryUrl.StartsWith("http://") && 
            !repositoryUrl.StartsWith("https://") && 
            !repositoryUrl.StartsWith("git@"))
            return false;

        return await Task.FromResult(true);
    }

    public Task CleanupRepositoryAsync(string repositoryPath)
    {
        try
        {
            if (Directory.Exists(repositoryPath))
            {
                Directory.Delete(repositoryPath, true);
                _logger.LogInformation("Cleaned up repository at {RepositoryPath}", repositoryPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cleanup repository at {RepositoryPath}", repositoryPath);
        }

        return Task.CompletedTask;
    }

    private async Task<string> RunNpmCommandAsync(string workingDirectory, string arguments)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "npm",
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start npm process");
        }

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0 && !arguments.Contains("test"))
        {
            throw new InvalidOperationException($"npm command failed: {error}");
        }

        return output + error;
    }

    private TestResult ParseTestResults(string testOutput)
    {
        var result = new TestResult
        {
            Output = testOutput
        };

        try
        {
            // Try to parse Jest JSON output
            var jsonStart = testOutput.IndexOf('{');
            if (jsonStart >= 0)
            {
                var jsonEnd = testOutput.LastIndexOf('}');
                if (jsonEnd > jsonStart)
                {
                    var json = testOutput.Substring(jsonStart, jsonEnd - jsonStart + 1);
                    var testData = System.Text.Json.JsonSerializer.Deserialize<JestTestResult>(json);
                    
                    if (testData != null)
                    {
                        result.TotalTests = testData.NumTotalTests;
                        result.PassedTests = testData.NumPassedTests;
                        result.FailedTests = testData.NumFailedTests;
                        result.Success = testData.Success;
                    }
                }
            }

            // Fallback: try to parse from text output
            if (result.TotalTests == 0)
            {
                var passedMatch = System.Text.RegularExpressions.Regex.Match(testOutput, @"(\d+)\s+passed");
                var failedMatch = System.Text.RegularExpressions.Regex.Match(testOutput, @"(\d+)\s+failed");
                
                if (passedMatch.Success)
                    result.PassedTests = int.Parse(passedMatch.Groups[1].Value);
                if (failedMatch.Success)
                    result.FailedTests = int.Parse(failedMatch.Groups[1].Value);
                
                result.TotalTests = result.PassedTests + result.FailedTests;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse test results");
            result.Output = testOutput;
        }

        return result;
    }

    private class JestTestResult
    {
        public int NumTotalTests { get; set; }
        public int NumPassedTests { get; set; }
        public int NumFailedTests { get; set; }
        public bool Success { get; set; }
    }
}

