using Cake.Cli;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-001.3: Logging via Microsoft.Extensions.Logging tests
/// </summary>
public class LoggingTests
{
    [Theory]
    [InlineData("quiet", LogLevel.None)]
    [InlineData("minimal", LogLevel.Warning)]
    [InlineData("normal", LogLevel.Information)]
    [InlineData("verbose", LogLevel.Debug)]
    [InlineData("diagnostic", LogLevel.Trace)]
    [InlineData("Quiet", LogLevel.None)]
    [InlineData("Minimal", LogLevel.Warning)]
    [InlineData("Normal", LogLevel.Information)]
    [InlineData("Verbose", LogLevel.Debug)]
    [InlineData("Diagnostic", LogLevel.Trace)]
    public void VerbosityMapping_MapsCorrectLogLevel(string verbosity, LogLevel expected)
    {
        // Act
        var result = VerbosityMapping.MapVerbosity(verbosity);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Logger_IsRegisteredAndInjectable()
    {
        // Arrange
        var (services, _) = Program.BuildServiceProvider(Array.Empty<string>());

        // Act
        var loggerFactory = services.GetService<ILoggerFactory>();
        var logger = services.GetService<ILogger<LoggingTests>>();

        // Assert
        Assert.NotNull(loggerFactory);
        Assert.NotNull(logger);
    }
}
