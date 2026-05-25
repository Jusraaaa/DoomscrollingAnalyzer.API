using Microsoft.Extensions.Configuration;

namespace DoomscrollingAnalyzer.API.Tests.TestSupport;

internal static class TestConfigurationFactory
{
    public static IConfiguration Create()
    {
        var settings = new Dictionary<string, string?>
        {
            ["JwtSettings:Key"] = "this-is-a-secure-test-jwt-key-with-more-than-32-characters",
            ["JwtSettings:Issuer"] = "DoomscrollingAnalyzer.API.Tests",
            ["JwtSettings:Audience"] = "DoomscrollingAnalyzer.Tests",
            ["JwtSettings:ExpiresInMinutes"] = "60"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }
}
