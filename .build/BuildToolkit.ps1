# Tool Versions
$NunitVersion = "3.12.0";
$OpenCoverVersion = "4.7.1221";
$DocFxVersion = "2.58.4";
$ReportGeneratorVersion = "4.8.13";
$OpenCoverToCoberturaVersion = "0.3.4";

# Folder Pathes
$RootPath = $MyInvocation.PSScriptRoot;
$BuildTools = "$RootPath\packages";

# Artifacts
$ArtifactsDir = "$RootPath\artifacts";

# Build
$BuildArtifacts = "$ArtifactsDir\Build";

# Documentation
$DocumentationDir = "$RootPath\docs";
$DocumentationArtifcacts = "$ArtifactsDir\Documentation";

# Tests
$NunitReportsDir = "$ArtifactsDir\Tests";
$OpenCoverReportsDir = "$ArtifactsDir\Tests"
$CoberturaReportsDir = "$ArtifactsDir\Tests"

# Licensing
$LicensingArtifacts = "$ArtifactsDir\Licensing";

# Nuget
$NugetConfig = "$RootPath\NuGet.Config";
$NugetPackageArtifacts = "$ArtifactsDir\Packages";

# Load partial scripts
. "$PSScriptRoot\Output.ps1";

# Define Tools
$global:DotNetCli = "dotnet.exe";
$global:GitCli = "";
$global:OpenCoverCli = "$BuildTools\OpenCover.$OpenCoverVersion\tools\OpenCover.Console.exe";
$global:NunitCli = "$BuildTools\NUnit.ConsoleRunner.$NunitVersion\tools\nunit3-console.exe";
$global:ReportGeneratorCli = "$BuildTools\ReportGenerator.$ReportGeneratorVersion\tools\net47\ReportGenerator.exe";
$global:DocFxCli = "$BuildTools\docfx.console.$DocFxVersion\tools\docfx.exe";
$global:OpenCoverToCoberturaCli = "$BuildTools\OpenCoverToCoberturaConverter.$OpenCoverToCoberturaVersion\tools\OpenCoverToCoberturaConverter.exe";

# Git
$global:GitCommitHash = "";

# Functions
function Invoke-Initialize([string]$Version = "1.0.0", [bool]$Cleanup = $False) {
    Write-Step "Initializing BuildToolkit"

    # First check the powershell version
    if ($PSVersionTable.PSVersion.Major -lt 5) {
        Write-Host ("The needed major powershell version for this script is 5. Your version: " + ($PSVersionTable.PSVersion.ToString()))
        exit 1;
    }

    # Assign git.exe
    $gitCommand = (Get-Command "git.exe" -ErrorAction SilentlyContinue);
    if ($null -eq $gitCommand)  { 
        Write-Host "Unable to find git.exe in your PATH. Download from https://git-scm.com";
        exit 1;
    }

    $global:GitCli = $gitCommand.Path;

    # Load Hash
    $global:GitCommitHash = (& $global:GitCli rev-parse HEAD);
    Invoke-ExitCodeCheck $LastExitCode;

    # Initialize Folders
    CreateFolderIfNotExists $BuildTools;
    CreateFolderIfNotExists $ArtifactsDir;

    # Environment Variable Defaults
    if (-not $env:MORYX_BUILDNUMBER) {
        $env:MORYX_BUILDNUMBER = 0;
    }

    if (-not $env:MORYX_BUILD_CONFIG) {
        $env:MORYX_BUILD_CONFIG = "Debug";
    }

    if (-not $env:MORYX_BUILD_VERBOSITY) {
        $env:MORYX_BUILD_VERBOSITY = "minimal"
    }

    if (-not $env:MORYX_TEST_VERBOSITY) {
        $env:MORYX_TEST_VERBOSITY = "normal"
    }

    if (-not $env:MORYX_NUGET_VERBOSITY) {
        $env:MORYX_NUGET_VERBOSITY = "normal"
    }

    if (-not $env:MORYX_OPTIMIZE_CODE) {
        $env:MORYX_OPTIMIZE_CODE = $True;
    }
    else {
        if (-not [bool]::TryParse($env:MORYX_OPTIMIZE_CODE,  [ref]$env:MORYX_OPTIMIZE_CODE)) {
            $env:MORYX_OPTIMIZE_CODE = $True;
        }
    }

    if (-not $env:MORYX_PACKAGE_TARGET) {
        $env:MORYX_PACKAGE_TARGET = "";
    }

    if (-not $env:MORYX_PACKAGE_TARGET_V3) {
        $env:MORYX_PACKAGE_TARGET_V3 = "";
    }

    if (-not $env:MORYX_ASSEMBLY_VERSION) {
        $env:MORYX_ASSEMBLY_VERSION = $Version;
    }

    if (-not $env:MORYX_FILE_VERSION) {
        $env:MORYX_FILE_VERSION = $Version;
    }

    if (-not $env:MORYX_INFORMATIONAL_VERSION) {
        $env:MORYX_INFORMATIONAL_VERSION = $Version;
    }

    if (-not $env:MORYX_PACKAGE_VERSION) {
        $env:MORYX_PACKAGE_VERSION = $Version;
    }
    
    Set-Version $Version;

    # Printing Variables
    Write-Step "Printing global variables"
    Write-Variable "RootPath" $RootPath;
    Write-Variable "DocumentationDir" $DocumentationDir;
    Write-Variable "NunitReportsDir" $NunitReportsDir;

    Write-Step "Printing global scope"
    Write-Variable "OpenCoverCli" $global:OpenCoverCli;
    Write-Variable "NUnitCli" $global:NUnitCli;
    Write-Variable "ReportGeneratorCli" $global:ReportGeneratorCli;
    Write-Variable "DocFxCli" $global:DocFxCli;
    Write-Variable "OpenCoverToCoberturaCli" $global:OpenCoverToCoberturaCli;
    Write-Variable "GitCli" $global:GitCli;
    Write-Variable "GitCommitHash" $global:GitCommitHash;

    Write-Step "Printing environment variables"
    Write-Variable "MORYX_OPTIMIZE_CODE" $env:MORYX_OPTIMIZE_CODE;
    Write-Variable "MORYX_BUILDNUMBER" $env:MORYX_BUILDNUMBER;
    Write-Variable "MORYX_BUILD_CONFIG" $env:MORYX_BUILD_CONFIG;
    Write-Variable "MORYX_BUILD_VERBOSITY" $env:MORYX_BUILD_VERBOSITY;
    Write-Variable "MORYX_TEST_VERBOSITY" $env:MORYX_TEST_VERBOSITY;
    Write-Variable "MORYX_NUGET_VERBOSITY" $env:MORYX_NUGET_VERBOSITY;
    Write-Variable "MORYX_PACKAGE_TARGET" $env:MORYX_PACKAGE_TARGET;
    Write-Variable "MORYX_PACKAGE_TARGET_V3" $env:MORYX_PACKAGE_TARGET_V3;

    Write-Variable "MORYX_ASSEMBLY_VERSION" $env:MORYX_ASSEMBLY_VERSION;
    Write-Variable "MORYX_FILE_VERSION" $env:MORYX_FILE_VERSION;
    Write-Variable "MORYX_INFORMATIONAL_VERSION" $env:MORYX_INFORMATIONAL_VERSION;
    Write-Variable "MORYX_PACKAGE_VERSION" $env:MORYX_PACKAGE_VERSION;


    # Cleanp
    if ($Cleanup) {
        Write-Step "Cleanup"

        Write-Host "Cleaning up repository ..." -ForegroundColor Red;
        & $global:GitCli clean -f -d -x
        Invoke-ExitCodeCheck $LastExitCode;

        & $global:GitCli checkout .
        Invoke-ExitCodeCheck $LastExitCode;
    }
}

function Set-Version ([string]$MajorMinorPatch) {
    $semVer2Regex = "^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";
    
    $version = Read-VersionFromRef($MajorMinorPatch);
    Write-Host "Setting environment version to $version";

    # Match semVer2 regex
    $regexMatch = [regex]::Match($version, $semVer2Regex);

    if (-not $regexMatch.Success) {
        Write-Host "Could not parse version: $version";
        Invoke-ExitCodeCheck 1;
    }

    # Extract groups
    $matchgroups = $regexMatch.captures.groups;
    $majorGroup = $matchgroups[1];
    $minorGroup = $matchgroups[2];
    $patchGroup = $matchgroups[3];
    $preReleaseGroup = $matchgroups[4];

    # Compose Major.Minor.Patch
    $mmp = $majorGroup.Value + "." + $minorGroup.Value + "." + $patchGroup.Value;

    # Check if it is a pre release
    $env:MORYX_ASSEMBLY_VERSION = $majorGroup.Value + ".0.0.0" # 3.0.0.0
    $env:MORYX_FILE_VERSION = $mmp + "." + $env:MORYX_BUILDNUMBER; # 3.1.2.42

    if ($preReleaseGroup.Success) {
        $env:MORYX_INFORMATIONAL_VERSION = $mmp + "-" + $preReleaseGroup.Value + "+" + $global:GitCommitHash; # 3.1.2-beta.1+d95a996ed5ba14a1421dafeb844a56ab08211ead
        $env:MORYX_PACKAGE_VERSION = $mmp + "-" + $preReleaseGroup.Value;
    } else {
        $env:MORYX_INFORMATIONAL_VERSION = $mmp + "+" + $global:GitCommitHash; # 3.1.2+d95a996ed5ba14a1421dafeb844a56ab08211ead
        $env:MORYX_PACKAGE_VERSION = $mmp;
    }
}

function Read-VersionFromRef([string]$MajorMinorPatch) {
    function preReleaseVersion ([string] $name)
    {
        $name = $name.Replace("/","").ToLower();
        return "$MajorMinorPatch-$name.$env:MORYX_BUILDNUMBER";;
    }

    $ref = "";
    if ($env:GITHUB_WORKFLOW) { # GitHub Workflow
        Write-Host "Reading version from 'GitHub Workflow'";
        $ref = $env:GITHUB_REF;

        if ($ref.StartsWith("refs/tags/")) {
            if ($ref.StartsWith("refs/tags/v")) {
                # Its a version tag
                $version = $ref.Replace("refs/tags/v","")
            } 
            else {
                # Just a tag
                $name = $ref.Replace("refs/tags/","");
                $version = = preReleaseVersion($name);
            }
        }
        elseif ($ref.StartsWith("refs/heads/")) {
            # Its a branch
            $name = $ref.Replace("refs/heads/","");
            $version = preReleaseVersion($name);
        } 
        else {
            $version = preReleaseVersion($ref);
        }
    }
    else { # Local build
        Write-Host "Reading version from 'local'";
        $ref = (& $global:GitCli rev-parse --abbrev-ref HEAD);
        $version = preReleaseVersion($ref);
    }

    return $version;
}

function Invoke-Cleanup {
    # Clean up
    Write-Step "Cleaning up repository ...";
    & $global:GitCli clean -f -d -x
    Invoke-ExitCodeCheck $LastExitCode;
}


function Invoke-Build([string]$ProjectFile, [string]$Options = "") {
    Write-Step "Building $ProjectFile"

    $additonalOptions = "";
    if (-not [string]::IsNullOrEmpty($Options)) {
        $additonalOptions = ",$Options";
    }

    $msbuildParams = "Optimize=" + (&{If($env:MORYX_OPTIMIZE_CODE -eq $True) {"true"} Else {"false"}}) + ",DebugSymbols=true$additonalOptions";
    $buildArgs = "--configuration", "$env:MORYX_BUILD_CONFIG";
    $buildArgs += "--verbosity", $env:MORYX_BUILD_VERBOSITY;
    $buildArgs += "-p:$msbuildParams,Version=$env:MORYX_PACKAGE_VERSION"
    & $global:DotNetCli build $ProjectFile @buildArgs

    Invoke-ExitCodeCheck $LastExitCode;

    Copy-Build-To-Artifacts $BuildArtifacts;
}

function Copy-Build-To-Artifacts([string]$TargetPath){
    ForEach($csprojItem in Get-ChildItem $SearchPath -Recurse -Include "*.csproj") { 

        $projectName = ([System.IO.Path]::GetFileNameWithoutExtension($csprojItem.Name));
        $assemblyPath = [System.IO.Path]::Combine($csprojItem.DirectoryName, "bin", $env:MORYX_BUILD_CONFIG);
        $assemblyArtifactPath = [System.IO.Path]::Combine($TargetPath, $projectName, "bin", $env:MORYX_BUILD_CONFIG);
        $objPath = [System.IO.Path]::Combine($csprojItem.DirectoryName, "obj");
        $objArtifactPath = [System.IO.Path]::Combine($TargetPath, $projectName, "obj");
        
        # Check if the project was build
        If(-not (Test-Path $assemblyPath)){ continue; }

        CopyAndReplaceFolder $assemblyPath $assemblyArtifactPath;
        CopyAndReplaceFolder $objPath $objArtifactPath;
        Write-Host "Copied build of $csprojItem to artifacts..." 
    }
}


function Invoke-Licensing($SearchPath = $RootPath) {
    Write-Step "Licensing"
    # Assign AxProtector.exe
    $AxProtectorNetCommand = (Get-Command "AxProtectorNet.exe" -ErrorAction SilentlyContinue);
    if ($null -eq $AxProtectorNetCommand)  {
        Write-Error "Unable to find AxProtectorNet.exe in your PATH. Download from https://www.wibu.com/support/developer/downloads-developer-software.html"
        exit 1;
    }
    $axProtectorNetCli = $AxProtectorNetCommand.Path;
    $axProtectorParent = Split-Path $axProtectorNetCli -Parent
    $axProtectorNetCoreCli = [System.IO.Path]::Combine($axProtectorParent, "netstandard2.0","AxProtectorNet.exe");

    # Look for csproj in this directory
    $csprojItems = Get-ChildItem $SearchPath -Recurse -Include "*.csproj"
    if ($csprojItems.Length -eq 0) {
        Write-Host-Warning "No project to license found!"
        return;
    }

    $licenseCreationDate = Get-Date -Format "yyyy-MM-dd"
    ForEach($csprojItem in $csprojItems ) { 
        $projectName = ([System.IO.Path]::GetFileNameWithoutExtension($csprojItem.Name));
        if (IsLicensedProject $csprojItem){
            # Check if any assembly is available for licensing
            Write-Step "Trying to create licensed packages for $projectName..."
            $assemblyArtifactPath = [System.IO.Path]::Combine($BuildArtifacts, $projectName, "bin");
            $assemlbies = Get-ChildItem $assemblyArtifactPath -Recurse -Include "$projectName.dll" | ForEach-Object { if (!($_ -Match "\\ref\\" -or $_ -Match "/ref/")) { $_}}
            if ($assemlbies.Length -eq 0) {
                Write-Host-Warning "No assemblies found for licensing. Make sure the projects are build."
                exit 1;
            }
            
            # Update license config to include release date
            Write-Host "Updating release date to $licenseCreationDate for $projectName assemblies..."
            $licensingConfig = [System.IO.Path]::Combine($csprojItem.DirectoryName, "AxProtector_$projectName.xml");
            [xml]$licenseConfigContent = Get-Content $licensingConfig
            $licenseConfigContent.AxProtectorNet.CommandLine.ChildNodes | 
                                    Where-Object{$_.InnerText -Match "-rd:"} | 
                                    ForEach-Object{$_.InnerText="-rd:$licenseCreationDate,00:00:00"}
            Set-Content $licensingConfig $licenseConfigContent.OuterXml

            # Create a licensed assembly for each assembly available 
            Write-Host "Creating licensed assembly for $projectName..."
            ForEach($assembly in $assemlbies) {
                $assemblyTarget = GetTargetFrameworkDir $assembly
                $licensedAssembly = [System.IO.Path]::Combine($LicensingArtifacts, $projectName, "bin", $env:MORYX_BUILD_CONFIG, $assemblyTarget, "$projectName.dll");
                
                Write-Host "... licensing $assembly"
                & $axProtectorNetCoreCli "@$licensingConfig" -o:$licensedAssembly $assembly

                if (-not (Test-Path $licensedAssembly)) {
                    Write-Host-Error "Failed to create licensed assembly."
                    exit 1;
                }
                else {
                    Write-Host-Success "Licensed Assembly created successfully."
                }
            }
        } else {
            Write-Host-Warning "Skipping $projectName"
        }
    }
}

function IsLicensedProject($CsprojItem){
    $projectName = ([System.IO.Path]::GetFileNameWithoutExtension($CsprojItem.Name));
    $licensingConfig = [System.IO.Path]::Combine($CsprojItem.DirectoryName, "AxProtector_$projectName.xml");
    return Test-Path $licensingConfig
}

function GetTargetFrameworkDir([string]$Assembly){
    $assemblyTargetFramework = Split-Path (Split-Path $assembly -Parent) -Leaf
    if ($assemblyTargetFramework -eq $env:MORYX_BUILD_CONFIG) {
        return ""
    }
    else {
        return $assemblyTargetFramework
    }
}


## Pack
function Invoke-PackAll([switch]$Symbols = $False, [bool]$IsTool = $False) {
    CreateFolderIfNotExists $NugetPackageArtifacts;

    Write-Host "Looking for .csproj files..."
    # Look for csproj in this directory
    foreach ($csprojItem in Get-ChildItem $RootPath -Recurse -Filter *.csproj) {
        Invoke-Pack $csprojItem $Symbols -IsTool $IsTool
    }
}

function Invoke-Pack($ProjectItem, [bool]$IsTool = $False, [bool]$IncludeSymbols = $False) {
    CreateFolderIfNotExists (&{If($env:MORYX_COMMERCIAL_BUILD -eq $True) {$NugetCommercialPackageArtifacts} Else {$NugetPackageArtifacts}});

    if (Get-CsprojIsSdkProject($ProjectItem)) {
        Invoke-PackSdkProject $ProjectItem $IncludeSymbols;
    }
    else {
        Invoke-PackFrameworkProject $ProjectItem $IsTool $IncludeSymbols;
    }
}

function Invoke-PackSdkProject($CsprojItem, [bool]$IncludeSymbols = $False) {
    Write-Host "Try to pack .NET SDK project: $($CsprojItem.Name) ...";

    # Check if the project should be packed
    $csprojFullName = $CsprojItem.FullName;

    $packargs = "--output", "$NugetPackageArtifacts";
    $packargs += "--configuration", "$env:MORYX_BUILD_CONFIG";
    $packargs += "--verbosity", "$env:MORYX_NUGET_VERBOSITY";
    $packargs += "--no-build";

    if ($IncludeSymbols) {
        $packargs += "--include-symbols";
        $packargs += "--include-source";
    }

    $packargs += "-p:Version=$env:MORYX_FILE_VERSION"

    & $global:DotNetCli pack "$csprojFullName" @packargs
    Invoke-ExitCodeCheck $LastExitCode;
}

function Get-CsprojIsSdkProject($CsprojItem) {
    [xml]$csprojContent = Get-Content $CsprojItem.FullName
    $sdkProject = $csprojContent.Project.Sdk;
    if ($null -ne $sdkProject) {
        return $true;
    }
    return $false;
}


## Helper
function CreateFolderIfNotExists([string]$Folder) {
    if (-not (Test-Path $Folder)) {
        Write-Host "Creating missing directory '$Folder'"
        New-Item $Folder -Type Directory | Out-Null
    }
}

function CopyAndReplaceFolder($SourceDir, $TargetDir) {
    Write-Host "Copy from $SourceDir to $TargetDir ..." -ForegroundColor Green

    # Remove old folder if exists
    if (Test-Path $TargetDir) {
        Write-Host "Target path already exists, replacing ..." -ForegroundColor Yellow
        Remove-Item -Recurse -Force $TargetDir
    }

    # Copy to target path
    Copy-Item -Path $SourceDir -Recurse -Destination $TargetDir -Container
}
