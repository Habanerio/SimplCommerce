using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SimplCommerce.Tests.FunctionalTests.Catalog;

public class NewsTests : BaseFunctionalTests, IClassFixture<WebApplicationFactory<Program>>
{
    public NewsTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task NewsDetail_OkResult()
    {
        // Arrange
        var url = "/news";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //TODO: May want to look at this?
        Assert.Contains("<title>Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString - SimplCommerce</title>", responseString);
    }

}
