using FundTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FundTracker.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Fund> Funds => Set<Fund>();
    public DbSet<NavPrice> NavPrices => Set<NavPrice>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fund>(e =>
        {
            e.Property(f => f.Name).HasMaxLength(200);
            e.Property(f => f.Isin).HasMaxLength(12);
            e.Property(f => f.Currency).HasMaxLength(3);
            e.HasIndex(f => f.Isin).IsUnique();
        });

        modelBuilder.Entity<NavPrice>(e =>
        {
            e.Property(n => n.Value).HasPrecision(18, 4);
            e.HasIndex(n => new { n.FundId, n.Date }).IsUnique();
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.Property(t => t.ExternalId).HasMaxLength(50);
            e.Property(t => t.InvestorName).HasMaxLength(200);
            e.Property(t => t.Amount).HasPrecision(18, 2);
            e.Property(t => t.Type).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(t => t.ExternalId).IsUnique();
            e.HasIndex(t => new { t.FundId, t.TradeDate });
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fund>().HasData(
            new Fund { Id = 1, Name = "Euro Equity Growth", Isin = "LU2000000013", Currency = "EUR" },
            new Fund { Id = 2, Name = "Global Bond Income", Isin = "LU2000000021", Currency = "EUR" },
            new Fund { Id = 3, Name = "US Tech Leaders", Isin = "LU2000000039", Currency = "USD" });

        var navs = new List<NavPrice>();
        var id = 1;
        foreach (var (fundId, start) in new[] { (1, 100m), (2, 250m), (3, 80m) })
        {
            for (var day = 0; day < 10; day++)
            {
                navs.Add(new NavPrice
                {
                    Id = id++,
                    FundId = fundId,
                    Date = new DateOnly(2026, 9, 21).AddDays(day),
                    Value = start + day * 0.75m
                });
            }
        }
        modelBuilder.Entity<NavPrice>().HasData(navs);
    }
}