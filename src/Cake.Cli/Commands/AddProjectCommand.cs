using System.CommandLine;
using System.CommandLine.Invocation;
using Cake.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Cli.Commands;

public static class AddProjectCommand
{
    public static Command Create(IServiceProvider services)
    {
        var command = new Command("add-project", "Add .NET project(s) from a git repository to the build program's solution generation");

        var gitUrlOption = new Option<string>(
            name: "--git-url",
            description: "The git repository URL containing the .NET project(s)")
        {
            IsRequired = true,
        };

        var projectsOption = new Option<string[]>(
            name: "--projects",
            description: "The name(s) of the .NET project(s) in the git repository")
        {
            IsRequired = true,
            AllowMultipleArgumentsPerToken = true,
        };

        command.AddOption(gitUrlOption);
        command.AddOption(projectsOption);

        command.SetHandler((InvocationContext context) =>
        {
            var gitUrl = context.ParseResult.GetValueForOption(gitUrlOption)!;
            var projects = context.ParseResult.GetValueForOption(projectsOption)!;
            var modifier = services.GetRequiredService<IBuildProgramModifier>();
            var cwd = Directory.GetCurrentDirectory();
            context.ExitCode = modifier.Modify(cwd, gitUrl, projects);
        });

        return command;
    }
}
