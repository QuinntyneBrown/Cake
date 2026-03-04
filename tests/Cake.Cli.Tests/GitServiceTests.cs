using Cake.Cli.Services.Git;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-003.1: Git integration tests
/// </summary>
public class GitServiceTests
{
    [Fact]
    public void IGitService_ResolvesFromDI()
    {
        var (services, _) = Program.BuildServiceProvider(Array.Empty<string>());

        var gitService = services.GetService<IGitService>();

        Assert.NotNull(gitService);
        Assert.IsType<GitService>(gitService);
    }

    [Fact]
    public void Init_IsCallable()
    {
        var service = new GitService();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            service.Init(tempDir);
            Assert.True(Directory.Exists(Path.Combine(tempDir, ".git")));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void GetStatus_IsCallable()
    {
        var service = new GitService();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            service.Init(tempDir);
            var status = service.GetStatus(tempDir);
            Assert.NotNull(status);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}
