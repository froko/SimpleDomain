$baseDir = Resolve-Path ..
$scriptsDir = "$baseDir\scripts"
$sourceDir = "$baseDir\source"
$packagesDir = "$sourceDir\packages"
	
& $scriptsDir\clean.ps1	
	
Write-Host "deleting" $packagesDir	
Remove-Item $packagesDir -Force -Recurse -ErrorAction SilentlyContinue	

Write-Host "deleting suo files"	
Remove-Item "$sourceDir\*.suo" -Force -ErrorAction SilentlyContinue	
	
Write-Host "deleting StyleCop cache files"	
Remove-Item "$sourceDir\*\StyleCop.Cache" -Force -ErrorAction SilentlyContinue	
