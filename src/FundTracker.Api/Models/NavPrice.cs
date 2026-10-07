namespace FundTracker.Api.Models;

public class NavPrice
{
    public int Id { get; set; }
    public int FundId { get; set; }
    public Fund Fund { get; set; } = null!;
    public DateOnly Date { get; set; }
    public decimal Value { get; set; }
}