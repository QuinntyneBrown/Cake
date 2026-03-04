using Cake.Cli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-001.4: Configuration via Microsoft.Extensions.Configuration tests
/// </summary>
public class ConfigurationTests
{
    [Fact]
    public void Configuration_LoadsWithoutAppsettingsJson()
    {
        // Arrange & Act — no appsettings.json file exists in test output
        var (services, _) = Program.BuildServiceProvider(Array.Empty<string>());
        var configuration = services.GetService<IConfiguration>();

        // Assert
        Assert.NotNull(configuration);
    }

    [Fact]
    public void Configuration_ReadsFromEnvironmentVariables()
    {
        // Arrange
        var testKey = "CAKE_TEST_VALUE";
        var testValue = "hello_from_env";
        Environment.SetEnvironmentVariable(testKey, testValue);

        try
        {
            var (services, _) = Program.BuildServiceProvider(Array.Empty<string>());
            var configuration = services.GetRequiredService<IConfiguration>();

            // Act — env var prefix is "CAKE_", so the key without prefix is "TEST_VALUE"
            var value = configuration["TEST_VALUE"];

            // Assert
            Assert.Equal(testValue, value);
        }
        finally
        {
            Environment.SetEnvironmentVariable(testKey, null);
        }
    }
}
