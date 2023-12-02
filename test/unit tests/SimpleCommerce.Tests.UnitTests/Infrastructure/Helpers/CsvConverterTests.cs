using AutoFixture;
using AutoFixture.AutoMoq;

using SimplCommerce.Infrastructure.Helpers;

namespace SimpleCommerce.Tests.UnitTests.Infrastructure.Helpers;

internal class CsvMockClass
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int Age { get; set; }

    public string? NullStringValue { get; set; }

    public int? NullIntValue { get; set; }
}

public class CsvConverterTests
{
    private static IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    [Fact]
    public void CanCall_ExportCsv_WithHeaderAndDelimiter()
    {
        // Arrange
        const int count = 50;

        var data = new List<CsvMockClass>();

        for (var i = 0; i < count; i++)
        {
            data.Add(new CsvMockClass()
            {
                Id = i,
                Name = $"Name_{i}",
                Age = 20 + i,
                NullStringValue = i % 2 == 0 ? $"NullStringValue_{i}" : null,
                NullIntValue = i % 3 == 0 ? i : null,
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

        var headerLine = lines[0];
        Assert.Equal(5, headerLine.Split(csvDelimiter).Length);
        Assert.Equal("Id|Name|Age|NullStringValue|NullIntValue", lines[0]);

        for (var i = 0; i < count; i++)
        {
            var line = lines[i + 1];

            var fields = line.Split(csvDelimiter);

            Assert.Equal(i.ToString(), fields[0]);
            Assert.Equal($"Name_{i}", fields[1]);
            Assert.Equal((20 + i).ToString(), fields[2]);
            Assert.Equal(i % 2 == 0 ? $"NullStringValue_{i}" : "", fields[3]);
            Assert.Equal(i % 3 == 0 ? i.ToString() : "", fields[4]);
        }
    }

    [Fact]
    public void CannotCall_ExportCsv_WithNull_Data()
    {
        Assert.Throws<ArgumentNullException>(() => CsvConverter.ExportCsv(default(IList<string>), _fixture.Create<bool>(), _fixture.Create<string>()));
    }

    [Theory]
    [InlineData(null, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    [InlineData("   ", typeof(ArgumentException))]
    public void CannotCall_ExportCsv_WithInvalid_CsvDelimiter(string delimiter, Type exceptionType)
    {
        Assert.Throws(exceptionType, () => CsvConverter.ExportCsv(_fixture.Create<IList<string>>(), _fixture.Create<bool>(), delimiter));
    }
}
