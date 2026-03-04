using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using Cake.Cli;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-005.1: create Command Definition tests
/// </summary>
public class CreateCommandTests
{
    [Fact]
    public void RootCommand_ContainsCreateSubcommand()
    {
        // Arrange
        var (services, verbosityOption) = Program.BuildServiceProvider(Array.Empty<string>());
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        // Assert
        var subcommandNames = rootCommand.Subcommands.Select(c => c.Name).ToList();
        Assert.Contains("create", subcommandNames);
    }

    [Fact]
    public void CreateCommand_HasNameOption()
    {
        // Arrange
        var (services, verbosityOption) = Program.BuildServiceProvider(Array.Empty<string>());
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        // Act
        var createCmd = rootCommand.Subcommands.First(c => c.Name == "create");
        var optionNames = createCmd.Options.Select(o => o.Name).ToList();

        // Assert
        Assert.Contains("name", optionNames);
    }

    [Fact]
    public void CreateCommand_NameOptionIsRequired()
    {
        // Arrange
        var (services, verbosityOption) = Program.BuildServiceProvider(Array.Empty<string>());
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        var createCmd = rootCommand.Subcommands.First(c => c.Name == "create");
        var nameOption = createCmd.Options.First(o => o.Name == "name");

        // Assert
        Assert.True(nameOption.IsRequired);
    }

    [Fact]
    public void CreateCommand_HasForceOption()
    {
        // Arrange
        var (services, verbosityOption) = Program.BuildServiceProvider(Array.Empty<string>());
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        // Act
        var createCmd = rootCommand.Subcommands.First(c => c.Name == "create");
        var optionNames = createCmd.Options.Select(o => o.Name).ToList();

        // Assert
        Assert.Contains("force", optionNames);
    }

    [Fact]
    public async Task Create_MissingName_ShowsError()
    {
        // Arrange
        var args = new[] { "create" };
        var (services, verbosityOption) = Program.BuildServiceProvider(args);
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        var console = new TestConsole();
        var parser = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .Build();

        // Act
        var exitCode = await parser.InvokeAsync(args, console);

        // Assert
        Assert.NotEqual(0, exitCode);
        var errorText = console.Error.ToString()!;
        Assert.Contains("--name", errorText);
    }

    [Fact]
    public async Task Create_Help_ShowsUsage()
    {
        // Arrange
        var args = new[] { "create", "--help" };
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
        Assert.Contains("create", text);
        Assert.Contains("--name", text);
        Assert.Contains("--force", text);
    }
}
