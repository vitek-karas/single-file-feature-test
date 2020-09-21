[CmdletBinding()]
param (
    [switch] $scd,
    [switch] $sf,
    [switch] $includeAllContent,
    [switch] $includeNativeLibs,
    [switch] $includePdbs,
    [string] $RuntimeRepoLocation
)

if ([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows)) {
    $rid = "win-x64"
    $coreClrConfig = "Windows_NT.x64.Debug"
    $coreHostConfig = "win-x64.Debug"
} elseif ([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Linux)) {
    $rid = "linux-x64"
    $coreClrConfig = "Linux.x64.Debug"
    $coreHostConfig = "linux-x64.Debug"
} else {
    $rid = "osx"
}

$args = @()
$outPath = "$PSScriptRoot/FeatureTest/bin/"
$args += "publish"
$args += "$PSScriptRoot/FeatureTest/FeatureTest.csproj"

$args += "-r:$rid"

if ($sf) {
    $args += "/p:PublishSingleFile=true"
    $outPath += "sf-"
}
else {
    $outPath += "mf-"
}

if ($scd) {
    $args += "--self-contained:true"
    $outPath += "scd"
}
else {
    $args += "--self-contained:false"
    $outPath += "fdd"
}

if ($includeAllContent) {
    $args += "/p:IncludeAllContentForSelfExtract=true"
    $outPath += "-extall"
}

if ($includeNativeLibs) {
    $args += "/p:IncludeNativeLibrariesForSelfExtract=true"
    $outPath += "-extlib"
}

if ($includePdbs) {
    $args += "/p:IncludeSymbolsInSingleFile=true"
    $outPath += "-extpdb"
}

$args += "-o"
$args += $outPath

if (-Not [string]::IsNullOrEmpty($RuntimeRepoLocation))
{
    $args += "/p:CustomizePublishSingleFile_RuntimeRepoRoot=$RuntimeRepoLocation"
    $args += "/p:CustomizePublishSingleFile_CoreClrConfig=$coreClrConfig"
    $args += "/p:CustomizePublishSingleFile_CoreHostConfig=$coreHostConfig"
}

Write-Host "dotnet" $args
& "dotnet" $args

Write-Host " "
Write-Host $outPath/FeatureTest
& $outPath/FeatureTest
