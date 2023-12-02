using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace SimpleCommerce.Tests.FunctionalTests;

public class BaseFunctionalTests
{
    private readonly WebApplicationFactory<Program> _factory;

    protected readonly HttpClient HttpClient;
    protected readonly IConfiguration Config;

    protected BaseFunctionalTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;

        HttpClient = _factory.CreateClient();

        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.development.json")
            .AddEnvironmentVariables()
            .Build();

        Config = builder;
    }
}
