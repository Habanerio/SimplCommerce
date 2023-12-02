using AutoFixture;
using AutoFixture.AutoMoq;

using SimplCommerce.Infrastructure.Helpers;
using SimplCommerce.Module.Orders.Models;

namespace SimpleCommerce.Tests.UnitTests.Infrastructure.Helpers;

public class EnumHelperTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    [Fact]
    public void CanCall_ToDictionary()
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
    public void CannotCall_ToDictionary_WithNull_Type()
    {
        Assert.Throws<ArgumentNullException>(() => EnumHelper.ToDictionary(default));
    }

    [Theory]
    [InlineData(OrderStatus.OnHold, "OnHold")]
    [InlineData(OrderStatus.PendingPayment, "PendingPayment")]
    [InlineData(OrderStatus.Shipping, "Shipping")]
    public void CanCall_GetDisplayName(OrderStatus value, string expected)
    {
        // Act
        var actual = value.GetDisplayName();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CannotCall_GetDisplayName_WithNull_Value()
    {
        Assert.Throws<ArgumentNullException>(() => default(Enum).GetDisplayName());
    }
}
