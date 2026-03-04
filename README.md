# Cake CLI

A .NET CLI tool that extends [Cake Build](https://cakebuild.net/) with project scaffolding and Claude skill installation.

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
