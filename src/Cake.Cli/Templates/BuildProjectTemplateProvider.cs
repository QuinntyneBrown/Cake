namespace Cake.Cli.Templates;

public class BuildProjectTemplateProvider
{
    public string GetBuildCsproj()
    {
        return """
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net8.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Cake.Frosting" Version="4.0.0" />
              </ItemGroup>

            </Project>
            """;
    }

    public string GetBuildProgramCs()
    {
        return """
            using Cake.Core;
            using Cake.Core.Diagnostics;
            using Cake.Frosting;

            return new CakeHost()
                .UseContext<BuildContext>()
                .Run(args);

            public class BuildContext : FrostingContext
            {
                public BuildContext(ICakeContext context) : base(context) { }
            }

            [TaskName("Default")]
            public sealed class DefaultTask : FrostingTask<BuildContext>
            {
                public override void Run(BuildContext context)
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information, "Hello from Cake Frosting!");
                }
            }
            """;
    }

    public string GetBuildProgramCsWithSolutionGeneration(string gitUrl, string[] projectNames)
    {
        var repoName = gitUrl.Split('/').Last().Replace(".git", "");

        var findAndAddProjects = string.Join("\n", projectNames.Select(name => $$"""
                        var {{SanitizeIdentifier(name)}}Csproj = Directory.GetFiles(Path.Combine(srcDir, "{{repoName}}"), "{{name}}.csproj", SearchOption.AllDirectories);
                        if ({{SanitizeIdentifier(name)}}Csproj.Length > 0)
                        {
                            RunProcess("dotnet", $"sln \"{slnFile}\" add \"{{{SanitizeIdentifier(name)}}Csproj[0]}\"", rootDir);
                            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Added {{name}} to solution.");
                        }
                        else
                        {
                            context.Log.Write(Verbosity.Normal, LogLevel.Warning, "Could not find {{name}}.csproj in cloned repository.");
                        }
            """));

        return $$"""
            using System.Diagnostics;
            using Cake.Core;
            using Cake.Core.Diagnostics;
            using Cake.Frosting;

            return new CakeHost()
                .UseContext<BuildContext>()
                .Run(args);

            public class BuildContext : FrostingContext
            {
                public BuildContext(ICakeContext context) : base(context) { }

                public string RootDirectory => new DirectoryInfo(Environment.CurrentDirectory).Parent!.FullName;
            }

            [TaskName("GenerateSolution")]
            public sealed class GenerateSolutionTask : FrostingTask<BuildContext>
            {
                public override void Run(BuildContext context)
                {
                    var rootDir = context.RootDirectory;
                    var rootDirName = new DirectoryInfo(rootDir).Name;

                    // Check if solution already exists
                    var existingSln = Directory.GetFiles(rootDir, "*.sln");
                    var existingSlnx = Directory.GetFiles(rootDir, "*.slnx");
                    if (existingSln.Length > 0 || existingSlnx.Length > 0)
                    {
                        context.Log.Write(Verbosity.Normal, LogLevel.Information, "Solution file already exists. Skipping generation.");
                        return;
                    }

                    var srcDir = Path.Combine(rootDir, "src");
                    Directory.CreateDirectory(srcDir);

                    // Clone repository if not already present
                    var repoDir = Path.Combine(srcDir, "{{repoName}}");
                    if (!Directory.Exists(repoDir))
                    {
                        context.Log.Write(Verbosity.Normal, LogLevel.Information, "Cloning {{gitUrl}}...");
                        RunProcess("git", "clone {{gitUrl}} \"" + repoDir + "\"", rootDir);
                    }

                    // Create solution
                    RunProcess("dotnet", $"new sln --name {rootDirName} --output \"{rootDir}\"", rootDir);

                    // Find the created solution file
                    var slnFiles = Directory.GetFiles(rootDir, $"{rootDirName}.sln*");
                    if (slnFiles.Length == 0)
                    {
                        context.Log.Write(Verbosity.Normal, LogLevel.Error, "Failed to create solution file.");
                        return;
                    }
                    var slnFile = slnFiles[0];

            {{findAndAddProjects}}
                }

                private static void RunProcess(string fileName, string arguments, string workingDirectory)
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = arguments,
                        WorkingDirectory = workingDirectory,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };

                    using var process = Process.Start(psi);
                    if (process == null)
                    {
                        throw new InvalidOperationException($"Failed to start process: {fileName}");
                    }

                    process.WaitForExit(TimeSpan.FromSeconds(120));

                    if (process.ExitCode != 0)
                    {
                        var stderr = process.StandardError.ReadToEnd();
                        throw new InvalidOperationException($"{fileName} exited with code {process.ExitCode}: {stderr}");
                    }
                }
            }

            [TaskName("Default")]
            [IsDependentOn(typeof(GenerateSolutionTask))]
            public sealed class DefaultTask : FrostingTask<BuildContext>
            {
                public override void Run(BuildContext context)
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information, "Build completed.");
                }
            }
            """;
    }

    private static string SanitizeIdentifier(string name)
    {
        return name.Replace(".", "").Replace("-", "").Replace(" ", "");
    }
}
