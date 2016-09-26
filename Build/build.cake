#tool "nuget:?package=gitlink"

// Arguments
var target = Argument("target", "Default");
var version = "1.1.0.0";
var infoVersion = "1.1.0";
var alphaVersion = "1.1.0-alpha001";

var configGitLink = new GitLinkSettings {
  RepositoryUrl = "https://github.com/punker76/gong-wpf-dragdrop",
  Branch        = "master",
  Configuration = "Release"
};

var newAssemblyInfoSettings = new AssemblyInfoSettings {
	Product = string.Format("GongSolutions.WPF.DragDrop {0}", version),
	Version = version,
	FileVersion = version,
	InformationalVersion = string.Format("GongSolutions.WPF.DragDrop {0}", infoVersion),
	Copyright = string.Format("Copyright © GongSolutions.WPF.DragDrop 2013 - {0}", DateTime.Now.Year)
};

var nuGetPackSettings   = new NuGetPackSettings {
  BasePath                = "../src/bin/GongSolutions.WPF.DragDrop/",
  Id                      = "gong-wpf-dragdrop",
  Title                   = "gong-wpf-dragdrop",
  Copyright               = string.Format("Copyright © GongSolutions.WPF.DragDrop 2013 - {0}", DateTime.Now.Year)
};

// Tasks
Task("GitLink")
  .Does(() =>
{
  configGitLink.Branch = "master";
  configGitLink.Configuration = "Release";
  GitLink("../", configGitLink);
  DeleteFiles("../src/bin/**/*.srcsrv");
});

Task("GitLink_Debug")
  .Does(() =>
{
  configGitLink.Branch = "dev";
  configGitLink.Configuration = "Debug";
  GitLink("../", configGitLink);
  DeleteFiles("../src/bin/**/*.srcsrv");
});

Task("UpdateAssemblyInfo")
  .Does(() =>
{
  CreateAssemblyInfo("../src/GlobalAssemblyInfo.cs", newAssemblyInfoSettings);
});

Task("UpdateAssemblyInfo_Debug")
  .Does(() =>
{
  newAssemblyInfoSettings.InformationalVersion = string.Format("GongSolutions.WPF.DragDrop {0}", alphaVersion);
  CreateAssemblyInfo("../src/GlobalAssemblyInfo.cs", newAssemblyInfoSettings);
});

Task("Build")
  .Does(() =>
{
  MSBuild("../src/GongSolutions.WPF.DragDrop.sln", settings => settings.SetConfiguration("Release").UseToolVersion(MSBuildToolVersion.VS2015));
});

Task("Build_Debug")
  .Does(() =>
{
  MSBuild("../src/GongSolutions.WPF.DragDrop.sln", settings => settings.SetConfiguration("Debug").UseToolVersion(MSBuildToolVersion.VS2015));
});

Task("NuGetPack")
  .Does(() =>
{
  nuGetPackSettings.Version = version;
  NuGetPack("./GongSolutions.Wpf.DragDrop.nuspec", nuGetPackSettings);
});

Task("NuGetPack_Debug")
  .Does(() =>
{
  nuGetPackSettings.Version = alphaVersion;
  NuGetPack("./GongSolutions.Wpf.DragDrop.ALPHA.nuspec", nuGetPackSettings);
});

Task("ZipShowcase")
  .Does(() =>
{
  Zip("../src/bin/Showcase.WPF.DragDrop/Release_NET45/", "Showcase.WPF.DragDrop.Release.zip");
});

Task("ZipShowcase_Debug")
  .Does(() =>
{
  Zip("../src/bin/Showcase.WPF.DragDrop/Debug_NET45/", "Showcase.WPF.DragDrop.Debug.zip");
});

Task("CleanOutput")
  .ContinueOnError()
  .Does(() =>
{
  CleanDirectories("../src/bin");
});

// Task Targets
Task("Default").IsDependentOn("CleanOutput").IsDependentOn("UpdateAssemblyInfo").IsDependentOn("Build").IsDependentOn("GitLink").IsDependentOn("ZipShowcase");
Task("Debug").IsDependentOn("CleanOutput").IsDependentOn("UpdateAssemblyInfo_Debug").IsDependentOn("Build_Debug").IsDependentOn("GitLink_Debug").IsDependentOn("ZipShowcase_Debug");

Task("Appveyor")
  .IsDependentOn("CleanOutput")
	.IsDependentOn("UpdateAssemblyInfo")
  .IsDependentOn("Build")
  .IsDependentOn("GitLink")
  .IsDependentOn("ZipShowcase")
	.IsDependentOn("NuGetPack");
Task("AppveyorDev")
  .IsDependentOn("CleanOutput")
  .IsDependentOn("UpdateAssemblyInfo_Debug")
  .IsDependentOn("Build_Debug")
  .IsDependentOn("GitLink_Debug")
  .IsDependentOn("ZipShowcase_Debug")
  .IsDependentOn("NuGetPack_Debug");

// Execution
RunTarget(target);