using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using Cake.Cli;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-004.1: install-skill Command Definition tests
/// </summary>
public class InstallSkillCommandTests
{
    [Fact]
    public void RootCommand_ContainsInstallSkillSubcommand()
    {
        // Arrange
        var (services, verbosityOption) = Program.BuildServiceProvider(Array.Empty<string>());
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        // Assert
        var subcommandNames = rootCommand.Subcommands.Select(c => c.Name).ToList();
        Assert.Contains("install-skill", subcommandNames);
    }

    [Fact]
    public void InstallSkillCommand_HasForceOption()
    {
        // Arrange
        var (services, verbosityOption) = Program.BuildServiceProvider(Array.Empty<string>());
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        // Act
        var installSkillCmd = rootCommand.Subcommands.First(c => c.Name == "install-skill");
        var optionNames = installSkillCmd.Options.Select(o => o.Name).ToList();

        // Assert
        Assert.Contains("force", optionNames);
    }

    [Fact]
    public async Task InstallSkill_Help_ShowsUsageAndForceOption()
    {
        // Arrange
        var args = new[] { "install-skill", "--help" };
        var (services, verbosityOption) = Program.BuildServiceProvider(args);
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        var console = new TestConsole();
        var parser = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .Build();

        // Act
        var exitCode = await parser.InvokeAsync(args, console);

        // Assert
        Assert.Equal(0, exitCode);
        var text = console.Out.ToString()!;
        Assert.Contains("install-skill", text);
        Assert.Contains("--force", text);
    }
}
