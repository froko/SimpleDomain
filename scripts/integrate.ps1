$baseDir = Resolve-Path ..
$scriptsDir = "$baseDir\scripts"
$sourceDir = "$baseDir\source"
$nugetDir = "$baseDir\nuget"
$nugetCommand = "$sourceDir\.nuget\NuGet.exe"
$solutionFile = "$sourceDir\SimpleDomain.Core.sln"

& $scriptsDir\clean.ps1	
& $nugetCommand restore $solutionFile

msbuild $solutionFile /target:Rebuild /p:Configuration=Release /p:RunOctoPack=true /p:OctoPackPublishPackageToFileShare=$nugetDir