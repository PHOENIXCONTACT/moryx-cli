param (
    [switch]$Build,
    [switch]$Licensing,
    [switch]$Pack
)

# Load Toolkit
. ".build\BuildToolkit.ps1"

# Set MSBuild to latest version
$MsBuildVersion = "latest";

# Initialize Toolkit
Invoke-Initialize -Version (Get-Content "VERSION");

if ($Build) {
    Invoke-Build ".\Moryx.Cli.sln"
}

if ($Licensing) {
    Invoke-Licensing
}

if ($Pack) {
    Invoke-PackAll -Symbols
}

Write-Host "Success!"