using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Cake.Cli;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-001.1: System.CommandLine Root Command tests
/// </summary>
public class RootCommandTests
{
    [Fact]
    public async Task RootCommand_Help_ReturnsZeroAndContainsDescription()
    {
        // Arrange
        var args = new[] { "--help" };
        var (services, verbosityOption) = Program.BuildServiceProvider(args);
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        var output = new StringWriter();
        var parser = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .Build();

        Console.SetOut(output);

        // Act
        var exitCode = await parser.InvokeAsync(args);

        // Assert
        Assert.Equal(0, exitCode);
        var text = output.ToString();
        Assert.Contains("Cake CLI", text);
        Assert.Contains("build scaffolding and code generation", text);
    }

    [Fact]
    public async Task RootCommand_Version_ReturnsVersionString()
    {
        // Arrange
        var args = new[] { "--version" };
        var (services, verbosityOption) = Program.BuildServiceProvider(args);
        var rootCommand = Program.BuildRootCommand(services, verbosityOption);

        var output = new StringWriter();
        var parser = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .UseVersionOption()
            .Build();

        Console.SetOut(output);

        // Act
        var exitCode = await parser.InvokeAsync(args);

        // Assert
        Assert.Equal(0, exitCode);
        var versionOutput = output.ToString().Trim();
        Assert.False(string.IsNullOrWhiteSpace(versionOutput), "Version output should not be empty");
        // Version should match a semver-like pattern
        Assert.Matches(@"\d+\.\d+\.\d+", versionOutput);
    }
}
