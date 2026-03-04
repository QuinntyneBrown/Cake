namespace Cake.Cli.Services;

public interface IProjectScaffolder
{
    /// <summary>
    /// Scaffolds a new project in the specified base directory.
    /// Returns 0 on success, non-zero on failure.
    /// </summary>
    int Scaffold(string baseDirectory, string name, bool force);
}
