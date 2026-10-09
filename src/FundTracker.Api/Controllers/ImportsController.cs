using FundTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FundTracker.Api.Controllers;

[ApiController]
[Route("api/imports")]
public class ImportsController(IImportService imports) : ControllerBase
{
    [HttpPost("transactions")]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<ImportResultDto>> ImportTransactions(IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0)
            return BadRequest(new ProblemDetails { Title = "The file is empty." });

        await using var stream = file.OpenReadStream();
        return await imports.ImportTransactionsAsync(stream, ct);
    }
}