#:sdk Cake.Sdk@7.0.0

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

////////////////////////////////////////////////////////////////
// Tasks

Task("Clean")
    .Does(context =>
{
    CleanDirectory("./.artifacts");
});

Task("Lint")
    .Does(context =>
{
    DotNetFormatStyle("./src/Wcwidth.slnx", new DotNetFormatSettings
    {
        VerifyNoChanges = true,
    });
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Lint")
    .Does(context => 
{
    DotNetBuild("./src/Wcwidth.slnx", new DotNetBuildSettings {
        Configuration = configuration,
        NoIncremental = context.HasArgument("rebuild"),
        MSBuildSettings = new DotNetMSBuildSettings()
            .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
    });
});


Task("Test")
    .IsDependentOn("Build")
    .Does(context => 
{
    DotNetTest("./src/Wcwidth.slnx", new DotNetTestSettings {
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true,
    });
});

Task("Package")
    .IsDependentOn("Test")
    .Does(context => 
{
    context.DotNetPack($"./src/Wcwidth.slnx", new DotNetPackSettings {
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true,
        OutputDirectory = "./.artifacts",
        IncludeSource = true,
        MSBuildSettings = new DotNetMSBuildSettings()
            .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
    });
});

Task("Smoke-Test")
    .IsDependentOn("Package")
    .Does(context =>
{
    // Compiles the packed Wcwidth.Sources package in the setups of consuming projects
    var package = context.GetFiles("./.artifacts/Wcwidth.Sources.*.nupkg").Single();
    var version = package.GetFilenameWithoutExtension().FullPath["Wcwidth.Sources.".Length..];

    var setups = new (string Name, (string Key, string Value)[] Properties)[]
    {
        ("public, nullable enabled, latest C#", []),
        ("internal, nullable disabled, System.Memory on netstandard2.0",
            [("WcwidthInternal", "true"), ("Nullable", "disable"), ("WcwidthSystemMemory", "true")]),
        ("internal, nullable enabled, C# 12", [("WcwidthInternal", "true"), ("LangVersion", "12")]),
    };

    foreach (var (name, properties) in setups)
    {
        context.Information("Smoke testing Wcwidth.Sources {0}: {1}", version, name);

        var settings = new DotNetMSBuildSettings().WithProperty("WcwidthSourcesVersion", version);
        foreach (var (key, value) in properties)
        {
            settings.WithProperty(key, value);
        }

        DotNetBuild("./src/Wcwidth.Sources.Smoke/Wcwidth.Sources.Smoke.csproj", new DotNetBuildSettings
        {
            Configuration = configuration,
            NoIncremental = true,
            MSBuildSettings = settings,
        });
    }
});

Task("Publish-NuGet")
    .WithCriteria(ctx => BuildSystem.IsRunningOnGitHubActions, "Not running on GitHub Actions")
    .IsDependentOn("Smoke-Test")
    .Does(context => 
{
    var apiKey = Argument<string?>("nuget-key", null);
    if(string.IsNullOrWhiteSpace(apiKey)) {
        throw new CakeException("No NuGet API key was provided.");
    }

    // Publish to GitHub Packages
    foreach(var file in context.GetFiles("./.artifacts/*.nupkg")) 
    {
        context.Information("Publishing {0}...", file.GetFilename().FullPath);
        DotNetNuGetPush(file.FullPath, new DotNetNuGetPushSettings
        {
            Source = "https://api.nuget.org/v3/index.json",
            ApiKey = apiKey,
        });
    }
});

////////////////////////////////////////////////////////////////
// Targets

Task("Publish")
    .IsDependentOn("Publish-NuGet");

Task("Default")
    .IsDependentOn("Smoke-Test");

////////////////////////////////////////////////////////////////
// Execution

RunTarget(target);