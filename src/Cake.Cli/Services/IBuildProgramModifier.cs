namespace Cake.Cli.Services;

public interface IBuildProgramModifier
{
    /// <summary>
    /// Modifies the Program.cs in a cake build directory to include solution generation.
    /// Returns 0 on success, non-zero on failure.
    /// </summary>
    int Modify(string buildDirectory, string gitUrl, string[] projectNames);
}
