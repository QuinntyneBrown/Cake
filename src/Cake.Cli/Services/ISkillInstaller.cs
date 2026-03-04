namespace Cake.Cli.Services;

public interface ISkillInstaller
{
    /// <summary>
    /// Installs the Claude skill file into the target directory.
    /// Returns 0 on success, non-zero on failure.
    /// </summary>
    int Install(string targetDirectory, bool force);
}
