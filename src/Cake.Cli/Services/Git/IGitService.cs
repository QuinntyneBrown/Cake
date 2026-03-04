namespace Cake.Cli.Services.Git;

public interface IGitService
{
    void Init(string path);
    string GetStatus(string path);
}
