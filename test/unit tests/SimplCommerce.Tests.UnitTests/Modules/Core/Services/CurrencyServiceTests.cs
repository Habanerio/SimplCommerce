using System.Globalization;

using Microsoft.Extensions.Configuration;

using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Tests.UnitTests.Modules.Core.Services;

public class CurrencyServiceTests : UnitTestBase
{
    [Fact]
    public void Can_Construct()
    {
        // Arrange
        var configSettings = GetCurrencyConfigSettings("en-US", 2);

        var config = GetConfiguration(configSettings);

        // Act
        var instance = new CurrencyService(config);

        // Assert
        Assert.NotNull(instance);
    }

    [Fact]
    public void Cannot_Construct_WithNull_Config()
    {
        Assert.Throws<ArgumentNullException>(() => new CurrencyService(default(IConfiguration)));
    }

    [Theory]
    [InlineData("en-US", 2, "$")]
    [InlineData("en-CA", 4, "$")]
    [InlineData("ko-KR", 6, "₩")]
    [InlineData("ja-JP", 1, "\uffe5")] //¥
    public void CanCall_FormatCurrency(string cultureCode, int decimalPlaces, string dollarSign)
    {
        // Arrange
        var configSettings = GetCurrencyConfigSettings(cultureCode, decimalPlaces);

        var config = GetConfiguration(configSettings);

        var testClass = new CurrencyService(config);

        var value = 9999.9999999m;

        var numberFormat = new NumberFormatInfo
        {
            CurrencyDecimalDigits = decimalPlaces,
            CurrencySymbol = dollarSign
        };

        // Note: 9999.9999999 becomes $10,000.0000 when decimalPlaces is 4.
        var expected = value.ToString("C", numberFormat);

        // Act
        var actual = testClass.FormatCurrency(value);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CanGet_CurrencyCulture()
    {
        // Arrange
        var cultureCode = "en-US";
        var decimalPlaces = 2;

        var configSettings = GetCurrencyConfigSettings(cultureCode, decimalPlaces);

        var config = GetConfiguration(configSettings);
        var testClass = new CurrencyService(config);

        // Act
        var actual = testClass.CurrencyCulture;

        // Assert
        Assert.IsType<CultureInfo>(actual);
        Assert.Equal(cultureCode, actual.Name);
    }

    private Dictionary<string, string?> GetCurrencyConfigSettings(string culture, int decimalPlaces)
    {
        var configSettings = new Dictionary<string, string?>
        {
            {"Global.CurrencyCulture", culture},
            {"Global.CurrencyDecimalPlace", decimalPlaces.ToString()}
        };

        return configSettings;
    }
}
