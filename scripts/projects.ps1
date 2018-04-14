. ".\environment.ps1"

class Project {
    $name
    $path
    $solutionFile
    $testProjectFile

    Project($name, $path, $solutionFile) {
        $this.name = $name
        $this.path = $path
        $this.solutionFile = "$path\$solutionFile"
        $this.testProjectFile = "$path\TestProject.csproj"
    }
}

$simpledomain = [Project]::new("SimpleDomain", "$baseDir\source", "SimpleDomain.sln")

$projects = $simpledomain