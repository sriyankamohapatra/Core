# Enable -Verbose option
[CmdletBinding()]

[Regex] $AssemblyFileVersionRegEx = '(AssemblyFileVersion\(\"\d+\.\d+\.\d+\.)(\d+)(\"\))'
[Regex] $AssemblyInformationalVersionRegEx = '(AssemblyInformationalVersion\(\"\d+\.\d+\.\d+\.\d+)([\S]+)(\"\))'

# If this script is not running on a build server, remind user to 
# set environment variables so that this script can be debugged
if(-not ($Env:BUILD_SOURCESDIRECTORY -and $ENV:BUILD_BUILDNUMBER ))
{
    Write-Error "You must set the following environment variables"
    Write-Error "to test this script interactively."
    Write-Host '$Env:BUILD_SOURCESDIRECTORY - For example, enter something like:'
    Write-Host '$Env:BUILD_SOURCESDIRECTORY = "C:\code\FabrikamTFVC\HelloWorld"'
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
Write-Debug "BUILD_SOURCESDIRECTORY: $Env:BUILD_SOURCESDIRECTORY"

# Make sure path to source code directory is available
if (-not $Env:BUILD_BUILDNUMBER)
{
    Write-Error ("BUILD_BUILDNUMBER environment variable is missing.")
    exit 1
}
elseif (-not $Env:BUILD_BUILDNUMBER)
{
    Write-Error "BUILD_SOURCEVERSION does not exist: $Env:BUILD_BUILDNUMBER"
    exit 1
}
Write-Debug "BUILD_BUILDNUMBER: $Env:BUILD_BUILDNUMBER"

$buildNumber = $Env:BUILD_BUILDNUMBER -replace "[\D]", ""

# Apply the version to the assembly property files
$files = gci $Env:BUILD_SOURCESDIRECTORY -recurse | 
    foreach { gci -Path $_.FullName -Recurse -include CommonAssemblyInfo.* }

if($files)
{
    Write-Verbose "Will apply update to $($files.count) files."
    Write-Verbose "Build number is '$buildNumber'."

    foreach ($file in $files) {
        $filecontent = Get-Content($file)
        attrib $file -r
        $filecontent -ireplace $AssemblyFileVersionRegEx, "`${1}$buildNumber`${3}" | Out-File $file
        $filecontent = Get-Content($file)

        # Check for pre-release version
        if($filecontent -match $AssemblyInformationalVersionRegEx)
        {
            $filecontent -ireplace $AssemblyInformationalVersionRegEx, "`${1}`${2}-$buildNumber`${3}" | Out-File $file
        }

        Write-Verbose "$file - version applied"
    }
}
else
{
    Write-Warning "Found no files."
}