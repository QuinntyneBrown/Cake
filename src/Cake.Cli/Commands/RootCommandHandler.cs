using Microsoft.Extensions.Logging;

namespace Cake.Cli.Commands;

public class RootCommandHandler
{
    private readonly ILogger<RootCommandHandler> _logger;

    public RootCommandHandler(ILogger<RootCommandHandler> logger)
    {
        _logger = logger;
    }

    public Task<int> HandleAsync()
    {
        _logger.LogInformation("Cake CLI is ready. Use --help to see available commands.");
        return Task.FromResult(0);
    }
}
