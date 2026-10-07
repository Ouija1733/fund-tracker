using FundTracker.Api.Data;
using FundTracker.Api.Dtos;
using FundTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FundTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FundsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<FundDto>> GetAll() =>
        await db.Funds
            .AsNoTracking()
            .OrderBy(f => f.Name)
            .Select(f => new FundDto(f.Id, f.Name, f.Isin, f.Currency))
            .ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FundDto>> GetById(int id)
    {
        var fund = await db.Funds
            .AsNoTracking()
            .Where(f => f.Id == id)
            .Select(f => new FundDto(f.Id, f.Name, f.Isin, f.Currency))
            .FirstOrDefaultAsync();

        return fund is null ? NotFound() : fund;
    }

    [HttpPost]
    public async Task<ActionResult<FundDto>> Create(CreateFundRequest request)
    {
        if (await db.Funds.AnyAsync(f => f.Isin == request.Isin))
            return Conflict(new ProblemDetails { Title = "A fund with this ISIN already exists." });

        var fund = new Fund
        {
            Name = request.Name,
            Isin = request.Isin,
            Currency = request.Currency.ToUpperInvariant()
        };
        db.Funds.Add(fund);
        await db.SaveChangesAsync();

        var dto = new FundDto(fund.Id, fund.Name, fund.Isin, fund.Currency);
        return CreatedAtAction(nameof(GetById), new { id = fund.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateFundRequest request)
    {
        var fund = await db.Funds.FindAsync(id);
        if (fund is null) return NotFound();

        fund.Name = request.Name;
        fund.Currency = request.Currency.ToUpperInvariant();
        await db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await db.Funds.Where(f => f.Id == id).ExecuteDeleteAsync();
        return deleted == 0 ? NotFound() : NoContent();
    }
}