namespace Cake.Cli.Services.CodeGeneration;

public interface IDotNetCodeGenerator
{
    void GenerateSolution(string name, string path);
    void GenerateProject(string name, string path, string template);
    void GenerateSourceFile(string name, string path, string content);
}
