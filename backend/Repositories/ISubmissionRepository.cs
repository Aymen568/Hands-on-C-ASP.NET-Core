using CodePracticePlatform.Api.Models;

namespace CodePracticePlatform.Api.Repositories;


public interface ISubmissionRepository
{
    Task<Submission?> GetByIdAsync(int id);
    Task<IEnumerable<Submission>> GetAllAsync();
    Task<IEnumerable<Submission>> GetByProblemIdAsync(int problemId);
    Task<IEnumerable<Submission>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Submission>> GetByStatusAsync(SubmissionStatus status);
    Task<Submission> CreateAsync(Submission submission);
    Task<Submission> UpdateAsync(Submission submission);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

