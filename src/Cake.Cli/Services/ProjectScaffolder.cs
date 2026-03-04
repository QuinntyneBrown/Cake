using System.Diagnostics;
using Cake.Cli.Templates;
using Microsoft.Extensions.Logging;

namespace Cake.Cli.Services;

public class ProjectScaffolder : IProjectScaffolder
{
    private readonly ISkillInstaller _skillInstaller;
    private readonly ILogger<ProjectScaffolder> _logger;

    public ProjectScaffolder(ISkillInstaller skillInstaller, ILogger<ProjectScaffolder> logger)
    {
        _skillInstaller = skillInstaller;
        _logger = logger;
    }

    public int Scaffold(string baseDirectory, string name, bool force)
    {
        var projectDir = Path.Combine(baseDirectory, name);

        // L2-REQ-005.2: Root folder creation
        if (Directory.Exists(projectDir))
        {
            if (!force)
            {
                _logger.LogError("Directory '{Name}' already exists. Use --force to overwrite.", name);
                return 1;
            }

            _logger.LogWarning("Overwriting existing directory '{Name}'.", name);
            Directory.Delete(projectDir, recursive: true);
        }

        Directory.CreateDirectory(projectDir);
        _logger.LogInformation("Created project directory '{Name}'.", name);

        // L2-REQ-005.3: Build folder and solution scaffolding
        var buildDir = Path.Combine(projectDir, "build");
        Directory.CreateDirectory(buildDir);

        var templateProvider = new BuildProjectTemplateProvider();

        // Write build.csproj
        var csprojPath = Path.Combine(buildDir, "build.csproj");
        File.WriteAllText(csprojPath, templateProvider.GetBuildCsproj());
        _logger.LogInformation("Created {File}", csprojPath);

        // Write Program.cs
        var programPath = Path.Combine(buildDir, "Program.cs");
        File.WriteAllText(programPath, templateProvider.GetBuildProgramCs());
        _logger.LogInformation("Created {File}", programPath);

        // Create solution file using dotnet new sln, then add project
        var slnResult = RunDotNet($"new sln --name build --output \"{buildDir}\"");
        if (slnResult != 0)
        {
            _logger.LogError("Failed to create solution file.");
            return 1;
        }

        // Find the created sln/slnx file
        var slnFile = FindSolutionFile(buildDir);
        if (slnFile == null)
        {
            _logger.LogError("Solution file was not created.");
            return 1;
        }

        var addResult = RunDotNet($"sln \"{slnFile}\" add \"{csprojPath}\"");
        if (addResult != 0)
        {
            _logger.LogError("Failed to add build.csproj to solution.");
            return 1;
        }

        _logger.LogInformation("Created solution file and added build.csproj.");

        // L2-REQ-005.4: Auto-install Claude skill
        var skillResult = _skillInstaller.Install(buildDir, force: true);
        if (skillResult != 0)
        {
            _logger.LogError("Failed to install Claude skill file.");
            return skillResult;
        }

        _logger.LogInformation("Project '{Name}' created successfully.", name);
        return 0;
    }

    private static string? FindSolutionFile(string directory)
    {
        // Check for .slnx first (newer format), then .sln
        var slnxFiles = Directory.GetFiles(directory, "*.slnx");
        if (slnxFiles.Length > 0) return slnxFiles[0];

        var slnFiles = Directory.GetFiles(directory, "*.sln");
        if (slnFiles.Length > 0) return slnFiles[0];

        return null;
    }

    private int RunDotNet(string arguments)
    {
        _logger.LogDebug("Running: dotnet {Arguments}", arguments);
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(psi);
        if (process == null)
        {
            _logger.LogError("Failed to start dotnet process.");
            return 1;
        }

        process.WaitForExit(TimeSpan.FromSeconds(60));

        if (process.ExitCode != 0)
        {
            var stderr = process.StandardError.ReadToEnd();
            _logger.LogError("dotnet exited with code {Code}: {Error}", process.ExitCode, stderr);
        }

        return process.ExitCode;
    }
}
