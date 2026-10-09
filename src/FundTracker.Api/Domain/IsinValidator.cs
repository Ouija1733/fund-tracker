namespace FundTracker.Api.Domain;

public static class IsinValidator
{
    public static bool IsValid(string? isin)
    {
        if (string.IsNullOrWhiteSpace(isin)) return false;
        isin = isin.Trim().ToUpperInvariant();

        if (isin.Length != 12) return false;
        if (!IsLetter(isin[0]) || !IsLetter(isin[1]) || !char.IsAsciiDigit(isin[11])) return false;
        if (!isin.All(c => IsLetter(c) || char.IsAsciiDigit(c))) return false;

        // Le lettere diventano numeri (A=10 ... Z=35), poi si applica Luhn
        var digits = string.Concat(isin[..11].Select(c => IsLetter(c) ? (c - 'A' + 10).ToString() : c.ToString()));

        var sum = 0;
        var doubleIt = true;
        for (var i = digits.Length - 1; i >= 0; i--)
        {
            var d = digits[i] - '0';
            if (doubleIt)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            doubleIt = !doubleIt;
        }

        var checkDigit = (10 - sum % 10) % 10;
        return checkDigit == isin[11] - '0';
    }

    private static bool IsLetter(char c) => c is >= 'A' and <= 'Z';
}