namespace CodePracticePlatform.Api.Models;


public class Evaluation
{
    public int Id { get; set; }
    public int SubmissionId { get; set; }
    public Submission? Submission { get; set; }
    public string TestResults { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
    public int TotalTests { get; set; }
    public int PassedTests { get; set; }
    public int FailedTests { get; set; }


    public void CalculateScore()
    {
        if (TotalTests == 0)
        {
            Score = 0;
            return;
        }

        Score = (PassedTests * 100) / TotalTests;
    }

    public string GenerateFeedback()
    {
        var feedback = new System.Text.StringBuilder();

        if (PassedTests == TotalTests && TotalTests > 0)
        {
            feedback.AppendLine("✅ All tests passed! Great job!");
        }
        else if (FailedTests > 0)
        {
            feedback.AppendLine($"❌ {FailedTests} out of {TotalTests} tests failed.");
            feedback.AppendLine("Please review the test results and fix the issues.");
        }

        if (!string.IsNullOrWhiteSpace(Feedback))
        {
            feedback.AppendLine($"\nReviewer Feedback:\n{Feedback}");
        }

        return feedback.ToString();
    }
}

