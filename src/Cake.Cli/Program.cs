using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Reflection;
using Cake.Cli;
using Cake.Cli.Commands;
using Cake.Cli.Services;
using Cake.Cli.Services.CodeGeneration;
using Cake.Cli.Services.Git;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cake.Cli;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var (services, verbosity) = BuildServiceProvider(args);
        var rootCommand = BuildRootCommand(services, verbosity);

        var parser = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .UseVersionOption()
            .Build();

        return await parser.InvokeAsync(args);
    }

    public static (IServiceProvider Services, Option<string> VerbosityOption) BuildServiceProvider(string[] args)
    {
        var verbosityOption = new Option<string>(
            name: "--verbosity",
            description: "Set the verbosity level (Quiet, Minimal, Normal, Verbose, Diagnostic)",
            getDefaultValue: () => "Normal");

        // Pre-parse to extract verbosity before building the service provider
        var preParseRoot = new RootCommand();
        preParseRoot.AddGlobalOption(verbosityOption);
        var preParseResult = preParseRoot.Parse(args);
        var verbosityValue = preParseResult.GetValueForOption(verbosityOption) ?? "Normal";
        var logLevel = VerbosityMapping.MapVerbosity(verbosityValue);

        var services = new ServiceCollection();

        // Configuration (L2-REQ-001.4)
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables(prefix: "CAKE_")
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Logging (L2-REQ-001.3)
        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(logLevel);
            builder.AddConsole();
        });

        // Application services (L2-REQ-001.2)
        services.AddSingleton<IGreetingService, GreetingService>();
        services.AddSingleton<ISkillInstaller, SkillInstaller>();
        services.AddTransient<RootCommandHandler>();

        // Git integration (L2-REQ-003.1)
        services.AddSingleton<IGitService, GitService>();

        // Code generation (L2-REQ-003.2, L2-REQ-003.3)
        services.AddSingleton<IDotNetCodeGenerator, DotNetCodeGenerator>();
        services.AddSingleton<IAngularCodeGenerator, AngularCodeGenerator>();

        // Cake build integration (L2-REQ-002.2)
        services.AddSingleton<ICakeBuildRunner, CakeBuildRunner>();

        // Project scaffolding (L2-REQ-005)
        services.AddSingleton<IProjectScaffolder, ProjectScaffolder>();

        // Build program modification (L2-REQ-006)
        services.AddSingleton<IBuildProgramModifier, BuildProgramModifier>();

        var provider = services.BuildServiceProvider();
        return (provider, verbosityOption);
    }

    public static RootCommand BuildRootCommand(IServiceProvider services, Option<string> verbosityOption)
    {
        var rootCommand = new RootCommand("Cake CLI \u2014 build scaffolding and code generation");
        rootCommand.AddGlobalOption(verbosityOption);

        // Subcommands
        rootCommand.AddCommand(InstallSkillCommand.Create(services));
        rootCommand.AddCommand(CreateCommand.Create(services));

        rootCommand.SetHandler(async () =>
        {
            var handler = services.GetRequiredService<RootCommandHandler>();
            await handler.HandleAsync();
        });

        return rootCommand;
    }

    /// <summary>
    /// Returns the CLI version from the assembly informational version.
    /// </summary>
    public static string GetVersion()
    {
        return typeof(Program).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "0.0.0";
    }
}
