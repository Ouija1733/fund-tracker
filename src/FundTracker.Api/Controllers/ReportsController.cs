using FundTracker.Api.Dtos;
using FundTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FundTracker.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController(ITransactionService transactions) : ControllerBase
{
    [HttpGet("fund-flows")]
    public async Task<ActionResult<IReadOnlyList<FundFlowDto>>> FundFlows(
        [FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken ct)
    {
        if (from > to)
            return BadRequest(new ProblemDetails { Title = "'from' must be before 'to'." });

        return Ok(await transactions.GetFlowsAsync(from, to, ct));
    }
}