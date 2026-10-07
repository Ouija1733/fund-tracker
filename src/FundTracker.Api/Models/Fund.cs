namespace FundTracker.Api.Models;

public class Fund
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Isin { get; set; } = string.Empty;
    public string Currency { get; set; } = "EUR";

    public List<NavPrice> NavPrices { get; set; } = [];
    public List<Transaction> Transactions { get; set; } = [];
}