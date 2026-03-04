@echo off
setlocal

set SOLUTION_DIR=%~dp0..\..
set NUPKG_DIR=%SOLUTION_DIR%\nupkg
set PROJECT=src\Cake.Cli\Cake.Cli.csproj
set PACKAGE_ID=QuinntyneBrown.Cake.Cli

echo Packing %PROJECT%...
dotnet pack "%SOLUTION_DIR%\%PROJECT%" -o "%NUPKG_DIR%" --nologo
if %errorlevel% neq 0 (
    echo ERROR: Pack failed.
    exit /b %errorlevel%
)

echo Uninstalling existing tool (if any)...
dotnet tool uninstall -g %PACKAGE_ID% 2>nul

echo Installing %PACKAGE_ID%...
dotnet tool install -g %PACKAGE_ID% --add-source "%NUPKG_DIR%" --version 0.1.0
if %errorlevel% neq 0 (
    echo ERROR: Install failed.
    exit /b %errorlevel%
)

echo.
echo cake-cli installed successfully. Run "cake-cli --help" to get started.
