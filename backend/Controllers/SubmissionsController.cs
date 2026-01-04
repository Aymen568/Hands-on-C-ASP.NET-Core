using Microsoft.AspNetCore.Mvc;
using CodePracticePlatform.Api.Models;
using CodePracticePlatform.Api.Models.DTOs;
using CodePracticePlatform.Api.Services;

namespace CodePracticePlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;
    private readonly ILogger<SubmissionsController> _logger;

    public SubmissionsController(
        ISubmissionService submissionService,
        ILogger<SubmissionsController> logger)
    {
        _submissionService = submissionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Submission>>> GetSubmissions(
        [FromQuery] int? problemId,
        [FromQuery] int? userId)
    {
        try
        {
            var submissions = await _submissionService.GetSubmissionsAsync(problemId, userId);
            return Ok(submissions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting submissions");
            return StatusCode(500, "An error occurred while retrieving submissions");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Submission>> GetSubmission(int id)
    {
        try
        {
            var submission = await _submissionService.GetSubmissionByIdAsync(id);
            if (submission == null)
            {
                return NotFound($"Submission with id {id} not found");
            }

            return Ok(submission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting submission {SubmissionId}", id);
            return StatusCode(500, "An error occurred while retrieving the submission");
        }
    }


    [HttpPost]
    public async Task<ActionResult<Submission>> CreateSubmission([FromBody] CreateSubmissionDto dto)
    {
        try
        {
            var submission = await _submissionService.CreateSubmissionAsync(
                dto.ProblemId,
                userId: 1, // For now, single user
                dto.GitRepositoryUrl
            );

            return CreatedAtAction(nameof(GetSubmission), new { id = submission.Id }, submission);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating submission");
            return StatusCode(500, "An error occurred while creating the submission");
        }
    }
}

