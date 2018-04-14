. ".\environment.ps1"
. ".\projects.ps1"
. ".\msbuild.ps1"

Invoke-Command {
    $stopWatch = [System.Diagnostics.Stopwatch]::StartNew()

    Stop-MsBuild

    Write-Title "Clean up workspace"
    & ".\clean.ps1"

    Write-Title "Create new test results directory"
    New-Item -ItemType Directory -Force $resultsDir\TestResults
    
    $projects | ForEach-Object {
        $projectName = $_.name
        Write-Title "Build $projectName"

        Invoke-NugetRestore $_.solutionFile
        Invoke-MsBuild $_.solutionFile
        Invoke-Tests $_.testProjectFile
    }

    Stop-MsBuild
    
    Write-Summary $stopWatch.Elapsed
} 2>&1 | Tee-Object $resultsDir\integrate.log
