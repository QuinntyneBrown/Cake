using Cake.Cli;
using Cake.Cli.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-002.2: CLI Extends Cake Capabilities — service registration tests
/// </summary>
public class CakeBuildIntegrationTests
{
    [Fact]
    public void Container_ResolvesCakeBuildRunner()
    {
        var (services, _) = Program.BuildServiceProvider(Array.Empty<string>());

        var runner = services.GetService<ICakeBuildRunner>();

        Assert.NotNull(runner);
        Assert.IsType<CakeBuildRunner>(runner);
    }

    [Fact]
    public void CakeBuildRunner_ReturnsAvailableTasks()
    {
        var (services, _) = Program.BuildServiceProvider(Array.Empty<string>());
        var runner = services.GetRequiredService<ICakeBuildRunner>();

        var tasks = runner.GetAvailableTasks();

        Assert.NotNull(tasks);
    }

    [Fact]
    public void CakeBuildRunner_CanBeInstantiated()
    {
        var runner = new CakeBuildRunner();

        Assert.NotNull(runner);
    }
}
