using AutoFixture;
using AutoFixture.AutoMoq;

using SimplCommerce.Infrastructure.Helpers;
using SimplCommerce.Module.Tax.Areas.Tax.ViewModels;

using T = System.String;

namespace UnitTests.SimplCommerce.Infrastructure.Helpers;

public static class CsvConverterTests
{
    private static IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    [Fact]
    public static void CanCall_ExportCsv_WithHeaderAndDelimiter()
    {
        // Arrange
        const int count = 50;

        var data = new List<TaxRateImport>();

        for (var i = 0; i < count; i++)
        {
            data.Add(new TaxRateImport()
            {
                TaxClassId = i,
                CountryId = $"CountryId_{i}",
                Rate = i,
                StateOrProvinceName = $"StateOrProvince_{i}",
                ZipCode = $"ZipCode_{i}",
            });
        }

        var includeHeader = true;
        var csvDelimiter = "|";

        // Act
        var actual = CsvConverter.ExportCsv(data, includeHeader, csvDelimiter);

        // Assert
        Assert.NotNull(actual);

        var lines = actual.Split(Environment.NewLine);

        // 1 (Header) + 50 (Data) + 1 (Empty Line?) = 52
        Assert.Equal(52, lines.Length);
        Assert.Equal("TaxClassId|CountryId|StateOrProvinceName|ZipCode|Rate", lines[0]);

        for (var i = 0; i < count; i++)
        {
            var line = lines[i + 1];

            var fields = line.Split(csvDelimiter);

            Assert.Equal(5, fields.Length);

            Assert.Equal(i, int.Parse(fields[0]));
            Assert.Equal($"CountryId_{i}", fields[1]);
            Assert.Equal($"StateOrProvince_{i}", fields[2]);
            Assert.Equal($"ZipCode_{i}", fields[3]);
            Assert.Equal(i, int.Parse(fields[4]));
        }
    }

    [Fact]
    public static void CannotCall_ExportCsv_WithNull_Data()
    {
        Assert.Throws<ArgumentNullException>(() => CsvConverter.ExportCsv(default(IList<T>), _fixture.Create<bool>(), _fixture.Create<string>()));
    }

    [Theory]
    [InlineData(null, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    [InlineData("   ", typeof(ArgumentException))]
    public static void CannotCall_ExportCsv_WithInvalid_CsvDelimiter(string value, Type exceptionType)
    {
        Assert.Throws(exceptionType, () => CsvConverter.ExportCsv(_fixture.Create<IList<T>>(), _fixture.Create<bool>(), value));
    }
}
