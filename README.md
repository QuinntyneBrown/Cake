# Cake CLI

A .NET CLI tool that extends [Cake Build](https://cakebuild.net/) with project scaffolding, solution generation, and Claude skill installation.

Built with `System.CommandLine`, Microsoft Extensions (DI, Logging, Configuration), and integrations for `QuinntyneBrown.Git.Core`, `QuinntyneBrown.CodeGenerator.DotNet`, and `QuinntyneBrown.CodeGenerator.Angular`.

## Install

```bat
eng\scripts\install-cli.bat
```

Or manually:

```bash
dotnet pack src/Cake.Cli/Cake.Cli.csproj -o ./nupkg
dotnet tool install -g QuinntyneBrown.Cake.Cli --add-source ./nupkg --version 0.1.0
```

## Commands

### `cake-cli create --name <project-name>`

Scaffolds a new project with a Cake Frosting build setup.

```bash
cake-cli create --name MyApp
```

Creates:

```
MyApp/
  build/
    build.slnx
    build.csproj        (Cake.Frosting, net8.0)
    Program.cs           (CakeHost entry point)
    .claude/skills/
      cake-cli.md        (Claude skill)
```

Options:

- `--name` (required) — project folder name
- `--force` — overwrite existing directory

### `cake-cli add-project --git-url <url> --projects <names...>`

Modifies the Cake Frosting `Program.cs` in the current build directory to include solution generation logic. Must be run from inside a cake project's `build/` directory.

When the generated build program runs, it will:
1. Create a solution file in the project root (parent of `build/`) if none exists
2. Clone the specified git repository into `src/{repoName}/`
3. Add the named .NET projects from the cloned repo to the solution

```bash
cd MyApp/build
cake-cli add-project --git-url https://github.com/user/repo.git --projects MyLib.Core MyLib.Api
dotnet run
```

Options:

- `--git-url` (required) — git repository URL to clone
- `--projects` (required) — one or more .NET project names to add to the solution

### `cake-cli install-skill`

Installs the Claude skill file into the current directory at `.claude/skills/cake-cli.md`.

```bash
cake-cli install-skill
cake-cli install-skill --force
```

Options:

- `--force` — overwrite existing skill file

### Global Options

- `--verbosity <level>` — set log verbosity (`Quiet`, `Minimal`, `Normal`, `Verbose`, `Diagnostic`)
- `--version` — display version
- `--help` — display help

## Build

```bash
dotnet build Cake.Cli.slnx
```

## Test

```bash
dotnet test Cake.Cli.slnx
```

## Project Structure

```
src/Cake.Cli/           CLI tool source
tests/Cake.Cli.Tests/   Acceptance tests (xunit)
eng/scripts/            Build and install scripts
docs/specs/             L1/L2 requirements
```
