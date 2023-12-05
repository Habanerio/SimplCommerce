using System.Net;

using Microsoft.AspNetCore.Mvc.Testing;

namespace SimplCommerce.Tests.FunctionalTests.Catalog;

public class CmsTests : BaseFunctionalTests, IClassFixture<WebApplicationFactory<Program>>
{
    public CmsTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task HomePage_OkResult()
    {
        // Arrange
        var url = "/";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //TODO: May want to look at this?
        Assert.Contains("<title>Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString - SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task AboutUs_OkResult()
    {
        // Arrange
        var url = "/about-us";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>About Us - SimplCommerce</title>", responseString);
        Assert.Contains("<h1>About Us</h1>", responseString);
    }
}
