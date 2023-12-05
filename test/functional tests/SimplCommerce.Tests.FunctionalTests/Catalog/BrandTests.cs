using System.Net;

using Microsoft.AspNetCore.Mvc.Testing;

namespace SimplCommerce.Tests.FunctionalTests.Catalog;

/// <summary>
/// Tests when the user goes to a brand page.
/// Hard to really test without more useful info in the page
/// </summary>
public class BrandTests : BaseFunctionalTests, IClassFixture<WebApplicationFactory<Program>>
{
    public BrandTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task BrandDetail_OkResult()
    {
        // Arrange
        var url = "/calvin-klein";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Calvin Klein - SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task BrandDetail_WithValid_PageNo_OkResult()
    {
        // Arrange
        var url = "/calvin-klein?page=2";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Calvin Klein - SimplCommerce</title>", responseString);
    }

    /// <summary>
    /// Not sure that this should be valid. If I go to page 999, I should get a 404.
    /// Instead, I get the last page of results that is available.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task BrandDetail_WithInvalid_PageNo_OkResult()
    {
        // Arrange
        var url = "/calvin-klein?page=999";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Calvin Klein - SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task BrandDetail_NotFoundResult()
    {
        // Arrange
        var url = "/NOT-A-REAL-BRAND";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("<title>SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task BrandDetail_WithCategory_OkResult()
    {
        // Arrange
        var url = "/calvin-klein?category=t-shirt";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Calvin Klein - SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task BrandDetail_WithCategory_AndMinPrice_OkResult()
    {
        // Arrange
        var url = "/calvin-klein?minPrice=46";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Calvin Klein - SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task BrandDetail_WithCategory_AndMaxPrice_OkResult()
    {
        // Arrange
        var url = "/calvin-klein?maxPrice=46";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Calvin Klein - SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task BrandDetail_WithCategory_AndMinMaxPrice_OkResult()
    {
        // Arrange
        var url = "/calvin-klein?minPrice=10&maxPrice=46";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Calvin Klein - SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task BrandDetail_WithCategory_AndSameMinMaxPrice_OkResult()
    {
        // Arrange
        var url = "/calvin-klein?minPrice=46&maxPrice=46";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Calvin Klein - SimplCommerce</title>", responseString);
    }
}
