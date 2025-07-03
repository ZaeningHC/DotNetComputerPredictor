using ComputerPredictor.Models;
using ComputerPredictor.Services;
using Microsoft.AspNetCore.Mvc;

namespace ComputerPredictor.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PredictionController : ControllerBase
{
    private readonly PredictionService _predictionService;

    public PredictionController(PredictionService predictionService)
    {
        _predictionService = predictionService;
    }

    [HttpPost]
    public async Task<IActionResult> Predict([FromBody] PredictionRequest request)
    {
        if (request == null || request.Features == null)
        {
            return BadRequest(new { Error = "Request or Features cannot be null." });
        }

        try
        {
            var result = await _predictionService.PredictAsync(request.Target, request.Features);
            return Ok(new { Prediction = result });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}
