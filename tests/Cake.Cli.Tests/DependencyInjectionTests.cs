using Cake.Cli;
using Cake.Cli.Commands;
using Cake.Cli.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-001.2: DI via Microsoft.Extensions.DependencyInjection tests
/// </summary>
public class DependencyInjectionTests
{
    [Fact]
    public void Container_ResolvesRegisteredService()
    {
        // Arrange
        var (services, _) = Program.BuildServiceProvider(Array.Empty<string>());

        // Act
        var greetingService = services.GetService<IGreetingService>();

        // Assert
        Assert.NotNull(greetingService);
        Assert.IsType<GreetingService>(greetingService);
    }

    [Fact]
    public void CommandHandler_ReceivesDependencyViaInjection()
    {
        // Arrange
        var (services, _) = Program.BuildServiceProvider(Array.Empty<string>());

        // Act
        var handler = services.GetService<RootCommandHandler>();

        // Assert
        Assert.NotNull(handler);
    }
}
