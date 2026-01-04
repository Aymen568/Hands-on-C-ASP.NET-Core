using CodePracticePlatform.Api.Models;
using CodePracticePlatform.Api.Models.DTOs;

namespace CodePracticePlatform.Api.Services;

public interface ISubmissionService
{
    Task<Submission?> GetSubmissionByIdAsync(int id);
    Task<IEnumerable<Submission>> GetAllSubmissionsAsync();
    Task<IEnumerable<Submission>> GetSubmissionsByProblemIdAsync(int problemId);
    Task<IEnumerable<Submission>> GetSubmissionsByUserIdAsync(int userId);
    Task<IEnumerable<Submission>> GetSubmissionsAsync(int? problemId, int? userId);
    Task<Submission> CreateSubmissionAsync(int problemId, int userId, string gitRepositoryUrl);
    Task<Submission> UpdateSubmissionStatusAsync(int submissionId, SubmissionStatus status);
    Task<bool> DeleteSubmissionAsync(int id);
}
