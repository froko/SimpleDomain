#tool "nuget:?package=GitVersion.CommandLine"

var target = Argument("target", "Default");
var outputDir = "./artifacts/";

Task("Clean")
    .Does(() => {
        if (DirectoryExists(outputDir))
        {
            DeleteDirectory(outputDir, recursive:true);
        }
    });

Task("Reset")
    .IsDependentOn("Clean")
    .Does(() => {
        CleanDirectories("./src/**/obj");
        CleanDirectories("./src/**/bin");
    });

GitVersion versionInfo = null;
Task("Version")
    .Does(() => {
        GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = true,
            OutputType = GitVersionOutput.BuildServer
        });
        versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Version")
    .Does(() => {
        var settings = new DotNetCoreBuildSettings
        {
            Configuration = "Release"
        };

        DotNetCoreBuild("./src/SimpleDomain.sln", settings);
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        var settings = new DotNetCoreTestSettings
        {
            Configuration = "Release"
        };


        var projectFiles = GetFiles("./src/*.Facts/*.csproj");
        foreach(var file in projectFiles)
        {
            DotNetCoreTest(file.FullPath, settings);
        }
    });

Task("Package")
    .IsDependentOn("Test")
    .Does(() => {
        var settings = new DotNetCorePackSettings
        {
            ArgumentCustomization = args=> args.Append(" --include-symbols /p:PackageVersion=" + versionInfo.NuGetVersion),
            OutputDirectory = outputDir,
            Configuration = "Release",
            NoBuild = true
        };

        DotNetCorePack("./src/SimpleDomain/SimpleDomain.csproj", settings);
        DotNetCorePack("./src/SimpleDomain.Ninject/SimpleDomain.Ninject.csproj", settings);
        DotNetCorePack("./src/SimpleDomain.MSMQ/SimpleDomain.MSMQ.csproj", settings);
        DotNetCorePack("./src/SimpleDomain.RabbitMq/SimpleDomain.RabbitMq.csproj", settings);
        DotNetCorePack("./src/SimpleDomain.GetEventStore/SimpleDomain.GetEventStore.csproj", settings);

        System.IO.File.WriteAllLines(outputDir + "artifacts", new[]{
            "nuget:SimpleDomain." + versionInfo.NuGetVersion + ".nupkg",
            "nugetSymbols:SimpleDomain." + versionInfo.NuGetVersion + ".symbols.nupkg",
            "nuget:SimpleDomain.Ninject." + versionInfo.NuGetVersion + ".nupkg",
            "nugetSymbols:SimpleDomain.Ninject." + versionInfo.NuGetVersion + ".symbols.nupkg",
            "nuget:SimpleDomain.MSMQ." + versionInfo.NuGetVersion + ".nupkg",
            "nugetSymbols:SimpleDomain.MSMQ." + versionInfo.NuGetVersion + ".symbols.nupkg",
            "nuget:SimpleDomain.RabbitMq." + versionInfo.NuGetVersion + ".nupkg",
            "nugetSymbols:SimpleDomain.RabbitMq." + versionInfo.NuGetVersion + ".symbols.nupkg",
            "nuget:SimpleDomain.GetEventStore." + versionInfo.NuGetVersion + ".nupkg",
            "nugetSymbols:SimpleDomain.GetEventStore." + versionInfo.NuGetVersion + ".symbols.nupkg"
        });

        if (AppVeyor.IsRunningOnAppVeyor)
        {
            foreach (var file in GetFiles(outputDir + "**/*"))
                AppVeyor.UploadArtifact(file.FullPath);
        }
    });

Task("Default")
    .IsDependentOn("Package");

RunTarget(target);