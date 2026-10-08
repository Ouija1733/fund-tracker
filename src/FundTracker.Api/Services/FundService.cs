using FundTracker.Api.Data;
using FundTracker.Api.Dtos;
using FundTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FundTracker.Api.Services;

public interface IFundService
{
    Task<PagedResult<FundDto>> SearchAsync(string? search, int page, int pageSize, CancellationToken ct);
    Task<FundDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> IsinExistsAsync(string isin, CancellationToken ct);
    Task<FundDto> CreateAsync(CreateFundRequest request, CancellationToken ct);
    Task<bool> UpdateAsync(int id, UpdateFundRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}

public class FundService(AppDbContext db) : IFundService
{
    // Proiezione riusabile: entità -> DTO, con l'ultimo NAV calcolato in SQL
    private static IQueryable<FundDto> ToDto(IQueryable<Fund> query) =>
        query.Select(f => new FundDto(
            f.Id, f.Name, f.Isin, f.Currency,
            f.NavPrices.OrderByDescending(n => n.Date).Select(n => (decimal?)n.Value).FirstOrDefault(),
            f.NavPrices.OrderByDescending(n => n.Date).Select(n => (DateOnly?)n.Date).FirstOrDefault()));

    public async Task<PagedResult<FundDto>> SearchAsync(string? search, int page, int pageSize, CancellationToken ct)
    {
        var query = db.Funds.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(f => f.Name.Contains(term) || f.Isin.Contains(term));
        }

        var total = await query.CountAsync(ct);
        var items = await ToDto(query
                .OrderBy(f => f.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize))
            .ToListAsync(ct);

        return new PagedResult<FundDto>(items, page, pageSize, total);
    }

    public Task<FundDto?> GetByIdAsync(int id, CancellationToken ct) =>
        ToDto(db.Funds.AsNoTracking().Where(f => f.Id == id)).FirstOrDefaultAsync(ct);

    public Task<bool> IsinExistsAsync(string isin, CancellationToken ct) =>
        db.Funds.AnyAsync(f => f.Isin == isin, ct);

    public async Task<FundDto> CreateAsync(CreateFundRequest request, CancellationToken ct)
    {
        var fund = new Fund
        {
            Name = request.Name.Trim(),
            Isin = request.Isin,
            Currency = request.Currency.ToUpperInvariant()
        };
        db.Funds.Add(fund);
        await db.SaveChangesAsync(ct);
        return new FundDto(fund.Id, fund.Name, fund.Isin, fund.Currency, null, null);
    }

    public async Task<bool> UpdateAsync(int id, UpdateFundRequest request, CancellationToken ct)
    {
        var fund = await db.Funds.FindAsync([id], ct);
        if (fund is null) return false;

        fund.Name = request.Name.Trim();
        fund.Currency = request.Currency.ToUpperInvariant();
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct) =>
        await db.Funds.Where(f => f.Id == id).ExecuteDeleteAsync(ct) > 0;
}