namespace CodePracticePlatform.Api.Services;

/// <summary>
/// Represents the result of running tests
/// </summary>
public class TestResult
{
    public string Output { get; set; } = string.Empty;
    public int TotalTests { get; set; }
    public int PassedTests { get; set; }
    public int FailedTests { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

