
//////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
//////////////////////////////////////////////////////////////////////

#tool paket:?package=GitVersion.CommandLine
#tool paket:?package=gitreleasemanager
#tool paket:?package=vswhere
#addin paket:?package=Cake.Figlet
#addin paket:?package=Cake.Paket

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
if (string.IsNullOrWhiteSpace(target))
{
    target = "Default";
}

var configuration = Argument("configuration", "Release");
if (string.IsNullOrWhiteSpace(configuration))
{
    configuration = "Release";
}

var verbosity = Argument("verbosity", Verbosity.Normal);
if (string.IsNullOrWhiteSpace(configuration))
{
    verbosity = Verbosity.Normal;
}

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var repoName = "gong-wpf-dragdrop";
var local = BuildSystem.IsLocalBuild;

// Set build version
if (local == false
    || verbosity == Verbosity.Verbose)
{
    GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.BuildServer });
}
GitVersion gitVersion = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });

var latestInstallationPath = VSWhereProducts("*", new VSWhereProductSettings { Version = "[\"15.0\",\"16.0\"]" }).FirstOrDefault();
var msBuildPath = latestInstallationPath.CombineWithFilePath("./MSBuild/15.0/Bin/MSBuild.exe");

// Should MSBuild treat any errors as warnings?
var treatWarningsAsErrors = false;

var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var branchName = gitVersion.BranchName;
var isDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("dev", branchName);
var isReleaseBranch = StringComparer.OrdinalIgnoreCase.Equals("master", branchName);
var isTagged = AppVeyor.Environment.Repository.Tag.IsTag;

// Version
var nugetVersion = isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion;

// Directories and Paths
var iconPacksSolution = "./src/GongSolutions.WPF.DragDrop.sln";
var publishDir = "./Publish";

// Define global marcos.
Action Abort = () => { throw new Exception("a non-recoverable fatal error occurred."); };

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
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
    Information("IsLocalBuild           : {0}", local);
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

Task("CleanOutput")
  .ContinueOnError()
  .Does(() =>
{
    var directoriesToDelete = GetDirectories("./**/obj").Concat(GetDirectories("./**/bin")).Concat(GetDirectories("./**/Publish"));
    DeleteDirectories(directoriesToDelete, new DeleteDirectorySettings { Recursive = true, Force = true });
});

Task("Restore")
    .Does(() =>
{
    PaketRestore();

    var msBuildSettings = new MSBuildSettings { ToolPath = msBuildPath, ArgumentCustomization = args => args.Append("/m") };
    MSBuild(iconPacksSolution, msBuildSettings
            //.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .WithTarget("restore")
            );
});

Task("Build")
  .Does(() =>
{
  var msBuildSettings = new MSBuildSettings { ToolPath = msBuildPath, ArgumentCustomization = args => args.Append("/m") };

  Information("Build: Release");
  MSBuild(iconPacksSolution, msBuildSettings
            .SetMaxCpuCount(0)
            .SetConfiguration("Release") //.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Normal)
            //.WithRestore() only with cake 0.28.x            
            .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
            .WithProperty("FileVersion", gitVersion.AssemblySemFileVer)
            .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
            );
});

Task("Pack")
  .WithCriteria(() => !isPullRequest)
  .Does(() =>
{
  EnsureDirectoryExists(Directory(publishDir));

  // var msBuildSettings = new MSBuildSettings { ToolPath = msBuildPath };
 
  // var projects = GetFiles("./MahApps.Metro.IconPacks/*.csproj");

  // foreach(var project in projects)
  // {
  //   Information("Packing {0}", project);

  //   DeleteFiles(GetFiles("./MahApps.Metro.IconPacks/obj/**/*.nuspec"));

  //   MSBuild(project, msBuildSettings
  //     .SetConfiguration(configuration)
  //     .SetVerbosity(Verbosity.Normal)
  //     .WithTarget("pack")
  //     .WithProperty("PackageOutputPath", "../" + publishDir)
  //     .WithProperty("Version", isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion)
  //     .WithProperty("RepositoryBranch", branchName)
  //     .WithProperty("RepositoryCommit", gitVersion.Sha)
  //     .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
  //     .WithProperty("FileVersion", gitVersion.AssemblySemFileVer)
  //     .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
  //   );
  // }

});

Task("Zip")
  .Does(() =>
{
  EnsureDirectoryExists(Directory(publishDir));
  Zip($"./src/Showcase.WPF.DragDrop.NET45/bin/{configuration}/", $"{publishDir}/Showcase.DragDrop.{configuration}-v" + gitVersion.NuGetVersion + ".zip");
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

    GitReleaseManagerCreate(username, token, "MahApps", repoName, new GitReleaseManagerCreateSettings {
        Milestone         = gitVersion.MajorMinorPatch,
        Name              = gitVersion.AssemblySemFileVer,
        Prerelease        = isDevelopBranch,
        TargetCommitish   = branchName,
        WorkingDirectory  = "."
    });
});

// Task Targets
Task("Default")
    .IsDependentOn("CleanOutput")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    // .IsDependentOn("Zip")
    ;

Task("appveyor")
    .IsDependentOn("Default")
    .IsDependentOn("Pack");

// Execution
RunTarget(target);