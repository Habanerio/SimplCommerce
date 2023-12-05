using System.Net;

using Microsoft.AspNetCore.Mvc.Testing;

namespace SimplCommerce.Tests.FunctionalTests.Catalog;

/// <summary>
/// Tests when the user goes to a brand page.
/// Hard to really test without more useful info in the page
/// </summary>
public class CategoryTests : BaseFunctionalTests, IClassFixture<WebApplicationFactory<Program>>
{
    public CategoryTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task CategoryDetail_OkResult()
    {
        // Arrange
        var url = "/woman";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Woman - SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task CategoryDetail_With_PageNo_OkResult()
    {
        // Arrange
        var url = "/man?page=1";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Man - SimplCommerce</title>", responseString);
    }

    /// <summary>
    /// Not sure that this should be valid. If I go to page 999, I should get a 404.
    /// Instead, I get the last page of results that is available.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CategoryDetail_WithInvalid_PageNo_OkResult()
    {
        // Arrange
        var url = "/man?page=999";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Man - SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task CategoryDetail_NotFoundResult()
    {
        // Arrange
        var url = "/NOT-A-REAL-CATEGORY";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("<title>SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task CategoryDetail_WithBrand_OkResult()
    {
        // Arrange
        var url = "/man?brand=calvin-klein&page=1&pageSize=10";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Man - SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task CategoryDetail_WithSubCategory_AndBrand_OkResult()
    {
        // Arrange
        var url = "/man?brand=calvin-klein&category=t-shirt&page=1&pageSize=10";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Man - SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task CategoryDetail_WithCategory_AndMinPrice_OkResult()
    {
        // Arrange
        var url = "/man?minPrice=46";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Man - SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task CategoryDetail_WithCategory_AndMaxPrice_OkResult()
    {
        // Arrange
        var url = "/man?maxPrice=46";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Man - SimplCommerce</title>", responseString);
    }

    [Fact]
    public async Task CategoryDetail_WithCategory_AndMinMaxPrice_OkResult()
    {
        // Arrange
        var url = "/man?minPrice=46&maxPrice=46";

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<title>Man - SimplCommerce</title>", responseString);
    }
}
