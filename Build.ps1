param (
    [switch]$Build,
    [switch]$Licensing,
    [switch]$Pack,
    [switch]$Publish
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

if ($Publish) {
    dotnet nuget push "artifacts/packages/" --api-key $MORYX_NUGET_APIKEY --source $MORYX_PACKAGE_TARGET --skip-duplicate --symbol-api-key $MORYX_NUGET_APIKEY --symbol-source $MORYX_PACKAGE_TARGET_V3
}

Write-Host "Success!"
