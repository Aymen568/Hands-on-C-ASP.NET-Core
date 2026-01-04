using CodePracticePlatform.Api.Models;

namespace CodePracticePlatform.Api.Repositories;


public interface IEvaluationRepository
{
    Task<Evaluation?> GetByIdAsync(int id);
    Task<Evaluation?> GetBySubmissionIdAsync(int submissionId);
    Task<IEnumerable<Evaluation>> GetAllAsync();
    Task<Evaluation> CreateAsync(Evaluation evaluation);
    Task<Evaluation> UpdateAsync(Evaluation evaluation);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

