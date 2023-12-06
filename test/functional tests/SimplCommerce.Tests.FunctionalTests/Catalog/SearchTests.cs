using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SimplCommerce.Tests.FunctionalTests.Catalog;

public class SearchTests : BaseFunctionalTests, IClassFixture<WebApplicationFactory<Program>>
{
    public SearchTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task Search_OkResult()
    {
        // Arrange
        var url = "/search?query=Herschel&category=all";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //TODO: May want to look at this?
        Assert.Contains("<title>Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString - SimplCommerce</title>", responseString);
        Assert.Contains("<h2>Search result for Herschel</h2>", responseString);
    }
}
