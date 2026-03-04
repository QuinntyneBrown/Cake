using Cake.Cli.Services.CodeGeneration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-003.2: DotNet code generator tests
/// </summary>
public class DotNetCodeGeneratorTests
{
    [Fact]
    public void IDotNetCodeGenerator_ResolvesFromDI()
    {
        var (services, _) = Program.BuildServiceProvider(Array.Empty<string>());

        var generator = services.GetService<IDotNetCodeGenerator>();

        Assert.NotNull(generator);
        Assert.IsType<DotNetCodeGenerator>(generator);
    }

    [Fact]
    public void GenerateSolution_CreatesSlnFile()
    {
        var generator = new DotNetCodeGenerator();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            generator.GenerateSolution("TestSolution", tempDir);
            Assert.True(File.Exists(Path.Combine(tempDir, "TestSolution", "TestSolution.sln")));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void GenerateProject_CreatesCsprojFile()
    {
        var generator = new DotNetCodeGenerator();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            generator.GenerateProject("TestProject", tempDir, "classlib");
            Assert.True(File.Exists(Path.Combine(tempDir, "TestProject", "TestProject.csproj")));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void GenerateSourceFile_CreatesFileWithContent()
    {
        var generator = new DotNetCodeGenerator();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            generator.GenerateSourceFile("Test.cs", tempDir, "namespace Test;");
            var filePath = Path.Combine(tempDir, "Test.cs");
            Assert.True(File.Exists(filePath));
            Assert.Equal("namespace Test;", File.ReadAllText(filePath));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}
