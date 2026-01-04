using CodePracticePlatform.Api.Models;
using CodePracticePlatform.Api.Repositories;
using CodePracticePlatform.Api.Services;
using System.Linq;

namespace CodePracticePlatform.Api.Services;

/// <summary>
/// Service for submission management (OOP - Service Layer)
/// </summary>
public class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IProblemService _problemService;
    private readonly IGitService _gitService;
    private readonly IEvaluationService _evaluationService;
    private readonly ILogger<SubmissionService> _logger;

    public SubmissionService(
        ISubmissionRepository submissionRepository,
        IProblemService problemService,
        IGitService gitService,
        IEvaluationService evaluationService,
        ILogger<SubmissionService> logger)
    {
        _submissionRepository = submissionRepository;
        _problemService = problemService;
        _gitService = gitService;
        _evaluationService = evaluationService;
        _logger = logger;
    }

    public async Task<Submission?> GetSubmissionByIdAsync(int id)
    {
        return await _submissionRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Submission>> GetAllSubmissionsAsync()
    {
        return await _submissionRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Submission>> GetSubmissionsByProblemIdAsync(int problemId)
    {
        return await _submissionRepository.GetByProblemIdAsync(problemId);
    }

    public async Task<IEnumerable<Submission>> GetSubmissionsByUserIdAsync(int userId)
    {
        return await _submissionRepository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<Submission>> GetSubmissionsAsync(int? problemId, int? userId)
    {
        var submissions = await _submissionRepository.GetAllAsync();

        if (problemId.HasValue)
        {
            submissions = submissions.Where(s => s.ProblemId == problemId.Value);
        }

        if (userId.HasValue)
        {
            submissions = submissions.Where(s => s.UserId == userId.Value);
        }

        return submissions;
    }

    public async Task<Submission> CreateSubmissionAsync(int problemId, int userId, string gitRepositoryUrl)
    {
        // Validate problem exists (using file-based ProblemService)
        var problem = await _problemService.GetProblemByIdAsync(problemId);
        if (problem == null)
        {
            throw new ArgumentException($"Problem with id {problemId} not found");
        }

        // Validate Git repository URL
        var isValid = await _gitService.ValidateRepositoryUrlAsync(gitRepositoryUrl);
        if (!isValid)
        {
            throw new ArgumentException("Invalid Git repository URL");
        }

        var submission = new Submission
        {
            ProblemId = problemId,
            UserId = userId,
            GitRepositoryUrl = gitRepositoryUrl,
            Status = SubmissionStatus.Pending,
            SubmittedAt = DateTime.UtcNow
        };

        submission = await _submissionRepository.CreateAsync(submission);

        // Kick off evaluation asynchronously; controller stays thin
        _ = Task.Run(async () =>
        {
            try
            {
                await _evaluationService.EvaluateSubmissionAsync(submission.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating submission {SubmissionId}", submission.Id);
            }
        });

        return submission;
    }

    public async Task<Submission> UpdateSubmissionStatusAsync(int submissionId, SubmissionStatus status)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId);
        if (submission == null)
        {
            throw new ArgumentException($"Submission with id {submissionId} not found");
        }

        submission.UpdateStatus(status);
        return await _submissionRepository.UpdateAsync(submission);
    }

    public async Task<bool> DeleteSubmissionAsync(int id)
    {
        return await _submissionRepository.DeleteAsync(id);
    }
}

