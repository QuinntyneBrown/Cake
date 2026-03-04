using System.CommandLine;
using System.CommandLine.Invocation;
using Cake.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Cli.Commands;

public static class CreateCommand
{
    public static Command Create(IServiceProvider services)
    {
        var command = new Command("create", "Create a new project with Cake Frosting build setup");

        var nameOption = new Option<string>(
            name: "--name",
            description: "The name of the project folder to create")
        {
            IsRequired = true,
        };

        var forceOption = new Option<bool>(
            name: "--force",
            description: "Overwrite existing directory if it exists",
            getDefaultValue: () => false);

        command.AddOption(nameOption);
        command.AddOption(forceOption);

        command.SetHandler((InvocationContext context) =>
        {
            var name = context.ParseResult.GetValueForOption(nameOption)!;
            var force = context.ParseResult.GetValueForOption(forceOption);
            var scaffolder = services.GetRequiredService<IProjectScaffolder>();
            var baseDir = Directory.GetCurrentDirectory();
            context.ExitCode = scaffolder.Scaffold(baseDir, name, force);
        });

        return command;
    }
}
