namespace Cake.Cli.Services.CodeGeneration;

public interface IAngularCodeGenerator
{
    void GenerateComponent(string name, string path);
    void GenerateModule(string name, string path);
    void GenerateService(string name, string path);
}
