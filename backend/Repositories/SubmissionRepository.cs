using System.Text.Json;
using CodePracticePlatform.Api.Models;

namespace CodePracticePlatform.Api.Repositories;

/// <summary>
/// Repository for submissions using file-based JSON storage
/// </summary>
public class SubmissionRepository : ISubmissionRepository
{
    private readonly string _submissionsPath;
    private readonly ILogger<SubmissionRepository> _logger;
    private List<Submission>? _cachedSubmissions;

    public SubmissionRepository(ILogger<SubmissionRepository> logger, IConfiguration configuration)
    {
        _logger = logger;
        
        // Get submissions path - relative to backend folder
        var basePath = Directory.GetCurrentDirectory();
        _submissionsPath = Path.Combine(basePath, "..", "shared", "submissions");
        _submissionsPath = Path.GetFullPath(_submissionsPath);
        
        // Ensure directory exists
        if (!Directory.Exists(_submissionsPath))
        {
            Directory.CreateDirectory(_submissionsPath);
        }
    }

    private async Task<List<Submission>> LoadSubmissionsFromFilesAsync()
    {
        if (_cachedSubmissions != null)
        {
            return _cachedSubmissions;
        }

        var submissions = new List<Submission>();

        if (!Directory.Exists(_submissionsPath))
        {
            return submissions;
        }

        var jsonFiles = Directory.GetFiles(_submissionsPath, "submission-*.json", SearchOption.TopDirectoryOnly);

        foreach (var filePath in jsonFiles)
        {
            try
            {
                var jsonContent = await File.ReadAllTextAsync(filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var submission = JsonSerializer.Deserialize<Submission>(jsonContent, options);
                
                if (submission != null)
                {
                    submissions.Add(submission);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading submission from file {File}", filePath);
            }
        }

        _cachedSubmissions = submissions;
        return submissions;
    }

    private async Task SaveSubmissionsToFileAsync(Submission submission)
    {
        try
        {
            var fileName = $"submission-{submission.Id}.json";
            var filePath = Path.Combine(_submissionsPath, fileName);
            
            var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
            var json = JsonSerializer.Serialize(submission, options);
            
            await File.WriteAllTextAsync(filePath, json);
            _cachedSubmissions = null; // Invalidate cache
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving submission {Id}", submission.Id);
            throw;
        }
    }

    public async Task<Submission?> GetByIdAsync(int id)
    {
        var submissions = await LoadSubmissionsFromFilesAsync();
        return submissions.FirstOrDefault(s => s.Id == id);
    }

    public async Task<IEnumerable<Submission>> GetAllAsync()
    {
        return await LoadSubmissionsFromFilesAsync();
    }

    public async Task<IEnumerable<Submission>> GetByProblemIdAsync(int problemId)
    {
        var submissions = await LoadSubmissionsFromFilesAsync();
        return submissions.Where(s => s.ProblemId == problemId).OrderByDescending(s => s.SubmittedAt);
    }

    public async Task<IEnumerable<Submission>> GetByUserIdAsync(int userId)
    {
        var submissions = await LoadSubmissionsFromFilesAsync();
        return submissions.Where(s => s.UserId == userId).OrderByDescending(s => s.SubmittedAt);
    }

    public async Task<IEnumerable<Submission>> GetByStatusAsync(SubmissionStatus status)
    {
        var submissions = await LoadSubmissionsFromFilesAsync();
        return submissions.Where(s => s.Status == status).OrderByDescending(s => s.SubmittedAt);
    }

    public async Task<Submission> CreateAsync(Submission submission)
    {
        var submissions = await LoadSubmissionsFromFilesAsync();
        
        // Auto-generate ID if not set
        if (submission.Id == 0)
        {
            submission.Id = submissions.Any() ? submissions.Max(s => s.Id) + 1 : 1;
        }
        
        submission.SubmittedAt = DateTime.UtcNow;
        
        await SaveSubmissionsToFileAsync(submission);
        return submission;
    }

    public async Task<Submission> UpdateAsync(Submission submission)
    {
        await SaveSubmissionsToFileAsync(submission);
        return submission;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var fileName = $"submission-{id}.json";
            var filePath = Path.Combine(_submissionsPath, fileName);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _cachedSubmissions = null; // Invalidate cache
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting submission {Id}", id);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var submission = await GetByIdAsync(id);
        return submission != null;
    }
}

