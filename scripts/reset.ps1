. ".\environment.ps1"
. ".\projects.ps1"

function Clean($projectDirectory) {
    Get-Childitem $projectDirectory -Include bin, obj, packages -Recurse | 
        Where {$_.psIsContainer -eq $true} | 
        Foreach-Object { 
        Write-Host "deleting" $_.fullname
        Remove-Item $_.fullname -Force -Recurse -ErrorAction SilentlyContinue
    }
}

$projects | ForEach-Object {
	Clean $_.path
}

Remove-Item $resultsDir -Force -Recurse -ErrorAction SilentlyContinue