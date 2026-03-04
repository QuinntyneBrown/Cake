using System.Diagnostics;
using Cake.Cli.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-005.2, L2-REQ-005.3, L2-REQ-005.4: Project scaffolding tests
/// </summary>
public class ProjectScaffolderTests : IDisposable
{
    private readonly string _tempDir;
    private readonly IProjectScaffolder _scaffolder;

    public ProjectScaffolderTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "CakeTest_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);

        var skillLogger = NullLogger<SkillInstaller>.Instance;
        var skillInstaller = new SkillInstaller(skillLogger);
        var scaffolderLogger = NullLogger<ProjectScaffolder>.Instance;
        _scaffolder = new ProjectScaffolder(skillInstaller, scaffolderLogger);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            try { Directory.Delete(_tempDir, recursive: true); }
            catch { /* best effort cleanup */ }
        }
    }

    // L2-REQ-005.2: Root folder creation
    [Fact]
    public void Scaffold_CreatesProjectDirectory()
    {
        var result = _scaffolder.Scaffold(_tempDir, "TestProject", force: false);

        Assert.Equal(0, result);
        Assert.True(Directory.Exists(Path.Combine(_tempDir, "TestProject")));
    }

    // L2-REQ-005.2: Existing dir fails without --force
    [Fact]
    public void Scaffold_ExistingDirectory_WithoutForce_Fails()
    {
        var projectDir = Path.Combine(_tempDir, "TestProject");
        Directory.CreateDirectory(projectDir);

        var result = _scaffolder.Scaffold(_tempDir, "TestProject", force: false);

        Assert.NotEqual(0, result);
    }

    // L2-REQ-005.2: --force overwrites existing directory
    [Fact]
    public void Scaffold_ExistingDirectory_WithForce_Overwrites()
    {
        var projectDir = Path.Combine(_tempDir, "TestProject");
        Directory.CreateDirectory(projectDir);
        File.WriteAllText(Path.Combine(projectDir, "old-file.txt"), "old");

        var result = _scaffolder.Scaffold(_tempDir, "TestProject", force: true);

        Assert.Equal(0, result);
        Assert.True(Directory.Exists(projectDir));
        Assert.False(File.Exists(Path.Combine(projectDir, "old-file.txt")));
    }

    // L2-REQ-005.3: Build directory is created
    [Fact]
    public void Scaffold_CreatesBuildDirectory()
    {
        _scaffolder.Scaffold(_tempDir, "TestProject", force: false);

        Assert.True(Directory.Exists(Path.Combine(_tempDir, "TestProject", "build")));
    }

    // L2-REQ-005.3: build.csproj is created with Cake.Frosting reference
    [Fact]
    public void Scaffold_CreatesBuildCsproj_WithCakeFrosting()
    {
        _scaffolder.Scaffold(_tempDir, "TestProject", force: false);

        var csprojPath = Path.Combine(_tempDir, "TestProject", "build", "build.csproj");
        Assert.True(File.Exists(csprojPath));

        var content = File.ReadAllText(csprojPath);
        Assert.Contains("Cake.Frosting", content);
    }

    // L2-REQ-005.3: Program.cs is created with CakeHost
    [Fact]
    public void Scaffold_CreatesProgramCs_WithCakeHost()
    {
        _scaffolder.Scaffold(_tempDir, "TestProject", force: false);

        var programPath = Path.Combine(_tempDir, "TestProject", "build", "Program.cs");
        Assert.True(File.Exists(programPath));

        var content = File.ReadAllText(programPath);
        Assert.Contains("CakeHost", content);
    }

    // L2-REQ-005.3: Solution file is created and references build.csproj
    [Fact]
    public void Scaffold_CreatesSolutionFile_ReferencesCsproj()
    {
        _scaffolder.Scaffold(_tempDir, "TestProject", force: false);

        var buildDir = Path.Combine(_tempDir, "TestProject", "build");
        var slnFiles = Directory.GetFiles(buildDir, "build.sln*");
        Assert.True(slnFiles.Length > 0, "Solution file should exist (build.sln or build.slnx)");

        var slnContent = File.ReadAllText(slnFiles[0]);
        Assert.Contains("build.csproj", slnContent);
    }

    // L2-REQ-005.3: Generated project compiles
    [Fact]
    public void Scaffold_GeneratedProject_DotNetBuildSucceeds()
    {
        _scaffolder.Scaffold(_tempDir, "TestProject", force: false);

        var buildDir = Path.Combine(_tempDir, "TestProject", "build");
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"build \"{buildDir}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(psi)!;
        process.WaitForExit(TimeSpan.FromSeconds(120));

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();

        Assert.True(process.ExitCode == 0,
            $"dotnet build failed with exit code {process.ExitCode}.\nStdout: {stdout}\nStderr: {stderr}");
    }

    // L2-REQ-005.4: Skill file is auto-installed
    [Fact]
    public void Scaffold_InstallsClaudeSkill()
    {
        _scaffolder.Scaffold(_tempDir, "TestProject", force: false);

        var skillPath = Path.Combine(_tempDir, "TestProject", "build", ".claude", "skills", "cake-cli.md");
        Assert.True(File.Exists(skillPath));
    }

    // L2-REQ-005.4: Skill content is valid
    [Fact]
    public void Scaffold_SkillContent_IsValid()
    {
        _scaffolder.Scaffold(_tempDir, "TestProject", force: false);

        var skillPath = Path.Combine(_tempDir, "TestProject", "build", ".claude", "skills", "cake-cli.md");
        var content = File.ReadAllText(skillPath);

        Assert.Contains("Cake CLI Skill", content);
        Assert.Contains("cake-cli create", content);
    }
}
