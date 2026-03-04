namespace Cake.Cli.Services;

public interface ICakeBuildRunner
{
    IReadOnlyList<string> GetAvailableTasks();
}
