using Cake.Cli.Services.CodeGeneration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-003.3: Angular code generator tests
/// </summary>
public class AngularCodeGeneratorTests
{
    [Fact]
    public void IAngularCodeGenerator_ResolvesFromDI()
    {
        var (services, _) = Program.BuildServiceProvider(Array.Empty<string>());

        var generator = services.GetService<IAngularCodeGenerator>();

        Assert.NotNull(generator);
        Assert.IsType<AngularCodeGenerator>(generator);
    }

    [Fact]
    public void GenerateComponent_CreatesComponentFiles()
    {
        var generator = new AngularCodeGenerator();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            generator.GenerateComponent("my-component", tempDir);
            var componentDir = Path.Combine(tempDir, "my-component");
            Assert.True(File.Exists(Path.Combine(componentDir, "my-component.component.ts")));
            Assert.True(File.Exists(Path.Combine(componentDir, "my-component.component.html")));
            Assert.True(File.Exists(Path.Combine(componentDir, "my-component.component.css")));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void GenerateModule_CreatesModuleFile()
    {
        var generator = new AngularCodeGenerator();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            generator.GenerateModule("my-module", tempDir);
            Assert.True(File.Exists(Path.Combine(tempDir, "my-module", "my-module.module.ts")));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void GenerateService_CreatesServiceFile()
    {
        var generator = new AngularCodeGenerator();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            generator.GenerateService("my-service", tempDir);
            Assert.True(File.Exists(Path.Combine(tempDir, "my-service.service.ts")));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}
