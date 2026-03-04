using System.CommandLine;
using System.CommandLine.Invocation;
using Cake.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Cli.Commands;

public static class InstallSkillCommand
{
    public static Command Create(IServiceProvider services)
    {
        var command = new Command("install-skill", "Install the Claude skill file into the current directory");

        var forceOption = new Option<bool>(
            name: "--force",
            description: "Overwrite existing skill file if it exists",
            getDefaultValue: () => false);

        command.AddOption(forceOption);

        command.SetHandler((InvocationContext context) =>
        {
            var force = context.ParseResult.GetValueForOption(forceOption);
            var installer = services.GetRequiredService<ISkillInstaller>();
            var targetDir = Directory.GetCurrentDirectory();
            context.ExitCode = installer.Install(targetDir, force);
        });

        return command;
    }
}
