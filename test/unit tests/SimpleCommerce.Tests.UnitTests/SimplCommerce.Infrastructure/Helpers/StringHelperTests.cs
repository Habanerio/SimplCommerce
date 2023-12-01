using AutoFixture;
using AutoFixture.AutoMoq;

using SimplCommerce.Infrastructure.Helpers;

namespace UnitTests.SimplCommerce.Infrastructure.Helpers;

public static class StringHelperTests
{
    private static IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    [Theory]
    [InlineData("friendly url", "friendly-url")]
    [InlineData("friendly123url", "friendly123url")]
    [InlineData("friendly123  url", "friendly123-url")]
    [InlineData("friendly123 123 url", "friendly123-123-url")]
    [InlineData("friendly    url", "friendly-url")]
    [InlineData("friendly--url", "friendly-url")]
    [InlineData("friendly---url", "friendly-url")]
    [InlineData("friendly---123---url", "friendly-123-url")]
    [InlineData("ıłŁđß øÞx", "illdss-othx")]
    public static void CanCall_ToUrlFriendly(string value, string expected)
    {
        // Act
        var actual = value.ToUrlFriendly();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public static void CannotCall_ToUrlFriendly_WithInvalid_Name(string value)
    {
        var actual = value.ToUrlFriendly();

        Assert.True(Guid.TryParse(actual, out var guid));
        Assert.NotEqual(Guid.Empty, guid);
    }

    [Theory]
    [InlineData("diakritikós", "diakritikos")]
    [InlineData("áéíóúýčďěňřšťžů", "aeiouycdenrstzu")]
    public static void CanCall_RemoveDiacritics(string value, string expected)
    {
        // Act
        var actual = StringHelper.RemoveDiacritics(value);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public static void CannotCall_RemoveDiacritics_WithInvalid_Text(string value)
    {
        var actual = StringHelper.RemoveDiacritics(value);

        Assert.Equal(string.Empty, actual);
    }

    [Theory]
    [InlineData("friendly@url", "friendlyurl")]
    [InlineData("friendly! url!", "friendlyurl")]
    public static void CanCall_Strip_With_SubjectAndPredicate(string value, string expected)
    {
        // Act
        var actual = value.Strip(c =>
            c != '-'
            && c != '_'
            && !char.IsLetter(c)
            && !char.IsDigit(c)
        );

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public static void CannotCall_Strip_With_SubjectAndPredicate_WithNull_Predicate()
    {
        Assert.Throws<ArgumentNullException>(() => _fixture.Create<string>().Strip(default(Func<char, bool>)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public static void CanCall_Strip_With_SubjectAndPredicate_WithInvalid_Subject(string value)
    {
        var actual = value.Strip(x => _fixture.Create<bool>());

        Assert.Equal(string.Empty, actual);
    }
}
