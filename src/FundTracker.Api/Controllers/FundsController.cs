using System.ComponentModel.DataAnnotations;
using FundTracker.Api.Dtos;
using FundTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FundTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FundsController(IFundService funds) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<FundDto>> Search(
        [FromQuery] string? search,
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 20,
        CancellationToken ct = default) =>
        funds.SearchAsync(search, page, pageSize, ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FundDto>> GetById(int id, CancellationToken ct)
    {
        var fund = await funds.GetByIdAsync(id, ct);
        return fund is null ? NotFound() : fund;
    }

    [HttpPost]
    public async Task<ActionResult<FundDto>> Create(CreateFundRequest request, CancellationToken ct)
    {
        if (await funds.IsinExistsAsync(request.Isin, ct))
            return Conflict(new ProblemDetails { Title = "A fund with this ISIN already exists." });

        var fund = await funds.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = fund.Id }, fund);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateFundRequest request, CancellationToken ct) =>
        await funds.UpdateAsync(id, request, ct) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
        await funds.DeleteAsync(id, ct) ? NoContent() : NotFound();
}