using Cake.Cli.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-006.5: Build Program Modifier Service tests
/// </summary>
public class BuildProgramModifierTests : IDisposable
{
    private readonly string _tempDir;
    private readonly IBuildProgramModifier _modifier;

    public BuildProgramModifierTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "CakeTest_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);

        var logger = NullLogger<BuildProgramModifier>.Instance;
        _modifier = new BuildProgramModifier(logger);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            try { Directory.Delete(_tempDir, recursive: true); }
            catch { /* best effort cleanup */ }
        }
    }

    // L2-REQ-006.5: Modify succeeds with valid build directory
    [Fact]
    public void Modify_ValidBuildDirectory_ReturnsSuccess()
    {
        File.WriteAllText(Path.Combine(_tempDir, "build.csproj"), "<Project />");
        File.WriteAllText(Path.Combine(_tempDir, "Program.cs"), "// original");

        var result = _modifier.Modify(_tempDir, "https://github.com/user/MyLib.git", new[] { "MyLib.Core" });

        Assert.Equal(0, result);
    }

    // L2-REQ-006.2: Fails when no .csproj found
    [Fact]
    public void Modify_NoCsproj_ReturnsFailure()
    {
        File.WriteAllText(Path.Combine(_tempDir, "Program.cs"), "// original");

        var result = _modifier.Modify(_tempDir, "https://github.com/user/MyLib.git", new[] { "MyLib.Core" });

        Assert.NotEqual(0, result);
    }

    // L2-REQ-006.2: Fails when no Program.cs found
    [Fact]
    public void Modify_NoProgramCs_ReturnsFailure()
    {
        File.WriteAllText(Path.Combine(_tempDir, "build.csproj"), "<Project />");

        var result = _modifier.Modify(_tempDir, "https://github.com/user/MyLib.git", new[] { "MyLib.Core" });

        Assert.NotEqual(0, result);
    }

    // L2-REQ-006.5: Written Program.cs contains expected content
    [Fact]
    public void Modify_WritesProgramCs_WithGenerateSolutionTask()
    {
        File.WriteAllText(Path.Combine(_tempDir, "build.csproj"), "<Project />");
        File.WriteAllText(Path.Combine(_tempDir, "Program.cs"), "// original");

        _modifier.Modify(_tempDir, "https://github.com/user/MyLib.git", new[] { "MyLib.Core", "MyLib.Api" });

        var content = File.ReadAllText(Path.Combine(_tempDir, "Program.cs"));
        Assert.Contains("GenerateSolutionTask", content);
        Assert.Contains("https://github.com/user/MyLib.git", content);
        Assert.Contains("MyLib.Core", content);
        Assert.Contains("MyLib.Api", content);
    }

    // L2-REQ-006.5: DI resolution
    [Fact]
    public void DI_ResolvesIBuildProgramModifier()
    {
        var (services, _) = Program.BuildServiceProvider(Array.Empty<string>());

        var modifier = services.GetRequiredService<IBuildProgramModifier>();

        Assert.NotNull(modifier);
        Assert.IsType<BuildProgramModifier>(modifier);
    }
}
