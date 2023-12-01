using AutoFixture;
using AutoFixture.AutoMoq;

using SimplCommerce.Infrastructure.Helpers;
using SimplCommerce.Module.Orders.Models;

namespace UnitTests.SimplCommerce.Infrastructure.Helpers;

public static class EnumHelperTests
{
    private static IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    [Fact]
    public static void CanCall_ToDictionary()
    {
        // Arrange
        var type = typeof(OrderStatus);

        // Act
        var actual = EnumHelper.ToDictionary(type);

        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(12, actual.Count);
        Assert.Equal("OnHold", actual[OrderStatus.OnHold]);
        Assert.Equal("PendingPayment", actual[OrderStatus.PendingPayment]);
        Assert.Equal("Shipping", actual[OrderStatus.Shipping]);
    }

    [Fact]
    public static void CannotCall_ToDictionary_WithNull_Type()
    {
        Assert.Throws<ArgumentNullException>(() => EnumHelper.ToDictionary(default));
    }

    [Theory]
    [InlineData(OrderStatus.OnHold, "OnHold")]
    [InlineData(OrderStatus.PendingPayment, "PendingPayment")]
    [InlineData(OrderStatus.Shipping, "Shipping")]
    public static void CanCall_GetDisplayName(OrderStatus value, string expected)
    {
        // Act
        var actual = value.GetDisplayName();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public static void CannotCall_GetDisplayName_WithNull_Value()
    {
        Assert.Throws<ArgumentNullException>(() => default(Enum).GetDisplayName());
    }
}
