using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using Cake.Cli;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-006.1: add-project Command Definition tests
/// </summary>
public class AddProjectCommandTests
{
    [Fact]
    public void RootCommand_ContainsAddProjectSubcommand()
    {
        // Arrange
        var (services, verbosityOption) = Program.BuildServiceProvider(Array.Empty<string>());
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        // Assert
        var subcommandNames = rootCommand.Subcommands.Select(c => c.Name).ToList();
        Assert.Contains("add-project", subcommandNames);
    }

    [Fact]
    public void AddProjectCommand_HasGitUrlOption()
    {
        // Arrange
        var (services, verbosityOption) = Program.BuildServiceProvider(Array.Empty<string>());
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        // Act
        var addProjectCmd = rootCommand.Subcommands.First(c => c.Name == "add-project");
        var optionNames = addProjectCmd.Options.Select(o => o.Name).ToList();

        // Assert
        Assert.Contains("git-url", optionNames);
    }

    [Fact]
    public void AddProjectCommand_HasProjectsOption()
    {
        // Arrange
        var (services, verbosityOption) = Program.BuildServiceProvider(Array.Empty<string>());
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        // Act
        var addProjectCmd = rootCommand.Subcommands.First(c => c.Name == "add-project");
        var optionNames = addProjectCmd.Options.Select(o => o.Name).ToList();

        // Assert
        Assert.Contains("projects", optionNames);
    }

    [Fact]
    public void AddProjectCommand_GitUrlOptionIsRequired()
    {
        // Arrange
        var (services, verbosityOption) = Program.BuildServiceProvider(Array.Empty<string>());
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        var addProjectCmd = rootCommand.Subcommands.First(c => c.Name == "add-project");
        var gitUrlOption = addProjectCmd.Options.First(o => o.Name == "git-url");

        // Assert
        Assert.True(gitUrlOption.IsRequired);
    }

    [Fact]
    public void AddProjectCommand_ProjectsOptionIsRequired()
    {
        // Arrange
        var (services, verbosityOption) = Program.BuildServiceProvider(Array.Empty<string>());
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        var addProjectCmd = rootCommand.Subcommands.First(c => c.Name == "add-project");
        var projectsOption = addProjectCmd.Options.First(o => o.Name == "projects");

        // Assert
        Assert.True(projectsOption.IsRequired);
    }

    [Fact]
    public async Task AddProject_Help_ShowsUsage()
    {
        // Arrange
        var args = new[] { "add-project", "--help" };
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
        Assert.Contains("add-project", text);
        Assert.Contains("--git-url", text);
        Assert.Contains("--projects", text);
    }

    [Fact]
    public async Task AddProject_MissingRequiredOptions_ShowsError()
    {
        // Arrange
        var args = new[] { "add-project" };
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
        Assert.Contains("--git-url", errorText);
    }
}
