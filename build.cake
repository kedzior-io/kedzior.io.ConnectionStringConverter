var target = Argument("Target", "Default");
var configuration =
    HasArgument("Configuration") ? Argument<string>("Configuration") :
    EnvironmentVariable("Configuration") != null ? EnvironmentVariable("Configuration") :
    "Release";
var preReleaseSuffix = "";
    // HasArgument("PreReleaseSuffix") ? Argument<string>("PreReleaseSuffix") :
    // (AppVeyor.IsRunningOnAppVeyor && AppVeyor.Environment.Repository.Tag.IsTag) ? null :
    // EnvironmentVariable("PreReleaseSuffix") != null ? EnvironmentVariable("PreReleaseSuffix") :
    // "beta";
var buildNumber =
    HasArgument("BuildNumber") ? Argument<int>("BuildNumber") :
    AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Build.Number :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Build.BuildNumber :
    EnvironmentVariable("BuildNumber") != null ? int.Parse(EnvironmentVariable("BuildNumber")) :
    0;

var artifactsDirectory = Directory("./artifacts");
string versionSuffix = null;
if (!string.IsNullOrEmpty(preReleaseSuffix))
{
    versionSuffix = preReleaseSuffix + "-" + buildNumber.ToString("D4");
}

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDirectory);
        DeleteDirectories(GetDirectories("**/bin"), new DeleteDirectorySettings {
			Recursive = true,
			Force = true
		});
        DeleteDirectories(GetDirectories("**/obj"), new DeleteDirectorySettings {
			Recursive = true,
			Force = true
		});
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore();
    });

 Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetCoreBuild(
            ".",
            new DotNetCoreBuildSettings()
            {
                Configuration = configuration,
                VersionSuffix = versionSuffix
            });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        foreach(var project in GetFiles("./test/**/*Tests.csproj"))
        {
            var outputFilePath = MakeAbsolute(artifactsDirectory.Path)
                .CombineWithFilePath(project.GetFilenameWithoutExtension());
            DotNetCoreTool(
                project,
                "xunit",
                new ProcessArgumentBuilder()
                    .AppendSwitch("-configuration", configuration)
                    .AppendSwitchQuoted("-xml", outputFilePath.AppendExtension(".xml").ToString())
                    .AppendSwitchQuoted("-html", outputFilePath.AppendExtension(".html").ToString()));
        }
    });

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
    {
        DotNetCorePack(
            ".",
            new DotNetCorePackSettings()
            {
                Configuration = configuration,
                OutputDirectory = artifactsDirectory,
                VersionSuffix = versionSuffix
            });
    });

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);