///////////////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
///////////////////////////////////////////////////////////////////////////////

#tool dotnet:?package=NuGetKeyVaultSignTool&version=3.2.3
#tool dotnet:?package=AzureSignTool&version=6.0.0
#tool dotnet:?package=GitReleaseManager.Tool&version=0.17.0
#tool dotnet:?package=XamlStyler.Console&version=3.2206.4

#tool nuget:?package=GitVersion.CommandLine&version=5.12.0

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");

///////////////////////////////////////////////////////////////////////////////
// PREPARATION
///////////////////////////////////////////////////////////////////////////////

var repoName = "gong-wpf-dragdrop";
var baseDir = MakeAbsolute(Directory(".")).ToString();
var srcDir = baseDir + "/src";
var solution = srcDir + "/GongSolutions.WPF.DragDrop.sln";
var publishDir = baseDir + "/Publish";

var styler = Context.Tools.Resolve("xstyler.exe");
var stylerFile = baseDir + "/Settings.XAMLStyler";

public class BuildData
{
    public string Configuration { get; }
    public Verbosity Verbosity { get; }
    public DotNetVerbosity DotNetVerbosity { get; }
    public bool IsLocalBuild { get; set; }
    public bool IsPullRequest { get; set; }
    public bool IsPrerelease { get; set; }
    public bool IsRunningOnCI { get; set; }
    public GitVersion GitVersion { get; set; }

    public BuildData(
        string configuration,
        Verbosity verbosity,
        DotNetVerbosity dotNetVerbosity
    )
    {
        Configuration = configuration;
        Verbosity = verbosity;
        DotNetVerbosity = dotNetVerbosity;
    }

    public void SetGitVersion(GitVersion gitVersion)
    {
        GitVersion = gitVersion;
        IsPrerelease = GitVersion.NuGetVersion.Contains("-");
    }
}

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup<BuildData>(ctx =>
{
    if (!IsRunningOnWindows())
    {
        throw new NotImplementedException($"{repoName} will only build on Windows because it's not possible to target WPF and Windows Forms from UNIX.");
    }

    Spectre.Console.AnsiConsole.Write(new Spectre.Console.FigletText(repoName));

    var gitVersionPath = Context.Tools.Resolve("gitversion.exe");
    Information("GitVersion             : {0}", gitVersionPath);

    var buildData = new BuildData(
        configuration: Argument("configuration", "Release"),
        verbosity: Argument("verbosity", Verbosity.Minimal),
        dotNetVerbosity: Argument("dotNetVerbosity", DotNetVerbosity.Minimal)
    )
    {
        IsLocalBuild = BuildSystem.IsLocalBuild,
        IsRunningOnCI = BuildSystem.GitHubActions.IsRunningOnGitHubActions || BuildSystem.AppVeyor.IsRunningOnAppVeyor,
        IsPullRequest =
            (BuildSystem.GitHubActions.IsRunningOnGitHubActions && BuildSystem.GitHubActions.Environment.PullRequest.IsPullRequest)
            || (BuildSystem.AppVeyor.IsRunningOnAppVeyor && BuildSystem.AppVeyor.Environment.PullRequest.IsPullRequest)
    };

    // Set build version for CI
    if (buildData.IsLocalBuild == false || buildData.Verbosity == Verbosity.Verbose)
    {
        GitVersion(new GitVersionSettings { ToolPath = gitVersionPath, OutputType = GitVersionOutput.BuildServer });
    }
    buildData.SetGitVersion(GitVersion(new GitVersionSettings { ToolPath = gitVersionPath, OutputType = GitVersionOutput.Json }));

    Information("GitVersion             : {0}", gitVersionPath);
    Information("Branch                 : {0}", buildData.GitVersion.BranchName);
    Information("Configuration          : {0}", buildData.Configuration);
    Information("IsRunningOnCI          : {0}", buildData.IsRunningOnCI);
    Information("IsPrerelease           : {0}", buildData.IsPrerelease);
    Information("IsPrerelease           : {0}", buildData.IsPrerelease);
    Information("Informational   Version: {0}", buildData.GitVersion.InformationalVersion);
    Information("SemVer          Version: {0}", buildData.GitVersion.SemVer);
    Information("AssemblySemVer  Version: {0}", buildData.GitVersion.AssemblySemVer);
    Information("MajorMinorPatch Version: {0}", buildData.GitVersion.MajorMinorPatch);
    Information("NuGet           Version: {0}", buildData.GitVersion.NuGetVersion);
    Information("Verbosity              : {0}", buildData.Verbosity);
    Information("Publish folder         : {0}", publishDir);

    return buildData;
});

Teardown(ctx =>
{
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .ContinueOnError()
    .Does(() =>
{
    var directoriesToDelete = GetDirectories("./**/obj")
        .Concat(GetDirectories("./**/bin"))
        .Concat(GetDirectories("./**/Publish"))
        ;
    DeleteDirectories(directoriesToDelete, new DeleteDirectorySettings { Recursive = true, Force = true });
});

Task("Restore")
    .Does<BuildData>(data =>
{
    DotNetRestore(solution);
});

Task("Build")
    .Does<BuildData>(data =>
{
    var msbuildSettings = new DotNetMSBuildSettings
    {
      MaxCpuCount = 0,
      Version = data.GitVersion.NuGetVersion,
      AssemblyVersion = data.GitVersion.AssemblySemVer,
      FileVersion = data.GitVersion.AssemblySemFileVer,
      InformationalVersion = data.GitVersion.InformationalVersion,
      ContinuousIntegrationBuild = data.IsRunningOnCI,
      ArgumentCustomization = args => args.Append("/m").Append("/nr:false") // The /nr switch tells msbuild to quite once it's done
    };
    // msbuildSettings.FileLoggers.Add(
    //     new MSBuildFileLoggerSettings
    //     {
    //       LogFile = buildLogFile,
    //       AppendToLogFile = true,
    //       Verbosity = data.DotNetVerbosity
    //     }
    // );

    var settings = new DotNetBuildSettings
    {
      MSBuildSettings = msbuildSettings,
      Configuration = data.Configuration,
      Verbosity = data.DotNetVerbosity,
      NoRestore = true
    };

    DotNetBuild(solution, settings);
});

Task("Pack")
    .ContinueOnError()
    .Does<BuildData>(data =>
{
    EnsureDirectoryExists(Directory(publishDir));

    var msbuildSettings = new DotNetMSBuildSettings
    {
      MaxCpuCount = 0,
      Version = data.GitVersion.NuGetVersion,
      AssemblyVersion = data.GitVersion.AssemblySemVer,
      FileVersion = data.GitVersion.AssemblySemFileVer,
      InformationalVersion = data.GitVersion.InformationalVersion,
      ContinuousIntegrationBuild = data.IsRunningOnCI
    }
    .WithProperty("IncludeBuildOutput", "true")
    .WithProperty("RepositoryBranch", data.GitVersion.BranchName)
    .WithProperty("RepositoryCommit", data.GitVersion.Sha)
    ;
    // msbuildSettings.FileLoggers.Add(
    //     new MSBuildFileLoggerSettings
    //     {
    //       LogFile = buildLogFile,
    //       AppendToLogFile = true,
    //       Verbosity = DotNetVerbosity.Minimal
    //     }
    // );

    var settings = new DotNetPackSettings
    {
      Configuration = data.Configuration,
      OutputDirectory = MakeAbsolute(Directory(publishDir)).FullPath,
      MSBuildSettings = msbuildSettings,
      NoBuild = true,
      NoRestore = true
    };

    var project = "./src/GongSolutions.WPF.DragDrop/GongSolutions.WPF.DragDrop.csproj";
    DotNetPack(project, settings);
});

Task("Sign")
    .WithCriteria<BuildData>((context, data) => !data.IsPullRequest)
    .ContinueOnError()
    .Does<BuildData>(data =>
{
    var files = GetFiles("./src/GongSolutions.WPF.DragDrop/bin/**/*/GongSolutions.WPF.DragDrop.dll");
    SignFiles(files, "GongSolutions.WPF.DragDrop, an easy to use drag'n'drop framework for WPF applications.");

    files = GetFiles("./src/Showcase/bin/**/*/Showcase.WPF.DragDrop.exe");
    SignFiles(files, "Demo application of GongSolutions.WPF.DragDrop, an easy to use drag'n'drop framework for WPF applications.");
});

Task("SignNuGet")
    .WithCriteria<BuildData>((context, data) => !data.IsPullRequest)
    .WithCriteria<BuildData>((context, data) => DirectoryExists(Directory(publishDir)))
    .ContinueOnError()
    .Does(() =>
{
    var vurl = EnvironmentVariable("azure-key-vault-url");
    if(string.IsNullOrWhiteSpace(vurl)) {
        Error("Could not resolve signing url.");
        return;
    }

    var vcid = EnvironmentVariable("azure-key-vault-client-id");
    if(string.IsNullOrWhiteSpace(vcid)) {
        Error("Could not resolve signing client id.");
        return;
    }

    var vctid = EnvironmentVariable("azure-key-vault-tenant-id");
    if(string.IsNullOrWhiteSpace(vctid)) {
        Error("Could not resolve signing client tenant id.");
        return;
    }

    var vcs = EnvironmentVariable("azure-key-vault-client-secret");
    if(string.IsNullOrWhiteSpace(vcs)) {
        Error("Could not resolve signing client secret.");
        return;
    }

    var vc = EnvironmentVariable("azure-key-vault-certificate");
    if(string.IsNullOrWhiteSpace(vc)) {
        Error("Could not resolve signing certificate.");
        return;
    }

    var nugetFiles = GetFiles(publishDir + "/*.nupkg");
    var signTool = Context.Tools.Resolve("NuGetKeyVaultSignTool.exe");

    foreach(var file in nugetFiles)
    {
        Information($"Sign file: {file}");

        ExecuteProcess(signTool,
                        new ProcessArgumentBuilder()
                            .Append("sign")
                            .Append(MakeAbsolute(file).FullPath)
                            .Append("--force")
                            .AppendSwitchQuoted("--file-digest", "sha256")
                            .AppendSwitchQuoted("--timestamp-rfc3161", "http://timestamp.digicert.com")
                            .AppendSwitchQuoted("--timestamp-digest", "sha256")
                            .AppendSwitchQuoted("--azure-key-vault-url", vurl)
                            .AppendSwitchQuotedSecret("--azure-key-vault-client-id", vcid)
                            .AppendSwitchQuotedSecret("--azure-key-vault-tenant-id", vctid)
                            .AppendSwitchQuotedSecret("--azure-key-vault-client-secret", vcs)
                            .AppendSwitchQuotedSecret("--azure-key-vault-certificate", vc)
                        );
    }
});

Task("Zip")
    .Does<BuildData>(data =>
{
    EnsureDirectoryExists(Directory(publishDir));
    Zip($"./src/Showcase/bin/{data.Configuration}", $"{publishDir}/Showcase.DragDrop.{data.Configuration}-v" + data.GitVersion.NuGetVersion + ".zip");
});

Task("StyleXaml")
    .Description("Ensures XAML Formatting is Clean")
    .Does(() =>
{
    Func<IFileSystemInfo, bool> exclude_Dir =
        fileSystemInfo => !fileSystemInfo.Path.Segments.Contains("obj");

    var files = GetFiles(srcDir + "/**/*.xaml", new GlobberSettings { Predicate = exclude_Dir });
    Information("\nChecking " + files.Count() + " file(s) for XAML Structure");
    ExecuteProcess(styler, "-f \"" + string.Join(",", files.Select(f => f.ToString())) + "\" -c \"" + stylerFile + "\"");
});

Task("CreateRelease")
    .WithCriteria<BuildData>((context, data) => !data.IsPullRequest)
    .Does<BuildData>(data =>
{
    var token = EnvironmentVariable("GITHUB_TOKEN");
    if (string.IsNullOrEmpty(token))
    {
        throw new Exception("The GITHUB_TOKEN environment variable is not defined.");
    }

    GitReleaseManagerCreate(token, "punker76", repoName, new GitReleaseManagerCreateSettings {
        Milestone         = data.GitVersion.MajorMinorPatch,
        Name              = data.GitVersion.AssemblySemFileVer,
        Prerelease        = data.IsPrerelease,
        TargetCommitish   = data.GitVersion.BranchName,
        WorkingDirectory  = "."
    });
});

///////////////////////////////////////////////////////////////////////////////
// HELPER
///////////////////////////////////////////////////////////////////////////////

void SignFiles(IEnumerable<FilePath> files, string description)
{
    var vurl = EnvironmentVariable("azure-key-vault-url");
    if(string.IsNullOrWhiteSpace(vurl)) {
        Error("Could not resolve signing url.");
        return;
    }

    var vcid = EnvironmentVariable("azure-key-vault-client-id");
    if(string.IsNullOrWhiteSpace(vcid)) {
        Error("Could not resolve signing client id.");
        return;
    }

    var vctid = EnvironmentVariable("azure-key-vault-tenant-id");
    if(string.IsNullOrWhiteSpace(vctid)) {
        Error("Could not resolve signing client tenant id.");
        return;
    }

    var vcs = EnvironmentVariable("azure-key-vault-client-secret");
    if(string.IsNullOrWhiteSpace(vcs)) {
        Error("Could not resolve signing client secret.");
        return;
    }

    var vc = EnvironmentVariable("azure-key-vault-certificate");
    if(string.IsNullOrWhiteSpace(vc)) {
        Error("Could not resolve signing certificate.");
        return;
    }

    var filesToSign = string.Join(" ", files.Select(f => MakeAbsolute(f).FullPath));
    var azureSignTool = Context.Tools.Resolve("azuresigntool.exe");

    ExecuteProcess(azureSignTool,
                    new ProcessArgumentBuilder()
                        .Append("sign")
                        .Append(filesToSign)
                        .AppendSwitchQuoted("--file-digest", "sha256")
                        .AppendSwitchQuoted("--description", description)
                        .AppendSwitchQuoted("--description-url", "https://github.com/punker76/gong-wpf-dragdrop")
                        .Append("--no-page-hashing")
                        .AppendSwitchQuoted("--timestamp-rfc3161", "http://timestamp.digicert.com")
                        .AppendSwitchQuoted("--timestamp-digest", "sha256")
                        .AppendSwitchQuoted("--azure-key-vault-url", vurl)
                        .AppendSwitchQuotedSecret("--azure-key-vault-client-id", vcid)
                        .AppendSwitchQuotedSecret("--azure-key-vault-tenant-id", vctid)
                        .AppendSwitchQuotedSecret("--azure-key-vault-client-secret", vcs)
                        .AppendSwitchQuotedSecret("--azure-key-vault-certificate", vc)
                    );
}

void ExecuteProcess(FilePath fileName, ProcessArgumentBuilder arguments, string workingDirectory = null)
{
  if (!FileExists(fileName))
  {
    throw new Exception($"File not found: {fileName}");
  }

  var processSettings = new ProcessSettings
  {
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    Arguments = arguments
  };

  if (!string.IsNullOrEmpty(workingDirectory))
  {
    processSettings.WorkingDirectory = workingDirectory;
  }

  Information($"Arguments: {arguments.RenderSafe()}");

  using(var process = StartAndReturnProcess(fileName, processSettings))
  {
    process.WaitForExit();

    if (process.GetStandardOutput().Any())
    {
      Information($"Output:{Environment.NewLine} {string.Join(Environment.NewLine, process.GetStandardOutput())}");
    }

    if (process.GetStandardError().Any())
    {
      // Information($"Errors occurred:{Environment.NewLine} {string.Join(Environment.NewLine, process.GetStandardError())}");
      throw new Exception($"Errors occurred:{Environment.NewLine} {string.Join(Environment.NewLine, process.GetStandardError())}");
    }

    // This should output 0 as valid arguments supplied
    var exitCode = process.GetExitCode();
    Information($"Exit code: {exitCode}");

    if (exitCode > 0)
    {
      throw new Exception($"Exit code: {exitCode}");
    }
  }
}

///////////////////////////////////////////////////////////////////////////////
// TASK TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("StyleXaml")
    .IsDependentOn("Build")
    ;

Task("ci")
    .IsDependentOn("Default")
    .IsDependentOn("Sign")
    .IsDependentOn("Pack")
    .IsDependentOn("SignNuGet")
    .IsDependentOn("Zip")
    ;

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
