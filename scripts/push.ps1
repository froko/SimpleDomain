$baseDir = Resolve-Path ..
$sourceDir = "$baseDir\source"
$nugetDir = "$baseDir\nuget"
$nugetCommand = "$sourceDir\.nuget\NuGet.exe"

Function NugetPackages {
	return Get-ChildItem $nugetDir\*.nupkg -Exclude *.symbols.nupkg
}

NugetPackages |
Foreach-Object {
	$package = $_.fullname
	
	Write-Host "pushing nuget package" $package
	& cmd /c "$nugetCommand push $package -Source https://www.myget.org/F/simpledomain/api/v2/package"
}