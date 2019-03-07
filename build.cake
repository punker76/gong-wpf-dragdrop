
//////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
//////////////////////////////////////////////////////////////////////

#tool GitVersion.CommandLine
#tool gitreleasemanager
#tool vswhere
#addin Cake.Figlet

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var verbosity = Argument("verbosity", Verbosity.Minimal);
var dotnetcoreverbosity = Argument("dotnetcoreverbosity", DotNetCoreVerbosity.Minimal);

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var repoName = "gong-wpf-dragdrop";
var isLocal = BuildSystem.IsLocalBuild;

// Set build version
if (isLocal == false || verbosity == Verbosity.Verbose)
{
    GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.BuildServer });
}
GitVersion gitVersion = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });

var latestInstallationPath = VSWhereLatest(new VSWhereLatestSettings { IncludePrerelease = true });
var msBuildPath = latestInstallationPath.Combine("./MSBuild/Current/Bin");
var msBuildPathExe = msBuildPath.CombineWithFilePath("./MSBuild.exe");

if (FileExists(msBuildPathExe) == false)
{
    throw new NotImplementedException("You need at least Visual Studio 2019 to build this project.");
}

var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var branchName = gitVersion.BranchName;
var isDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("dev", branchName);
var isReleaseBranch = StringComparer.OrdinalIgnoreCase.Equals("master", branchName);
var isTagged = AppVeyor.Environment.Repository.Tag.IsTag;

// Directories and Paths
var solution = "./src/GongSolutions.WPF.DragDrop.sln";
var publishDir = "./Publish";

// Define global marcos.
Action Abort = () => { throw new Exception("a non-recoverable fatal error occurred."); };

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    // Executed BEFORE the first task.

    if (!IsRunningOnWindows())
    {
        throw new NotImplementedException($"{repoName} will only build on Windows because it's not possible to target WPF and Windows Forms from UNIX.");
    }

    Information(Figlet(repoName));

    Information("Informational Version  : {0}", gitVersion.InformationalVersion);
    Information("SemVer Version         : {0}", gitVersion.SemVer);
    Information("AssemblySemVer Version : {0}", gitVersion.AssemblySemVer);
    Information("MajorMinorPatch Version: {0}", gitVersion.MajorMinorPatch);
    Information("NuGet Version          : {0}", gitVersion.NuGetVersion);
    Information("IsLocalBuild           : {0}", isLocal);
    Information("Branch                 : {0}", branchName);
    Information("Configuration          : {0}", configuration);
    Information("MSBuildPath            : {0}", msBuildPath);
});

Teardown(context =>
{
    // Executed AFTER the last task.
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
  .ContinueOnError()
  .Does(() =>
{
    var directoriesToDelete = GetDirectories("./**/obj")
        .Concat(GetDirectories("./**/bin"))
        .Concat(GetDirectories("./**/Publish"));
    DeleteDirectories(directoriesToDelete, new DeleteDirectorySettings { Recursive = true, Force = true });
});

Task("Restore")
    .Does(() =>
{
    var msBuildSettings = new MSBuildSettings {
        Verbosity = verbosity
        , ToolPath = msBuildPathExe
        , Configuration = configuration
        , ArgumentCustomization = args => args.Append("/m")
    };
    MSBuild(solution, msBuildSettings.WithTarget("restore"));
});

Task("Build")
  .Does(() =>
{
    var msBuildSettings = new MSBuildSettings {
        Verbosity = verbosity
        , ToolPath = msBuildPathExe
        , Configuration = configuration
        , ArgumentCustomization = args => args.Append("/m")
        , BinaryLogger = new MSBuildBinaryLogSettings() { Enabled = isLocal }
    };
    MSBuild(solution, msBuildSettings
            .SetMaxCpuCount(0)
            .WithProperty("Version", isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion)
            .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
            .WithProperty("FileVersion", gitVersion.AssemblySemFileVer)
            .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
            );
});

Task("dotnetBuild")
  .Does(() =>
{
    var buildSettings = new DotNetCoreBuildSettings {
        Verbosity = dotnetcoreverbosity,
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("/m"),
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .SetMaxCpuCount(0)
            .SetConfiguration(configuration)
            .WithProperty("Version", isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion)
            .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
            .WithProperty("FileVersion", gitVersion.AssemblySemFileVer)
            .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
    };

    DotNetCoreBuild(solution, buildSettings);
});

Task("Pack")
  .WithCriteria(() => !isPullRequest)
  .Does(() =>
{
    EnsureDirectoryExists(Directory(publishDir));

    var msBuildSettings = new MSBuildSettings {
        Verbosity = verbosity
        , ToolPath = msBuildPathExe
        , Configuration = configuration
    };
    var projects = GetFiles("./src/GongSolutions.WPF.DragDrop/*.csproj");

    foreach(var project in projects)
    {
        Information("Packing {0}", project);

        DeleteFiles(GetFiles("./src/**/*.nuspec"));

        MSBuild(project, msBuildSettings
            .WithTarget("pack")
            .WithProperty("PackageOutputPath", MakeAbsolute(Directory(publishDir)).FullPath)
            .WithProperty("RepositoryBranch", branchName)
            .WithProperty("RepositoryCommit", gitVersion.Sha)
            .WithProperty("Version", isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion)
            .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
            .WithProperty("FileVersion", gitVersion.AssemblySemFileVer)
            .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
        );
    }

});

Task("dotnetPack")
  .WithCriteria(() => !isPullRequest)
  .Does(() =>
{
    EnsureDirectoryExists(Directory(publishDir));

    var buildSettings = new DotNetCorePackSettings {
        Verbosity = dotnetcoreverbosity,
        Configuration = configuration,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .SetMaxCpuCount(0)
            .SetConfiguration(configuration)
            .WithProperty("PackageOutputPath", MakeAbsolute(Directory(publishDir)).FullPath)
            .WithProperty("RepositoryBranch", branchName)
            .WithProperty("RepositoryCommit", gitVersion.Sha)
            .WithProperty("Version", isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion)
            .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
            .WithProperty("FileVersion", gitVersion.AssemblySemFileVer)
            .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
    };

    var projects = GetFiles("./src/GongSolutions.WPF.DragDrop/*.csproj");

    foreach(var project in projects)
    {
        Information("Packing {0}", project);

        DeleteFiles(GetFiles("./src/**/*.nuspec"));

        DotNetCorePack(project.ToString(), buildSettings);
    }
});

Task("Zip")
  .Does(() =>
{
  EnsureDirectoryExists(Directory(publishDir));
  Zip($"./src/Showcase/bin/{configuration}", $"{publishDir}/Showcase.DragDrop.{configuration}-v" + gitVersion.NuGetVersion + ".zip");
});

Task("CreateRelease")
    .WithCriteria(() => !isTagged)
    .Does(() =>
{
    var username = EnvironmentVariable("GITHUB_USERNAME");
    if (string.IsNullOrEmpty(username))
    {
        throw new Exception("The GITHUB_USERNAME environment variable is not defined.");
    }

    var token = EnvironmentVariable("GITHUB_TOKEN");
    if (string.IsNullOrEmpty(token))
    {
        throw new Exception("The GITHUB_TOKEN environment variable is not defined.");
    }

    GitReleaseManagerCreate(username, token, "punker76", repoName, new GitReleaseManagerCreateSettings {
        Milestone         = gitVersion.MajorMinorPatch,
        Name              = gitVersion.AssemblySemFileVer,
        Prerelease        = isDevelopBranch,
        TargetCommitish   = branchName,
        WorkingDirectory  = "."
    });
});

// Task Targets
Task("Default")
    .IsDependentOn("Clean")
    // .IsDependentOn("Restore")
    // .IsDependentOn("Build")
    .IsDependentOn("dotnetBuild")
    .IsDependentOn("Zip")
    ;

Task("appveyor")
    .IsDependentOn("Default")
    // .IsDependentOn("Pack");
    .IsDependentOn("dotnetPack");

// Execution
RunTarget(target);