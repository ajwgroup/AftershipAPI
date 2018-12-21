// #tool "nuget:?package=coveralls.io&version=1.4.2"
#addin Cake.Git
// #addin "nuget:?package=Cake.Coveralls&version=0.9.0"
#addin "Cake.MiniCover"

SetMiniCoverToolsProject("./minicover/minicover.csproj");

//////////////////////////////////////////////////////
//      CONSTANTS AND ENVIRONMENT VARIABLES         //
//////////////////////////////////////////////////////

var target = Argument("target", "Default");
var artifactsDir = "./artifacts/";
var projectName = Argument<string>("projectName", null);
var solutionPath = projectName + ".sln";
var project = "./src/" + projectName + "/" + projectName + ".csproj";
var testFolder = "./tests/" + projectName + "Tests/";
var testProject = testFolder + projectName + "Tests.csproj";
var coverageResultsFileName = "coverage.xml";
var currentBranch = Argument<string>("currentBranch", GitBranchCurrent("./").FriendlyName);
var isReleaseBuild = string.Equals(currentBranch, "master", StringComparison.OrdinalIgnoreCase);
var configuration = "Release";
var nugetApiKey = Argument<string>("nugetApiKey", null);
var coverallsToken = Argument<string>("coverallsToken", null);
var nugetSource = "https://api.nuget.org/v3/index.json";

//////////////////////////////////////////////////////
//                     TASKS                        //
//////////////////////////////////////////////////////

Task("Clean")
    .Does(() => {
        if (DirectoryExists(artifactsDir))
        {
            DeleteDirectory(
                artifactsDir,
                new DeleteDirectorySettings {
                    Recursive = true,
                    Force = true
                }
            );
        }
        CreateDirectory(artifactsDir);
        DotNetCoreClean(solutionPath);
    });

Task("Restore")
    .Does(() => {
        DotNetCoreRestore(solutionPath);
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild(
            solutionPath,
            new DotNetCoreBuildSettings
            {
                Configuration = configuration
            }
        );
    });

Task("Test")
    .Does(() => {
        var settings = new DotNetCoreTestSettings
        {
            ArgumentCustomization = args => args.Append("/p:CollectCoverage=true")
                                                .Append("/p:CoverletOutputFormat=opencover")
                                                .Append("/p:CoverletOutput=./" + coverageResultsFileName)
        };
        DotNetCoreTest(testProject, settings);
        MoveFile(testFolder + coverageResultsFileName, artifactsDir + coverageResultsFileName);
    });

Task("UploadCoverage")
  .IsDependentOn("build")
    .Does(() =>
{
    MiniCover(tool =>
        {
            foreach(var project in GetFiles("./tests/**/*.csproj"))
            {
                tool.DotNetCoreTest(project.FullPath, new DotNetCoreTestSettings()
                {
                    // Required to keep instrumentation added by MiniCover
                    NoBuild = true,
                    Configuration = configuration
                });
            }
        },
        new MiniCoverSettings()
            .WithAssembliesMatching("./test/**/*.dll")
            .WithSourcesMatching("./src/**/*.cs")
            .GenerateReport(ReportType.CONSOLE | ReportType.XML)
            .Coveralls(new CoverallsSettings(){
                RepoToken = coverallsToken
            })

    );
});

Task("Package")
    .Does(() => {
        var settings = new DotNetCorePackSettings
        {
            OutputDirectory = artifactsDir,
            NoBuild = true
        };
        DotNetCorePack(project, settings);
    });

Task("Publish")
    .IsDependentOn("Package")
    .Does(() => {
        var pushSettings = new DotNetCoreNuGetPushSettings
        {
            Source = nugetSource,
            ApiKey = nugetApiKey
        };

        var pkgs = GetFiles(artifactsDir + "*.nupkg");
        foreach(var pkg in pkgs)
        {
            Information($"Publishing \"{pkg}\".");
            DotNetCoreNuGetPush(pkg.FullPath, pushSettings);
        }
    });

//////////////////////////////////////////////////////
//                     TARGETS                      //
//////////////////////////////////////////////////////

Task("BuildAndTest")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

Task("CompleteWithoutPublish")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("UploadCoverage");

if(isReleaseBuild)
{
    Information("Release build");
    Task("Complete")
        .IsDependentOn("Build")
        .IsDependentOn("Test")
        .IsDependentOn("UploadCoverage")
        .IsDependentOn("Publish");
}
else
{
    Information("Development build");
    Task("Complete")
        .IsDependentOn("Build")
        .IsDependentOn("Test")
        .IsDependentOn("UploadCoverage");
}

Task("Default")
    .IsDependentOn("Complete");


RunTarget(target);