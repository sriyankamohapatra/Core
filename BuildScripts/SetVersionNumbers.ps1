##-----------------------------------------------------------------------
## <copyright file="ApplyVersionToAssemblies.ps1">(c) Microsoft Corporation. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
##-----------------------------------------------------------------------
# Look for a 0.0.0.0 pattern in the build number. 
# If found use it to version the assemblies.
#
# For example, if the 'Build number format' build process parameter 
# $(BuildDefinitionName)_$(Year:yyyy).$(Month).$(DayOfMonth)$(Rev:.r)
# then your build numbers come out like this:
# "Build HelloWorld_2013.07.19.1"
# This script would then apply version 2013.07.19.1 to your assemblies.

# Enable -Verbose option
[CmdletBinding()]

# Regular expression pattern to find the version in the build number 
# and then apply it to the assemblies
$PartVersionRegex = "\d+\.\d+\.\d+"
$VersionRegex = "\d+\.\d+\.\d+\.\d+"

$AssmeblyVersionRegEx = "AssemblyVersion\(\""\d+\.\d+\.\d+\.\d+\""\)"
$AssmeblyFileVersionRegEx = "AssemblyFileVersion\(\""\d+\.\d+\.\d+\.\d+\""\)"

# If this script is not running on a build server, remind user to 
# set environment variables so that this script can be debugged
if(-not ($Env:BUILD_SOURCESDIRECTORY -and $Env:BUILD_VERSION_MAJOR -and $ENV:BUILD_VERSION_MINOR -and $ENV:BUILD_VERSION_SPRINT -and $ENV:BUILD_SOURCEVERSION ))
{
    Write-Error "You must set the following environment variables"
    Write-Error "to test this script interactively."
    Write-Host '$Env:BUILD_SOURCESDIRECTORY - For example, enter something like:'
    Write-Host '$Env:BUILD_SOURCESDIRECTORY = "C:\code\FabrikamTFVC\HelloWorld"'
    Write-Host '$Env:BUILD_VERSION_MAJOR - A number'
    Write-Host '$Env:BUILD_VERSION_MINOR - A number'
    Write-Host '$Env:BUILD_VERSION_SPRINT - A number'
    Write-Host '$Env:BUILD_SOURCEVERSION - For example, enter a number:'
    Write-Host '$Env:BUILD_SOURCEVERSION = "CS0001"'
    exit 1
}

# Make sure path to source code directory is available
if (-not $Env:BUILD_SOURCESDIRECTORY)
{
    Write-Error ("BUILD_SOURCESDIRECTORY environment variable is missing.")
    exit 1
}
elseif (-not (Test-Path $Env:BUILD_SOURCESDIRECTORY))
{
    Write-Error "BUILD_SOURCESDIRECTORY does not exist: $Env:BUILD_SOURCESDIRECTORY"
    exit 1
}
Write-Verbose "BUILD_SOURCESDIRECTORY: $Env:BUILD_SOURCESDIRECTORY"

# Make sure path to source code directory is available
if (-not $Env:BUILD_SOURCEVERSION)
{
    Write-Error ("BUILD_SOURCEVERSION environment variable is missing.")
    exit 1
}
elseif (-not $Env:BUILD_SOURCEVERSION)
{
    Write-Error "BUILD_SOURCEVERSION does not exist: $Env:BUILD_SOURCEVERSION"
    exit 1
}
Write-Verbose "BUILD_SOURCEVERSION: $Env:BUILD_SOURCEVERSION"

$sourceVersion = $Env:BUILD_SOURCEVERSION -replace "[\D]", ""

# Make sure there is a major build number
if (-not $Env:BUILD_VERSION_MAJOR)
{
    Write-Error ("BUILD_VERSION_MAJOR environment variable is missing.")
    exit 1
}
Write-Verbose "BUILD_VERSION_MAJOR: $Env:BUILD_VERSION_MAJOR"

# Get and validate the major version data
$VersionMajor = [regex]::matches($Env:BUILD_VERSION_MAJOR,"\d+")
switch($VersionMajor.Count)
{
   0        
      { 
         Write-Error "Could not find version number data in BUILD_VERSION_MAJOR: $Env:BUILD_VERSION_MAJOR"
         exit 1
      }
   1 {}
   default 
      { 
         Write-Warning "Found more than instance of version data in BUILD_VERSION_MAJOR." 
         Write-Warning "Will assume first instance is version."
      }
}

# Make sure there is a minor build number
if (-not $Env:BUILD_VERSION_MINOR)
{
    Write-Error ("BUILD_VERSION_MINOR environment variable is missing.")
    exit 1
}
Write-Verbose "BUILD_VERSION_MINOR: $Env:BUILD_VERSION_MINOR"

# Get and validate the minor version data
$VersionMinor = [regex]::matches($Env:BUILD_VERSION_MINOR,"\d+")
switch($VersionMinor.Count)
{
   0        
      { 
         Write-Error "Could not find version number data in BUILD_VERSION_MINOR: $Env:BUILD_VERSION_MINOR"
         exit 1
      }
   1 {}
   default 
      { 
         Write-Warning "Found more than instance of version data in BUILD_VERSION_MINOR." 
         Write-Warning "Will assume first instance is version."
      }
}

# Make sure there is a sprint build number
if (-not $Env:BUILD_VERSION_SPRINT)
{
    Write-Error ("BUILD_VERSION_SPRINT environment variable is missing.")
    exit 1
}
Write-Verbose "BUILD_VERSION_SPRINT: $Env:BUILD_VERSION_SPRINT"

# Get and validate the sprint version data
$VersionSprint = [regex]::matches($Env:BUILD_VERSION_SPRINT,"\d+")
switch($VersionSprint.Count)
{
   0        
      { 
         Write-Error "Could not find version number data in BUILD_VERSION_SPRINT: $Env:BUILD_VERSION_SPRINT"
         exit 1
      }
   1 {}
   default 
      { 
         Write-Warning "Found more than instance of version data in BUILD_VERSION_SPRINT." 
         Write-Warning "Will assume first instance is version."
      }
}

$VersionMajorValue = $VersionMajor[0]
$VersionMinorValue = $VersionMinor[0]
$VersionSprintValue = $VersionSprint[0]

$NewAssemblyVersion = "$VersionMajorValue.$VersionMinorValue.$VersionSprintValue.0"
Write-Verbose "Assembly Version: $NewAssemblyVersion"

$NewAssemblyFileVersion = "$VersionMajorValue.$VersionMinorValue.$VersionSprintValue.$sourceVersion"
Write-Verbose "Assembly File Version: $NewAssemblyFileVersion"

# Apply the version to the assembly property files
#$files = gci $Env:BUILD_SOURCESDIRECTORY -recurse -include "*Properties*","My Project" | 
$files = gci $Env:BUILD_SOURCESDIRECTORY -recurse | 
    ?{ $_.PSIsContainer } | 
    foreach { gci -Path $_.FullName -Recurse -include CommonAssemblyInfo.* }

$FullNewAssemblyVersion = "AssemblyVersion(""" + $NewAssemblyVersion + """)"
$FullNewAssemblyFileVersion = "AssemblyFileVersion(""" + $NewAssemblyFileVersion + """)"

if($files)
{
    Write-Verbose "Will apply $NewAssemblyVersion and $NewAssemblyFileVersion to $($files.count) files."

    foreach ($file in $files) {
        $filecontent = Get-Content($file)
        attrib $file -r
        $filecontent -replace $AssmeblyVersionRegEx, $FullNewAssemblyVersion | Out-File $file
        $filecontent = Get-Content($file)
        attrib $file -r
        $filecontent -replace $AssmeblyFileVersionRegEx, $FullNewAssemblyFileVersion | Out-File $file
        Write-Verbose "$file.FullName - version applied"
    }
}
else
{
    Write-Warning "Found no files."
}