using Microsoft.Extensions.Configuration;

namespace SimpleCommerce.Tests.UnitTests;

public abstract class UnitTestBase
{
    protected IConfiguration GetConfiguration(Dictionary<string, string?> configSettings)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(configSettings).Build();

        return config;
    }
}
