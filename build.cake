
//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////

#tool "nuget:?package=GitVersion.CommandLine&version=3.6.5"
#tool "nuget:?package=vswhere&version=2.2.7"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
if (string.IsNullOrWhiteSpace(target))
{
    target = "Default";
}

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Build configuration
var local = BuildSystem.IsLocalBuild;
var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var isDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("dev", AppVeyor.Environment.Repository.Branch);
var isReleaseBranch = StringComparer.OrdinalIgnoreCase.Equals("master", AppVeyor.Environment.Repository.Branch);
var isTagged = AppVeyor.Environment.Repository.Tag?.IsTag;

var githubOwner = "punker76";
var githubRepository = "gong-wpf-dragdrop";
var githubUrl = string.Format("https://github.com/{0}/{1}", githubOwner, githubRepository);

// Version
var gitVersion = GitVersion(new GitVersionSettings {
  UpdateAssemblyInfo = true,
  UpdateAssemblyInfoFilePath = "./src/GlobalAssemblyInfo.cs"
  });
var majorMinorPatch = gitVersion.MajorMinorPatch;
var informationalVersion = gitVersion.InformationalVersion;
var nugetVersion = gitVersion.NuGetVersion;
var buildVersion = gitVersion.FullBuildMetaData;

// Define global marcos.
Action Abort = () => { throw new Exception("a non-recoverable fatal error occurred."); };

var nuGetPackSettings = new NuGetPackSettings {
  Version         = nugetVersion,
  BasePath        = "./src/bin/GongSolutions.WPF.DragDrop/",
  OutputDirectory = "./Build",
  Id              = "gong-wpf-dragdrop",
  Title           = "gong-wpf-dragdrop",
  Copyright       = string.Format("Copyright © GongSolutions.WPF.DragDrop 2013 - {0}", DateTime.Now.Year)
};

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
    if (!IsRunningOnWindows())
    {
        throw new NotImplementedException("gong-wpf-dragdrop will only build on Windows because it's not possible to target WPF and Windows Forms from UNIX.");
    }

    Information("Building version {0} of gong-wpf-dragdrop. (isTagged: {1})", informationalVersion, isTagged);
});

Teardown(context =>
{
    // Executed AFTER the last task.
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Build")
  .Does(() =>
{
  MSBuild("./src/GongSolutions.WPF.DragDrop.sln", settings => settings.SetConfiguration("Release").UseToolVersion(MSBuildToolVersion.VS2015));
});

Task("Build_Debug")
  .Does(() =>
{
  MSBuild("./src/GongSolutions.WPF.DragDrop.sln", settings => settings.SetConfiguration("Debug").UseToolVersion(MSBuildToolVersion.VS2015));
});

Task("NuGetPack")
  .Does(() =>
{
  NuGetPack("./Build/GongSolutions.Wpf.DragDrop.nuspec", nuGetPackSettings);
});

Task("ZipShowcase")
  .Does(() =>
{
  Zip("./src/bin/Showcase.WPF.DragDrop/Release_NET45/", "./Build/Showcase.WPF.DragDrop." + nugetVersion + ".zip");
});

Task("ZipShowcase_Debug")
  .Does(() =>
{
  Zip("./src/bin/Showcase.WPF.DragDrop/Debug_NET45/", "./Build/Showcase.WPF.DragDrop.Debug." + nugetVersion + ".zip");
});

Task("CleanOutput")
  .ContinueOnError()
  .Does(() =>
{
  CleanDirectories("./src/bin");
});

// Task Targets
Task("Default")
  .IsDependentOn("CleanOutput")
  .IsDependentOn("Build_Debug").IsDependentOn("Build")
  .IsDependentOn("ZipShowcase_Debug").IsDependentOn("ZipShowcase");

Task("Release")
  .IsDependentOn("CleanOutput")
  .IsDependentOn("Build")
  .IsDependentOn("ZipShowcase");

Task("Appveyor")
  .IsDependentOn("Release")
  .IsDependentOn("NuGetPack");

// Execution
RunTarget(target);
