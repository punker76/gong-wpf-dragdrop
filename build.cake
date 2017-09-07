// Arguments
var target = Argument("target", "Default");

var version = "1.2.0.0";
var infoVersion = "1.2.0";
var alphaVersion = "1.2.0-dev001";

var newAssemblyInfoSettings = new AssemblyInfoSettings {
  Product = string.Format("GongSolutions.WPF.DragDrop {0}", version),
  Version = version,
  FileVersion = version,
  InformationalVersion = string.Format("GongSolutions.WPF.DragDrop {0}", infoVersion),
  Copyright = string.Format("Copyright © GongSolutions.WPF.DragDrop 2013 - {0}", DateTime.Now.Year)
};

var nuGetPackSettings = new NuGetPackSettings {
  BasePath        = "./src/bin/GongSolutions.WPF.DragDrop/",
  OutputDirectory = "./Build",
  Id              = "gong-wpf-dragdrop",
  Title           = "gong-wpf-dragdrop",
  Copyright       = string.Format("Copyright © GongSolutions.WPF.DragDrop 2013 - {0}", DateTime.Now.Year)
};

// Tasks

Task("UpdateAssemblyInfo")
  .Does(() =>
{
  CreateAssemblyInfo("./src/GongSolutions.WPF.DragDrop.Shared/GlobalAssemblyInfo.cs", newAssemblyInfoSettings);
});

Task("UpdateAssemblyInfo_Debug")
  .Does(() =>
{
  newAssemblyInfoSettings.InformationalVersion = string.Format("GongSolutions.WPF.DragDrop {0}", alphaVersion);
  CreateAssemblyInfo("./src/GongSolutions.WPF.DragDrop.Shared/GlobalAssemblyInfo.cs", newAssemblyInfoSettings);
});

Task("Build")
  .Does(() =>
{
  MSBuild("./src/GongSolutions.WPF.DragDrop.sln", settings => settings.SetConfiguration("Release").UseToolVersion(MSBuildToolVersion.VS2017));
});

Task("Build_Debug")
  .Does(() =>
{
  MSBuild("./src/GongSolutions.WPF.DragDrop.sln", settings => settings.SetConfiguration("Debug").UseToolVersion(MSBuildToolVersion.VS2017));
});

Task("NuGetPack")
  .Does(() =>
{
  nuGetPackSettings.Version = version;
  NuGetPack("./Build/GongSolutions.Wpf.DragDrop.nuspec", nuGetPackSettings);
});

Task("NuGetPack_Debug")
  .Does(() =>
{
  nuGetPackSettings.Version = alphaVersion;
  NuGetPack("./Build/GongSolutions.Wpf.DragDrop.ALPHA.nuspec", nuGetPackSettings);
});

Task("ZipShowcase")
  .Does(() =>
{
  Zip("./src/bin/Showcase.WPF.DragDrop/Release_NET45/", "./Build/Showcase.WPF.DragDrop.Release.zip");
});

Task("ZipShowcase_Debug")
  .Does(() =>
{
  Zip("./src/bin/Showcase.WPF.DragDrop/Debug_NET45/", "./Build/Showcase.WPF.DragDrop.Debug.zip");
});

Task("CleanOutput")
  .ContinueOnError()
  .Does(() =>
{
  CleanDirectories("./src/bin");
});

// Task Targets
Task("Release").IsDependentOn("CleanOutput").IsDependentOn("UpdateAssemblyInfo").IsDependentOn("Build").IsDependentOn("ZipShowcase");
Task("Default").IsDependentOn("CleanOutput").IsDependentOn("UpdateAssemblyInfo_Debug").IsDependentOn("Build_Debug").IsDependentOn("ZipShowcase_Debug");

Task("Appveyor")
  .IsDependentOn("CleanOutput")
  .IsDependentOn("UpdateAssemblyInfo")
  .IsDependentOn("Build")
  .IsDependentOn("ZipShowcase")
  .IsDependentOn("NuGetPack");

Task("AppveyorDev")
  .IsDependentOn("CleanOutput")
  .IsDependentOn("UpdateAssemblyInfo_Debug")
  .IsDependentOn("Build_Debug")
  .IsDependentOn("ZipShowcase_Debug")
  .IsDependentOn("NuGetPack_Debug");

// Execution
RunTarget(target);
