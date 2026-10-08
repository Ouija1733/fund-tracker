using System.ComponentModel.DataAnnotations;
using FundTracker.Api.Models;

namespace FundTracker.Api.Dtos;

public record TransactionDto(int Id, string ExternalId, string InvestorName,
    TransactionType Type, decimal Amount, DateOnly TradeDate);

public record CreateTransactionRequest(
    [Required, StringLength(200, MinimumLength = 2)] string InvestorName,
    TransactionType Type,
    [Range(typeof(decimal), "0.01", "1000000000",
    ParseLimitsInInvariantCulture = true,
    ConvertValueInInvariantCulture = true)] decimal Amount,
    DateOnly TradeDate);
public record FundFlowDto(int FundId, string FundName, decimal Subscriptions,
    decimal Redemptions, decimal NetFlow, int Operations);