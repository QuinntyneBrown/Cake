using Cake.Cli.Templates;
using Xunit;

namespace Cake.Cli.Tests;

/// <summary>
/// L2-REQ-002.1: Cake Frosting Dependency — template content tests
/// L2-REQ-006.4: Template Provider for Generated Program.cs tests
/// </summary>
public class BuildProjectTemplateProviderTests
{
    private readonly BuildProjectTemplateProvider _provider = new();

    [Fact]
    public void BuildCsproj_ContainsCakeFrostingPackageReference()
    {
        var csproj = _provider.GetBuildCsproj();

        Assert.Contains("Cake.Frosting", csproj);
        Assert.Contains("PackageReference", csproj);
    }

    [Fact]
    public void BuildCsproj_TargetsNet8()
    {
        var csproj = _provider.GetBuildCsproj();

        Assert.Contains("net8.0", csproj);
    }

    [Fact]
    public void ProgramCs_ContainsCakeHost()
    {
        var programCs = _provider.GetBuildProgramCs();

        Assert.Contains("CakeHost", programCs);
    }

    [Fact]
    public void ProgramCs_UsesCakeFrostingNamespace()
    {
        var programCs = _provider.GetBuildProgramCs();

        Assert.Contains("Cake.Frosting", programCs);
    }

    // L2-REQ-006.4: GetBuildProgramCsWithSolutionGeneration tests

    [Fact]
    public void WithSolutionGeneration_ContainsGenerateSolutionTask()
    {
        var result = _provider.GetBuildProgramCsWithSolutionGeneration(
            "https://github.com/user/MyLib.git",
            new[] { "MyLib.Core", "MyLib.Api" });

        Assert.Contains("GenerateSolutionTask", result);
    }

    [Fact]
    public void WithSolutionGeneration_ContainsGitUrl()
    {
        var gitUrl = "https://github.com/user/MyLib.git";

        var result = _provider.GetBuildProgramCsWithSolutionGeneration(
            gitUrl,
            new[] { "MyLib.Core" });

        Assert.Contains(gitUrl, result);
    }

    [Fact]
    public void WithSolutionGeneration_ContainsProjectNames()
    {
        var result = _provider.GetBuildProgramCsWithSolutionGeneration(
            "https://github.com/user/MyLib.git",
            new[] { "MyLib.Core", "MyLib.Api", "MyLib.Tests" });

        Assert.Contains("MyLib.Core", result);
        Assert.Contains("MyLib.Api", result);
        Assert.Contains("MyLib.Tests", result);
    }

    [Fact]
    public void WithSolutionGeneration_ContainsRequiredUsingDirectives()
    {
        var result = _provider.GetBuildProgramCsWithSolutionGeneration(
            "https://github.com/user/MyLib.git",
            new[] { "MyLib.Core" });

        Assert.Contains("using System.Diagnostics;", result);
        Assert.Contains("using Cake.Core;", result);
        Assert.Contains("using Cake.Core.Diagnostics;", result);
        Assert.Contains("using Cake.Frosting;", result);
    }

    [Fact]
    public void WithSolutionGeneration_ContainsDependencyOnGenerateSolution()
    {
        var result = _provider.GetBuildProgramCsWithSolutionGeneration(
            "https://github.com/user/MyLib.git",
            new[] { "MyLib.Core" });

        Assert.Contains("IsDependentOn(typeof(GenerateSolutionTask))", result);
    }

    [Fact]
    public void WithSolutionGeneration_DerivesRepoNameFromGitUrl()
    {
        var result = _provider.GetBuildProgramCsWithSolutionGeneration(
            "https://github.com/user/MyLib.git",
            new[] { "MyLib.Core" });

        Assert.Contains("\"MyLib\"", result);
    }

    [Fact]
    public void WithSolutionGeneration_ContainsCakeHostSetup()
    {
        var result = _provider.GetBuildProgramCsWithSolutionGeneration(
            "https://github.com/user/MyLib.git",
            new[] { "MyLib.Core" });

        Assert.Contains("CakeHost", result);
        Assert.Contains(".UseContext<BuildContext>()", result);
        Assert.Contains(".Run(args)", result);
    }

    [Fact]
    public void WithSolutionGeneration_ContainsRootDirectoryProperty()
    {
        var result = _provider.GetBuildProgramCsWithSolutionGeneration(
            "https://github.com/user/MyLib.git",
            new[] { "MyLib.Core" });

        Assert.Contains("RootDirectory", result);
        Assert.Contains("new DirectoryInfo(System.Environment.CurrentDirectory).Parent!.FullName", result);
    }

    [Fact]
    public void WithSolutionGeneration_ContainsRunProcessHelper()
    {
        var result = _provider.GetBuildProgramCsWithSolutionGeneration(
            "https://github.com/user/MyLib.git",
            new[] { "MyLib.Core" });

        Assert.Contains("private static void RunProcess(", result);
    }

    [Fact]
    public void WithSolutionGeneration_MultipleProjects_SearchesEachCsproj()
    {
        var result = _provider.GetBuildProgramCsWithSolutionGeneration(
            "https://github.com/user/MyLib.git",
            new[] { "MyLib.Core", "MyLib.Api" });

        Assert.Contains("MyLib.Core.csproj", result);
        Assert.Contains("MyLib.Api.csproj", result);
    }
}
