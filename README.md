# MIPSCore

A MIPS processor simulator written in C#.

## Releases

Pre-built releases are automatically published by the CI/CD pipeline after every merged pull request.

➡️ **[Download the latest release](https://github.com/DavidBechtold/MIPSCore/releases)**

## Build

Requirements: Visual Studio / MSBuild, NuGet, .NET Framework 4.5

```
nuget restore MIPSCore.sln
msbuild MIPSCore.sln /p:Configuration=Release
```

## Usage

Run the console application with an `.objdump` file:

```
MIPSCoreConsole.exe -p <path-to-objdump>
```
