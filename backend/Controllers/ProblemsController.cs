using Microsoft.AspNetCore.Mvc;
using CodePracticePlatform.Api.Models;
using CodePracticePlatform.Api.Services;

namespace CodePracticePlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProblemsController : ControllerBase
{
    private readonly IProblemService _problemService;
    private readonly ILogger<ProblemsController> _logger;

    public ProblemsController(IProblemService problemService, ILogger<ProblemsController> logger)
    {
        _problemService = problemService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Problem>>> GetProblems(
        [FromQuery] Difficulty? difficulty,
        [FromQuery] FeatureType? featureType)
    {
        try
        {
            var problems = await _problemService.GetProblemsAsync(difficulty, featureType);
            return Ok(problems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting problems");
            return StatusCode(500, "An error occurred while retrieving problems");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Problem>> GetProblem(int id)
    {
        try
        {
            var problem = await _problemService.GetProblemByIdAsync(id);
            if (problem == null)
            {
                return NotFound($"Problem with id {id} not found");
            }

            return Ok(problem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting problem {ProblemId}", id);
            return StatusCode(500, "An error occurred while retrieving the problem");
        }
    }


    [HttpPost("reload")]
    public ActionResult ReloadProblems()
    {
        try
        {
            _problemService.ReloadProblems();
            return Ok(new { message = "Problems cache cleared. Problems will be reloaded on next request." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reloading problems");
            return StatusCode(500, "An error occurred while reloading problems");
        }
    }
}

