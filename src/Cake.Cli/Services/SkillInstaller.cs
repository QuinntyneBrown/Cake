using Microsoft.Extensions.Logging;

namespace Cake.Cli.Services;

public class SkillInstaller : ISkillInstaller
{
    private readonly ILogger<SkillInstaller> _logger;

    public SkillInstaller(ILogger<SkillInstaller> logger)
    {
        _logger = logger;
    }

    public int Install(string targetDirectory, bool force)
    {
        var skillDir = Path.Combine(targetDirectory, ".claude", "skills");
        var skillPath = Path.Combine(skillDir, "cake-cli.md");

        if (File.Exists(skillPath) && !force)
        {
            _logger.LogWarning("Skill file already exists at {Path}. Use --force to overwrite.", skillPath);
            return 1;
        }

        Directory.CreateDirectory(skillDir);
        File.WriteAllText(skillPath, SkillContent);
        _logger.LogInformation("Installed Claude skill file at {Path}", skillPath);
        return 0;
    }

    private const string SkillContent =
        """
        # Cake CLI Skill

        Use the `cake-cli` command to scaffold projects and manage build infrastructure.

        ## Available Commands

        ### `cake-cli create --name <project-name>`
        Creates a new project folder with a Cake Frosting build setup.

        **Options:**
        - `--name` (required): The name of the project folder to create
        - `--force`: Overwrite existing directory if it exists

        **What it creates:**
        - `{name}/build/build.sln` — Solution file
        - `{name}/build/build.csproj` — Cake Frosting build project
        - `{name}/build/Program.cs` — Cake Frosting entry point
        - `{name}/build/.claude/skills/cake-cli.md` — This skill file

        ### `cake-cli install-skill`
        Installs this Claude skill file into the current directory.

        **Options:**
        - `--force`: Overwrite existing skill file if it exists

        ## Examples

        ```bash
        # Create a new project
        cake-cli create --name MyApp

        # Install the Claude skill in current directory
        cake-cli install-skill

        # Force overwrite existing skill
        cake-cli install-skill --force
        ```

        ## When to Use
        - Use `create` when starting a new project that needs Cake build infrastructure
        - Use `install-skill` to add CLI awareness to an existing project directory
        """;
}
