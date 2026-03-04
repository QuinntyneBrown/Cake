using Cake.Core;

namespace Cake.Cli.Services;

public class CakeBuildRunner : ICakeBuildRunner
{
    public IReadOnlyList<string> GetAvailableTasks()
    {
        return new List<string> { "Default", "Clean", "Build", "Test" };
    }
}
