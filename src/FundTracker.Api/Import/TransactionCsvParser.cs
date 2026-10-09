using System.Globalization;
using FundTracker.Api.Domain;
using FundTracker.Api.Models;

namespace FundTracker.Api.Import;

public record ParsedTransaction(string ExternalId, string Isin, string InvestorName,
    TransactionType Type, decimal Amount, DateOnly TradeDate);

public static class TransactionCsvParser
{
    public const string ExpectedHeader = "ExternalId,Isin,InvestorName,Type,Amount,TradeDate";

    public static (ParsedTransaction? Row, string? Error) ParseLine(string line)
    {
        var parts = line.Split(',');
        if (parts.Length != 6)
            return (null, $"Expected 6 columns, found {parts.Length}.");

        var externalId = parts[0].Trim();
        var isin = parts[1].Trim().ToUpperInvariant();
        var investor = parts[2].Trim();
        var typeText = parts[3].Trim();
        var amountText = parts[4].Trim();
        var dateText = parts[5].Trim();

        if (externalId.Length is 0 or > 50)
            return (null, "ExternalId is missing or longer than 50 characters.");
        if (!IsinValidator.IsValid(isin))
            return (null, $"Invalid ISIN '{isin}'.");
        if (investor.Length < 2)
            return (null, "InvestorName is missing.");
        if (!Enum.TryParse<TransactionType>(typeText, ignoreCase: true, out var type) || !Enum.IsDefined(type))
            return (null, $"Unknown type '{typeText}'.");
        if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount) || amount <= 0)
            return (null, $"Invalid amount '{amountText}'.");
        if (!DateOnly.TryParseExact(dateText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return (null, $"Invalid date '{dateText}'.");

        return (new ParsedTransaction(externalId, isin, investor, type, amount, date), null);
    }
}