. ".\environment.ps1"

function NugetPackages {
	return Get-ChildItem $resultsDir\*.nupkg
}

NugetPackages | Foreach-Object {
	$package = $_.fullname
	
	Write-Host "pushing nuget package" $package
	& cmd /c "$nugetCommand push $package -Source https://www.myget.org/F/simpledomain/api/v2/package"
}