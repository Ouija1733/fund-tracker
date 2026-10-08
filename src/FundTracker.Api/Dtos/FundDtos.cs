using System.ComponentModel.DataAnnotations;

namespace FundTracker.Api.Dtos;

public record FundDto(int Id, string Name, string Isin, string Currency,
    decimal? LatestNav, DateOnly? LatestNavDate);

public record CreateFundRequest(
    [Required, StringLength(200, MinimumLength = 2)] string Name,
    [Required, RegularExpression("^[A-Z]{2}[A-Z0-9]{9}[0-9]$",
        ErrorMessage = "ISIN must be 2 letters, 9 letters or digits and 1 check digit.")] string Isin,
    [Required, StringLength(3, MinimumLength = 3)] string Currency);

public record UpdateFundRequest(
    [Required, StringLength(200, MinimumLength = 2)] string Name,
    [Required, StringLength(3, MinimumLength = 3)] string Currency);

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);