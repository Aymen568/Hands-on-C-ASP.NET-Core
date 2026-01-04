namespace CodePracticePlatform.Api.Models;


public class Submission
{
    public int Id { get; set; }
    public int ProblemId { get; set; }
    public Problem? Problem { get; set; }
    public int UserId { get; set; } = 1; // For now, single user
    public string GitRepositoryUrl { get; set; } = string.Empty;
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Pending;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EvaluatedAt { get; set; }
    public Evaluation? Evaluation { get; set; }


    public void MarkAsEvaluated()
    {
        EvaluatedAt = DateTime.UtcNow;
    }


    public void UpdateStatus(SubmissionStatus newStatus)
    {
        Status = newStatus;
        if (newStatus == SubmissionStatus.Passed || 
            newStatus == SubmissionStatus.Failed || 
            newStatus == SubmissionStatus.NeedsReview)
        {
            MarkAsEvaluated();
        }
    }
}

