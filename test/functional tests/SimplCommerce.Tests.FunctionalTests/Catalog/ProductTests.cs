using System.Net;

using Microsoft.AspNetCore.Mvc.Testing;

namespace SimplCommerce.Tests.FunctionalTests.Catalog;

public class ProductTests : BaseFunctionalTests, IClassFixture<WebApplicationFactory<Program>>
{
    public ProductTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task ProductDetail_OkResult()
    {
        // Arrange
        var url = "/square-neck-back";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Square Neck Back - SimplCommerce</title>", responseString);
    }
}
