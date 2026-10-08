using FundTracker.Api.Data;
using FundTracker.Api.Dtos;
using FundTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FundTracker.Api.Services;

public interface ITransactionService
{
    Task<PagedResult<TransactionDto>?> GetByFundAsync(int fundId, int page, int pageSize, CancellationToken ct);
    Task<TransactionDto?> CreateAsync(int fundId, CreateTransactionRequest request, CancellationToken ct);
    Task<IReadOnlyList<FundFlowDto>> GetFlowsAsync(DateOnly from, DateOnly to, CancellationToken ct);
}

public class TransactionService(AppDbContext db) : ITransactionService
{
    public async Task<PagedResult<TransactionDto>?> GetByFundAsync(int fundId, int page, int pageSize, CancellationToken ct)
    {
        if (!await db.Funds.AnyAsync(f => f.Id == fundId, ct)) return null;

        var query = db.Transactions.AsNoTracking().Where(t => t.FundId == fundId);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(t => t.TradeDate).ThenByDescending(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransactionDto(t.Id, t.ExternalId, t.InvestorName, t.Type, t.Amount, t.TradeDate))
            .ToListAsync(ct);

        return new PagedResult<TransactionDto>(items, page, pageSize, total);
    }

    public async Task<TransactionDto?> CreateAsync(int fundId, CreateTransactionRequest request, CancellationToken ct)
    {
        if (!await db.Funds.AnyAsync(f => f.Id == fundId, ct)) return null;

        var transaction = new Transaction
        {
            ExternalId = $"API-{Guid.NewGuid():N}",
            FundId = fundId,
            InvestorName = request.InvestorName.Trim(),
            Type = request.Type,
            Amount = request.Amount,
            TradeDate = request.TradeDate
        };
        db.Transactions.Add(transaction);
        await db.SaveChangesAsync(ct);

        return new TransactionDto(transaction.Id, transaction.ExternalId, transaction.InvestorName,
            transaction.Type, transaction.Amount, transaction.TradeDate);
    }

    public async Task<IReadOnlyList<FundFlowDto>> GetFlowsAsync(DateOnly from, DateOnly to, CancellationToken ct) =>
    await db.Transactions
        .AsNoTracking()
        .Where(t => t.TradeDate >= from && t.TradeDate <= to)
        .GroupBy(t => new { t.FundId, t.Fund.Name })
        .Select(g => new
        {
            g.Key.FundId,
            g.Key.Name,
            Subscriptions = g.Sum(t => t.Type == TransactionType.Subscription ? t.Amount : 0),
            Redemptions = g.Sum(t => t.Type == TransactionType.Redemption ? t.Amount : 0),
            Operations = g.Count()
        })
        .OrderByDescending(x => x.Subscriptions - x.Redemptions)
        .Select(x => new FundFlowDto(x.FundId, x.Name, x.Subscriptions, x.Redemptions,
            x.Subscriptions - x.Redemptions, x.Operations))
        .ToListAsync(ct);
}