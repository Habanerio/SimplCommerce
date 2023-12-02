using SimplCommerce.Infrastructure.Extensions;
using SimplCommerce.Module.Tax.Models;

namespace SimpleCommerce.Tests.UnitTests.Infrastructure.Extensions;

public class DictionaryExtensionsTests
{
    [Fact]
    public void CanCall_GetOrDefault_WithNullDictionary_ShouldReturnDefault()
    {
        Dictionary<string, TaxRate>? dict = null;
        var result = dict.GetOrDefault("key");

        Assert.Null(result);
    }

    [Fact]
    public void CanCall_GetOrDefault_GetOrDefault_ShouldReturnCorrectValue()
    {
        var mockClass = new TaxRate();
        Dictionary<string, TaxRate> dict = new Dictionary<string, TaxRate> { { "key", mockClass } };
        var result = dict.GetOrDefault("key");

        Assert.NotNull(result);
        Assert.Equal(mockClass, result);
    }
}
