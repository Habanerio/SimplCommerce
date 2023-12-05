using Microsoft.Extensions.Configuration;

namespace SimplCommerce.Tests.UnitTests;

public abstract class UnitTestBase
{
    protected IConfiguration GetConfiguration(Dictionary<string, string?> configSettings)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(configSettings).Build();

        return config;
    }
}
