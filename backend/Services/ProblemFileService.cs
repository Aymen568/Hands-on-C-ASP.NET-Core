using System.Text.Json;
using CodePracticePlatform.Api.Models;
using CodePracticePlatform.Api.Factories;

namespace CodePracticePlatform.Api.Services;

/// <summary>
/// Service for loading problems from JSON files in shared/problems folder
/// </summary>
public class ProblemFileService
{
    private readonly IProblemFactory _problemFactory;
    private readonly ILogger<ProblemFileService> _logger;
    private readonly string _problemsPath;
    private List<Problem>? _cachedProblems;

    public ProblemFileService(
        IProblemFactory problemFactory,
        ILogger<ProblemFileService> logger,
        IConfiguration configuration)
    {
        _problemFactory = problemFactory;
        _logger = logger;
        
        // Get problems path - relative to backend folder
        var basePath = Directory.GetCurrentDirectory();
        _problemsPath = Path.Combine(basePath, "..", "shared", "problems");
        
        // Normalize the path
        _problemsPath = Path.GetFullPath(_problemsPath);
    }

    /// <summary>
    /// Load all problems from JSON files
    /// </summary>
    public virtual async Task<List<Problem>> LoadProblemsFromFilesAsync()
    {
        // Return cached problems if available
        if (_cachedProblems != null)
        {
            return _cachedProblems;
        }

        var problems = new List<Problem>();

        if (!Directory.Exists(_problemsPath))
        {
            _logger.LogWarning("Problems directory not found: {Path}", _problemsPath);
            return problems;
        }

        var jsonFiles = Directory.GetFiles(_problemsPath, "*.json", SearchOption.TopDirectoryOnly)
            .OrderBy(f => f)
            .ToList();

        _logger.LogInformation("Loading {Count} problem files from {Path}", jsonFiles.Count, _problemsPath);

        foreach (var filePath in jsonFiles)
        {
            try
            {
                var jsonContent = await File.ReadAllTextAsync(filePath);
                var problem = _problemFactory.CreateProblemFromJson(jsonContent);
                
                // Assign ID based on file name (e.g., problem-001.json -> ID 1)
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var idString = fileName.Replace("problem-", "").TrimStart('0');
                if (string.IsNullOrEmpty(idString))
                    idString = "0";
                    
                if (int.TryParse(idString, out var id))
                {
                    problem.Id = id;
                }
                else
                {
                    // If parsing fails, use index + 1
                    problem.Id = problems.Count + 1;
                }

                problems.Add(problem);
                _logger.LogInformation("Loaded problem: {Title} from {File}", problem.Title, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading problem from file {File}", filePath);
            }
        }

        _cachedProblems = problems;
        return problems;
    }

    /// <summary>
    /// Reload problems from files (clears cache)
    /// </summary>
    public virtual void ClearCache()
    {
        _cachedProblems = null;
    }

    /// <summary>
    /// Get a problem by ID
    /// </summary>
    public virtual async Task<Problem?> GetProblemByIdAsync(int id)
    {
        var problems = await LoadProblemsFromFilesAsync();
        return problems.FirstOrDefault(p => p.Id == id);
    }

    /// <summary>
    /// Get all problems
    /// </summary>
    public virtual async Task<IEnumerable<Problem>> GetAllProblemsAsync()
    {
        return await LoadProblemsFromFilesAsync();
    }

    /// <summary>
    /// Get problems by difficulty
    /// </summary>
    public async Task<IEnumerable<Problem>> GetProblemsByDifficultyAsync(Difficulty difficulty)
    {
        var problems = await LoadProblemsFromFilesAsync();
        return problems.Where(p => p.Difficulty == difficulty);
    }

    /// <summary>
    /// Get problems by feature type
    /// </summary>
    public async Task<IEnumerable<Problem>> GetProblemsByFeatureTypeAsync(FeatureType featureType)
    {
        var problems = await LoadProblemsFromFilesAsync();
        return problems.Where(p => p.FeatureType == featureType);
    }
}

