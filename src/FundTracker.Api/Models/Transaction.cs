namespace FundTracker.Api.Models;

public enum TransactionType
{
    Subscription,
    Redemption
}

public class Transaction
{
    public int Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public int FundId { get; set; }
    public Fund Fund { get; set; } = null!;
    public string InvestorName { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateOnly TradeDate { get; set; }
}