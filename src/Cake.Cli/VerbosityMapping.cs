using Microsoft.Extensions.Logging;

namespace Cake.Cli;

public static class VerbosityMapping
{
    public static LogLevel MapVerbosity(string verbosity)
    {
        return verbosity.ToLowerInvariant() switch
        {
            "quiet" => LogLevel.None,
            "minimal" => LogLevel.Warning,
            "normal" => LogLevel.Information,
            "verbose" => LogLevel.Debug,
            "diagnostic" => LogLevel.Trace,
            _ => LogLevel.Information
        };
    }
}
