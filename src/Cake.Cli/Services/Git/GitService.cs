using System.Diagnostics;

namespace Cake.Cli.Services.Git;

public class GitService : IGitService
{
    public void Init(string path)
    {
        RunGit($"init \"{path}\"", path);
    }

    public string GetStatus(string path)
    {
        return RunGit("status --porcelain", path);
    }

    private static string RunGit(string arguments, string workingDirectory)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process is null)
            return string.Empty;

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output.TrimEnd();
    }
}
