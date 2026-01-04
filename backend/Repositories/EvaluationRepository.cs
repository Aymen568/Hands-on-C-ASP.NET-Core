using System.Text.Json;
using CodePracticePlatform.Api.Models;

namespace CodePracticePlatform.Api.Repositories;

/// <summary>
/// Repository for evaluations using file-based JSON storage
/// </summary>
public class EvaluationRepository : IEvaluationRepository
{
    private readonly string _evaluationsPath;
    private readonly ILogger<EvaluationRepository> _logger;
    private List<Evaluation>? _cachedEvaluations;

    public EvaluationRepository(ILogger<EvaluationRepository> logger, IConfiguration configuration)
    {
        _logger = logger;
        
        // Get evaluations path - relative to backend folder
        var basePath = Directory.GetCurrentDirectory();
        _evaluationsPath = Path.Combine(basePath, "..", "shared", "evaluations");
        _evaluationsPath = Path.GetFullPath(_evaluationsPath);
        
        // Ensure directory exists
        if (!Directory.Exists(_evaluationsPath))
        {
            Directory.CreateDirectory(_evaluationsPath);
        }
    }

    private async Task<List<Evaluation>> LoadEvaluationsFromFilesAsync()
    {
        if (_cachedEvaluations != null)
        {
            return _cachedEvaluations;
        }

        var evaluations = new List<Evaluation>();

        if (!Directory.Exists(_evaluationsPath))
        {
            return evaluations;
        }

        var jsonFiles = Directory.GetFiles(_evaluationsPath, "evaluation-*.json", SearchOption.TopDirectoryOnly);

        foreach (var filePath in jsonFiles)
        {
            try
            {
                var jsonContent = await File.ReadAllTextAsync(filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var evaluation = JsonSerializer.Deserialize<Evaluation>(jsonContent, options);
                
                if (evaluation != null)
                {
                    evaluations.Add(evaluation);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading evaluation from file {File}", filePath);
            }
        }

        _cachedEvaluations = evaluations;
        return evaluations;
    }

    private async Task SaveEvaluationToFileAsync(Evaluation evaluation)
    {
        try
        {
            var fileName = $"evaluation-{evaluation.Id}.json";
            var filePath = Path.Combine(_evaluationsPath, fileName);
            
            var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
            var json = JsonSerializer.Serialize(evaluation, options);
            
            await File.WriteAllTextAsync(filePath, json);
            _cachedEvaluations = null; // Invalidate cache
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving evaluation {Id}", evaluation.Id);
            throw;
        }
    }

    public async Task<Evaluation?> GetByIdAsync(int id)
    {
        var evaluations = await LoadEvaluationsFromFilesAsync();
        return evaluations.FirstOrDefault(e => e.Id == id);
    }

    public async Task<Evaluation?> GetBySubmissionIdAsync(int submissionId)
    {
        var evaluations = await LoadEvaluationsFromFilesAsync();
        return evaluations.FirstOrDefault(e => e.SubmissionId == submissionId);
    }

    public async Task<IEnumerable<Evaluation>> GetAllAsync()
    {
        var evaluations = await LoadEvaluationsFromFilesAsync();
        return evaluations.OrderByDescending(e => e.EvaluatedAt);
    }

    public async Task<Evaluation> CreateAsync(Evaluation evaluation)
    {
        var evaluations = await LoadEvaluationsFromFilesAsync();
        
        // Auto-generate ID if not set
        if (evaluation.Id == 0)
        {
            evaluation.Id = evaluations.Any() ? evaluations.Max(e => e.Id) + 1 : 1;
        }
        
        evaluation.EvaluatedAt = DateTime.UtcNow;
        
        await SaveEvaluationToFileAsync(evaluation);
        return evaluation;
    }

    public async Task<Evaluation> UpdateAsync(Evaluation evaluation)
    {
        await SaveEvaluationToFileAsync(evaluation);
        return evaluation;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var fileName = $"evaluation-{id}.json";
            var filePath = Path.Combine(_evaluationsPath, fileName);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _cachedEvaluations = null; // Invalidate cache
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting evaluation {Id}", id);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var evaluation = await GetByIdAsync(id);
        return evaluation != null;
    }
}

