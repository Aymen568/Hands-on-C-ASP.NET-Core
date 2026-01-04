using CodePracticePlatform.Api.Models;

namespace CodePracticePlatform.Api.Services;

public interface IProblemService
{
    Task<Problem?> GetProblemByIdAsync(int id);
    Task<IEnumerable<Problem>> GetAllProblemsAsync();
    Task<IEnumerable<Problem>> GetProblemsByDifficultyAsync(Difficulty difficulty);
    Task<IEnumerable<Problem>> GetProblemsByFeatureTypeAsync(FeatureType featureType);
    Task<IEnumerable<Problem>> GetProblemsAsync(Difficulty? difficulty, FeatureType? featureType);
    void ReloadProblems();
}
