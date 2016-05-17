# Enable -Verbose option
[CmdletBinding()]

[Regex] $regex = '(AssemblyFileVersion\(\".\..\..\.)(.)(\"\))'

# If this script is not running on a build server, remind user to 
# set environment variables so that this script can be debugged
if(-not ($Env:BUILD_SOURCESDIRECTORY -and $ENV:BUILD_SOURCEVERSION ))
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

# Apply the version to the assembly property files
#$files = gci $Env:BUILD_SOURCESDIRECTORY -recurse -include "*Properties*","My Project" | 
$files = gci $Env:BUILD_SOURCESDIRECTORY -recurse | 
    ?{ $_.PSIsContainer } | 
    foreach { gci -Path $_.FullName -Recurse -include CommonAssemblyInfo.* }

if($files)
{
    Write-Verbose "Will apply $NewAssemblyVersion and $NewAssemblyFileVersion to $($files.count) files."

    foreach ($file in $files) {
        $filecontent = Get-Content($file)
        attrib $file -r
        $filecontent -ireplace $regex, "`${1}$sourceVersion[0]`${3}" | Out-File $file
        Write-Verbose "$file.FullName - version applied"
    }
}
else
{
    Write-Warning "Found no files."
}