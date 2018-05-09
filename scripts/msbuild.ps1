. ".\environment.ps1"

function Invoke-NugetRestore ($solutionFile) {
    & $nugetCommand restore $solutionFile
    if (-not $?) { 
        Write-Error $solutionFile
        exit 1
    }
}

function Invoke-MsBuild ($solutionFile) {
    msbuild $solutionFile /v:minimal /m /target:Rebuild /p:Configuration=Release /p:RunOctoPack=true /p:OctoPackPublishPackageToFileShare=$resultsDir /p:OctoPackNuGetArguments="-Symbols"
    if (-not $?) { 
        Write-Error $solutionFile
        exit 1
    }
}

function Invoke-Tests ($testProject) {
    if (Test-Path $testProject) {
        msbuild $testProject /v:minimal

        if (-not $?) { 
            Write-Error $testProject
            exit 1
        }
    }
}