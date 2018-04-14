$baseDir = Resolve-Path ..
$scriptsDir = "$baseDir\scripts"
$toolsDir = "$baseDir\tools"
$resultsDir = "$baseDir\results"
$nugetCommand = "$toolsDir\NuGet.exe"

function Write-Title($title) {
	Write-Host "`n`n===== $title =====" -f Yellow
}

function Write-Summary($elapsed) {
	Write-Host "`n`n===== Done. This script took $elapsed to run. =====" -f Green
}

function Write-Error($project) {
	Write-Host "`n`n===== Failed to build $project. =====" -f Red
}

function GitVersion() {
	& cmd /c "$gitVersionCommand > $baseDir\version.json"
	$version = [IO.File]::ReadAllText("$baseDir\version.json") | ConvertFrom-Json
	
	return $version.NuGetVersion
}

function Stop-MsBuild() {
	Stop-Process -Name "MSBuild" -Force -ErrorAction SilentlyContinue
}