using FundTracker.Api.Import;
using FundTracker.Api.Models;

namespace FundTracker.Tests;

public class TransactionCsvParserTests
{
    [Fact]
    public void ParseLine_ValidLine_ReturnsTransaction()
    {
        // Arrange
        var line = "TX-1,LU2000000013,Anna Bianchi,Subscription,12500.50,2026-09-15";

        // Act
        var (row, error) = TransactionCsvParser.ParseLine(line);

        // Assert
        Assert.Null(error);
        Assert.NotNull(row);
        Assert.Equal("TX-1", row.ExternalId);
        Assert.Equal(TransactionType.Subscription, row.Type);
        Assert.Equal(12500.50m, row.Amount);
        Assert.Equal(new DateOnly(2026, 9, 15), row.TradeDate);
    }

    [Theory]
    [InlineData("TX-1,LU2000000013,Anna,Subscription,12500", "Expected 6 columns")]
    [InlineData("TX-1,LU2000000014,Anna,Subscription,100,2026-09-15", "Invalid ISIN")]
    [InlineData("TX-1,LU2000000013,Anna,Transfer,100,2026-09-15", "Unknown type")]
    [InlineData("TX-1,LU2000000013,Anna,Subscription,-5,2026-09-15", "Invalid amount")]
    [InlineData("TX-1,LU2000000013,Anna,Subscription,100,15/09/2026", "Invalid date")]
    public void ParseLine_InvalidLine_ReturnsError(string line, string expectedError)
    {
        var (row, error) = TransactionCsvParser.ParseLine(line);

        Assert.Null(row);
        Assert.Contains(expectedError, error);
    }
}