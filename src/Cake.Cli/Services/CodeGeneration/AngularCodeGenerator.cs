namespace Cake.Cli.Services.CodeGeneration;

public class AngularCodeGenerator : IAngularCodeGenerator
{
    public void GenerateComponent(string name, string path)
    {
        var componentDir = Path.Combine(path, name);
        Directory.CreateDirectory(componentDir);

        File.WriteAllText(Path.Combine(componentDir, $"{name}.component.ts"), string.Empty);
        File.WriteAllText(Path.Combine(componentDir, $"{name}.component.html"), string.Empty);
        File.WriteAllText(Path.Combine(componentDir, $"{name}.component.css"), string.Empty);
    }

    public void GenerateModule(string name, string path)
    {
        var moduleDir = Path.Combine(path, name);
        Directory.CreateDirectory(moduleDir);

        File.WriteAllText(Path.Combine(moduleDir, $"{name}.module.ts"), string.Empty);
    }

    public void GenerateService(string name, string path)
    {
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, $"{name}.service.ts"), string.Empty);
    }
}
