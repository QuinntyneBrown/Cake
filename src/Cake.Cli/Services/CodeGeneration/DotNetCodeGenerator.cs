namespace Cake.Cli.Services.CodeGeneration;

public class DotNetCodeGenerator : IDotNetCodeGenerator
{
    public void GenerateSolution(string name, string path)
    {
        var solutionDir = Path.Combine(path, name);
        Directory.CreateDirectory(solutionDir);

        var slnPath = Path.Combine(solutionDir, $"{name}.sln");
        File.WriteAllText(slnPath, string.Empty);
    }

    public void GenerateProject(string name, string path, string template)
    {
        var projectDir = Path.Combine(path, name);
        Directory.CreateDirectory(projectDir);

        var csprojPath = Path.Combine(projectDir, $"{name}.csproj");
        var content = $"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
              </PropertyGroup>
            </Project>
            """;
        File.WriteAllText(csprojPath, content);
    }

    public void GenerateSourceFile(string name, string path, string content)
    {
        Directory.CreateDirectory(path);
        var filePath = Path.Combine(path, name);
        File.WriteAllText(filePath, content);
    }
}
