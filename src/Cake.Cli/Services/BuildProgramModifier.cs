using Cake.Cli.Templates;
using Microsoft.Extensions.Logging;

namespace Cake.Cli.Services;

public class BuildProgramModifier : IBuildProgramModifier
{
    private readonly ILogger<BuildProgramModifier> _logger;

    public BuildProgramModifier(ILogger<BuildProgramModifier> logger)
    {
        _logger = logger;
    }

    public int Modify(string buildDirectory, string gitUrl, string[] projectNames)
    {
        // L2-REQ-006.2: Validate build directory has a .csproj file
        var csprojFiles = Directory.GetFiles(buildDirectory, "*.csproj");
        if (csprojFiles.Length == 0)
        {
            _logger.LogError("No .csproj file found in '{BuildDirectory}'.", buildDirectory);
            return 1;
        }

        // L2-REQ-006.2: Validate build directory has Program.cs
        var programPath = Path.Combine(buildDirectory, "Program.cs");
        if (!File.Exists(programPath))
        {
            _logger.LogError("No Program.cs found in '{BuildDirectory}'.", buildDirectory);
            return 1;
        }

        var templateProvider = new BuildProjectTemplateProvider();
        var content = templateProvider.GetBuildProgramCsWithSolutionGeneration(gitUrl, projectNames);

        File.WriteAllText(programPath, content);
        _logger.LogInformation("Updated Program.cs with solution generation for {GitUrl}.", gitUrl);

        return 0;
    }
}
