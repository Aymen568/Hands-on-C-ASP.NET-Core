namespace CodePracticePlatform.Api.Models.DTOs;


public class CreateSubmissionDto
{
    public int ProblemId { get; set; }
    public string GitRepositoryUrl { get; set; } = string.Empty;
}

