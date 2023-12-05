using System.Globalization;

using SimplCommerce.Infrastructure.Helpers;

namespace SimplCommerce.Tests.UnitTests.Infrastructure.Helpers;

public static class CurrencyHelperTests
{
    [Theory]
    [InlineData("en-CA", false)]
    [InlineData("fr-CA", false)]
    [InlineData("ja-JP", true)]
    [InlineData("ko-KR", true)]
    [InlineData("en-US", false)]
    public static void CanCall_IsZeroDecimalCurrencies(string cultureCode, bool expected)
    {
        // Arrange
        Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureCode);

        // Act
        var actual = CurrencyHelper.IsZeroDecimalCurrencies(CultureInfo.CurrentCulture);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public static void CannotCall_IsZeroDecimalCurrencies_WithNull_CultureInfo()
    {
        Assert.Throws<ArgumentNullException>(() => CurrencyHelper.IsZeroDecimalCurrencies(default));
    }
}
