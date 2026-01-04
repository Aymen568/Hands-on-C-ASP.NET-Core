using Microsoft.AspNetCore.Mvc;
using CodePracticePlatform.Api.Models;
using CodePracticePlatform.Api.Services;

namespace CodePracticePlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvaluationsController : ControllerBase
{
    private readonly IEvaluationService _evaluationService;
    private readonly ILogger<EvaluationsController> _logger;

    public EvaluationsController(IEvaluationService evaluationService, ILogger<EvaluationsController> logger)
    {
        _evaluationService = evaluationService;
        _logger = logger;
    }

    [HttpGet("submission/{submissionId}")]
    public async Task<ActionResult<Evaluation>> GetEvaluationBySubmissionId(int submissionId)
    {
        try
        {
            var evaluation = await _evaluationService.GetEvaluationBySubmissionIdAsync(submissionId);
            if (evaluation == null)
            {
                return NotFound($"Evaluation for submission {submissionId} not found");
            }

            return Ok(evaluation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting evaluation for submission {SubmissionId}", submissionId);
            return StatusCode(500, "An error occurred while retrieving the evaluation");
        }
    }

    [HttpPost("submission/{submissionId}")]
    public async Task<ActionResult<Evaluation>> EvaluateSubmission(int submissionId)
    {
        try
        {
            var evaluation = await _evaluationService.EvaluateSubmissionAsync(submissionId);
            return Ok(evaluation);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating submission {SubmissionId}", submissionId);
            return StatusCode(500, "An error occurred while evaluating the submission");
        }
    }
}

