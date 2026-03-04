using Cake.Cli.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-004.2 & L2-REQ-004.3: Skill file generation and content tests
/// </summary>
public class SkillInstallerTests : IDisposable
{
    private readonly string _tempDir;
    private readonly ISkillInstaller _installer;

    public SkillInstallerTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
        var logger = NullLogger<SkillInstaller>.Instance;
        _installer = new SkillInstaller(logger);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    // L2-REQ-004.2: Skill file is created in target directory
    [Fact]
    public void Install_CreatesSkillFileInTargetDirectory()
    {
        // Act
        var result = _installer.Install(_tempDir, force: false);

        // Assert
        Assert.Equal(0, result);
        var skillPath = Path.Combine(_tempDir, ".claude", "skills", "cake-cli.md");
        Assert.True(File.Exists(skillPath));
    }

    // L2-REQ-004.2: Existing file is NOT overwritten without --force
    [Fact]
    public void Install_ExistingFile_WithoutForce_DoesNotOverwrite()
    {
        // Arrange
        var skillDir = Path.Combine(_tempDir, ".claude", "skills");
        Directory.CreateDirectory(skillDir);
        var skillPath = Path.Combine(skillDir, "cake-cli.md");
        File.WriteAllText(skillPath, "original content");

        // Act
        var result = _installer.Install(_tempDir, force: false);

        // Assert
        Assert.NotEqual(0, result);
        Assert.Equal("original content", File.ReadAllText(skillPath));
    }

    // L2-REQ-004.2: --force overwrites existing file
    [Fact]
    public void Install_ExistingFile_WithForce_Overwrites()
    {
        // Arrange
        var skillDir = Path.Combine(_tempDir, ".claude", "skills");
        Directory.CreateDirectory(skillDir);
        var skillPath = Path.Combine(skillDir, "cake-cli.md");
        File.WriteAllText(skillPath, "original content");

        // Act
        var result = _installer.Install(_tempDir, force: true);

        // Assert
        Assert.Equal(0, result);
        var content = File.ReadAllText(skillPath);
        Assert.NotEqual("original content", content);
        Assert.Contains("Cake CLI Skill", content);
    }

    // L2-REQ-004.3: Skill content contains all command descriptions
    [Fact]
    public void Install_SkillContent_ContainsCommandDescriptions()
    {
        // Act
        _installer.Install(_tempDir, force: false);

        // Assert
        var skillPath = Path.Combine(_tempDir, ".claude", "skills", "cake-cli.md");
        var content = File.ReadAllText(skillPath);

        Assert.Contains("cake-cli create --name <project-name>", content);
        Assert.Contains("cake-cli install-skill", content);
        Assert.Contains("--force", content);
        Assert.Contains("--name", content);
    }

    // L2-REQ-004.3: Skill content includes examples
    [Fact]
    public void Install_SkillContent_ContainsExamples()
    {
        // Act
        _installer.Install(_tempDir, force: false);

        // Assert
        var skillPath = Path.Combine(_tempDir, ".claude", "skills", "cake-cli.md");
        var content = File.ReadAllText(skillPath);

        Assert.Contains("## Examples", content);
        Assert.Contains("cake-cli create --name MyApp", content);
        Assert.Contains("cake-cli install-skill --force", content);
    }

    // L2-REQ-004.3: Skill content references "cake-cli" correctly
    [Fact]
    public void Install_SkillContent_ReferencesCakeCliCorrectly()
    {
        // Act
        _installer.Install(_tempDir, force: false);

        // Assert
        var skillPath = Path.Combine(_tempDir, ".claude", "skills", "cake-cli.md");
        var content = File.ReadAllText(skillPath);

        Assert.Contains("cake-cli", content);
        Assert.Contains("# Cake CLI Skill", content);
        Assert.Contains("## When to Use", content);
    }
}
