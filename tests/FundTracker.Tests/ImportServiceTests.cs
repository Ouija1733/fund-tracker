using System.Text;
using FundTracker.Api.Data;
using FundTracker.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace FundTracker.Tests;

public class ImportServiceTests
{
    private const string Csv = """
        ExternalId,Isin,InvestorName,Type,Amount,TradeDate
        TX-1,LU2000000013,Anna Bianchi,Subscription,100,2026-09-15
        TX-2,LU2000000021,Luca Verdi,Redemption,50,2026-09-16
        TX-3,LU2000000014,Bad Isin,Subscription,10,2026-09-16
        """;

    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(options);
        db.Database.EnsureCreated(); // inserisce i tre fondi del seed
        return db;
    }

    private static Stream ToStream(string text) => new MemoryStream(Encoding.UTF8.GetBytes(text));

    [Fact]
    public async Task Import_ValidAndInvalidRows_ImportsValidOnes()
    {
        using var db = CreateDb();
        var service = new ImportService(db, NullLogger<ImportService>.Instance);

        var result = await service.ImportTransactionsAsync(ToStream(Csv), CancellationToken.None);

        Assert.Equal(3, result.TotalRows);
        Assert.Equal(2, result.Imported);
        Assert.Equal(1, result.ErrorCount);
        Assert.Equal(4, result.FirstErrors[0].Line);
    }

    [Fact]
    public async Task Import_SameFileTwice_DoesNotDuplicate()
    {
        using var db = CreateDb();
        var service = new ImportService(db, NullLogger<ImportService>.Instance);

        await service.ImportTransactionsAsync(ToStream(Csv), CancellationToken.None);
        var second = await service.ImportTransactionsAsync(ToStream(Csv), CancellationToken.None);

        Assert.Equal(0, second.Imported);
        Assert.Equal(2, second.Duplicates);
        Assert.Equal(2, await db.Transactions.CountAsync());
    }
}