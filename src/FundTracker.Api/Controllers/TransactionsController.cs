using System.ComponentModel.DataAnnotations;
using FundTracker.Api.Dtos;
using FundTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FundTracker.Api.Controllers;

[ApiController]
[Route("api/funds/{fundId:int}/transactions")]
public class TransactionsController(ITransactionService transactions) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<TransactionDto>>> GetByFund(
        int fundId,
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await transactions.GetByFundAsync(fundId, page, pageSize, ct);
        return result is null ? NotFound() : result;
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create(int fundId, CreateTransactionRequest request, CancellationToken ct)
    {
        var created = await transactions.CreateAsync(fundId, request, ct);
        return created is null ? NotFound() : Created($"/api/funds/{fundId}/transactions/{created.Id}", created);
    }
}