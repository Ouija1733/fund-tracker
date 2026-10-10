using FundTracker.Api.Domain;

namespace FundTracker.Tests;

public class IsinValidatorTests
{
    [Theory]
    [InlineData("US0378331005")]
    [InlineData("LU0274208692")]
    [InlineData("LU2000000013")]
    [InlineData(" lu2000000013 ")]
    public void IsValid_ValidIsin_ReturnsTrue(string isin) =>
        Assert.True(IsinValidator.IsValid(isin));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("LU2000000014")]   // cifra di controllo sbagliata
    [InlineData("LU200000001")]    // troppo corto
    [InlineData("1U2000000013")]   // il paese deve essere di lettere
    [InlineData("LU20000000-3")]   // carattere non valido
    public void IsValid_InvalidIsin_ReturnsFalse(string? isin) =>
        Assert.False(IsinValidator.IsValid(isin));
}