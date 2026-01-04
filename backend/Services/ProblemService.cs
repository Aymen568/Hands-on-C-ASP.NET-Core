using CodePracticePlatform.Api.Models;
using System.Linq;

namespace CodePracticePlatform.Api.Services;

/// <summary>
/// Service for problem management (OOP - Service Layer)
/// Now loads problems from JSON files instead of database
/// </summary>
public class ProblemService : IProblemService
{
    private readonly ProblemFileService _problemFileService;
    private readonly ILogger<ProblemService> _logger;

    public ProblemService(
        ProblemFileService problemFileService,
        ILogger<ProblemService> logger)
    {
        _problemFileService = problemFileService;
        _logger = logger;
    }

    public async Task<Problem?> GetProblemByIdAsync(int id)
    {
        return await _problemFileService.GetProblemByIdAsync(id);
    }

    public async Task<IEnumerable<Problem>> GetAllProblemsAsync()
    {
        return await _problemFileService.GetAllProblemsAsync();
    }

    public async Task<IEnumerable<Problem>> GetProblemsByDifficultyAsync(Difficulty difficulty)
    {
        return await _problemFileService.GetProblemsByDifficultyAsync(difficulty);
    }

    public async Task<IEnumerable<Problem>> GetProblemsByFeatureTypeAsync(FeatureType featureType)
    {
        return await _problemFileService.GetProblemsByFeatureTypeAsync(featureType);
    }

    public async Task<IEnumerable<Problem>> GetProblemsAsync(Difficulty? difficulty, FeatureType? featureType)
    {
        var problems = await _problemFileService.GetAllProblemsAsync();

        if (difficulty.HasValue)
        {
            problems = problems.Where(p => p.Difficulty == difficulty.Value);
        }

        if (featureType.HasValue)
        {
            problems = problems.Where(p => p.FeatureType == featureType.Value);
        }

        return problems;
    }

    /// <summary>
    /// Reload problems from files (useful after adding new problem files)
    /// </summary>
    public void ReloadProblems()
    {
        _problemFileService.ClearCache();
        _logger.LogInformation("Problem cache cleared. Problems will be reloaded on next request.");
    }
}

