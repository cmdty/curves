var target = Argument<string>("Target", "Default");
var configuration = Argument<string>("Configuration", "Release");
bool publishWithoutBuild = Argument<bool>("PublishWithoutBuild", false);

var artifactsDirectory = Directory("./artifacts");
var testResultDir = "./temp/";
var isRunningOnBuildServer = !BuildSystem.IsLocalBuild;

var msBuildSettings = new DotNetCoreMSBuildSettings();


if (HasArgument("BuildNumber"))
{
    msBuildSettings.WithProperty("BuildNumber", Argument<string>("BuildNumber"));
    msBuildSettings.WithProperty("VersionSuffix", "alpha" + Argument<string>("BuildNumber"));
}

if (HasArgument("VersionPrefix"))
{
    msBuildSettings.WithProperty("VersionPrefix", Argument<string>("VersionPrefix"));
}

if (HasArgument("PythonVersion"))
{
    msBuildSettings.WithProperty("PythonVersion", Argument<string>("PythonVersion"));
}


Task("Clean-Artifacts")
    .Does(() =>
{
    CleanDirectory(artifactsDirectory);
});

Task("Build")
    .Does(() =>
{
    var dotNetCoreSettings = new DotNetCoreBuildSettings()
            {
                Configuration = configuration,
                MSBuildSettings = msBuildSettings
            };
    DotNetCoreBuild("Cmdty.Curves.sln", dotNetCoreSettings);
});

Task("Test-C#")
    .IsDependentOn("Build")
    .Does(() =>
{
    Information("Cleaning test output directory");
    CleanDirectory(testResultDir);

    var projects = GetFiles("./tests/**/*.Test.csproj");
    
    foreach(var project in projects)
    {
        Information("Testing project " + project);
        DotNetCoreTest(
            project.ToString(),
            new DotNetCoreTestSettings()
            {
                ArgumentCustomization = args=>args.Append($"/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura"),
                Logger = "trx",
                ResultsDirectory = testResultDir,
                Configuration = configuration,
                NoBuild = true
            });
    }
});

Task("Add-NuGetSource")
    .Does(() =>
    {
		if (isRunningOnBuildServer)
		{
			// Get the access token
			string accessToken = EnvironmentVariable("SYSTEM_ACCESSTOKEN");
			if (string.IsNullOrEmpty(accessToken))
			{
				throw new InvalidOperationException("Could not resolve SYSTEM_ACCESSTOKEN.");
			}

			// Add the authenticated feed source
			NuGetAddSource(
				"Cmdty",
				"https://pkgs.dev.azure.com/cmdty/_packaging/cmdty/nuget/v3/index.json",
				new NuGetSourcesSettings
				{
					UserName = "VSTS",
					Password = accessToken
				});
		}
		else
		{
			Information("Not running on build so no need to add Cmdty NuGet source");
		}
    });

Task("Build-Samples")
    .IsDependentOn("Add-NuGetSource")
	.Does(() =>
{
	var dotNetCoreSettings = new DotNetCoreBuildSettings()
        {
            Configuration = configuration,
        };
	DotNetCoreBuild("samples/c#/Cmdty.Curves.Samples.sln", dotNetCoreSettings);
});

Task("Pack-NuGet")
	.IsDependentOn("Build-Samples")
	.IsDependentOn("Test-C#")
	.IsDependentOn("Clean-Artifacts")
	.Does(() =>
{
	var dotNetPackSettings = new DotNetCorePackSettings()
                {
                    Configuration = configuration,
                    OutputDirectory = artifactsDirectory,
                    NoRestore = true,
                    NoBuild = true,
                    MSBuildSettings = msBuildSettings
                };
	DotNetCorePack("src/Cmdty.Curves/Cmdty.Curves.csproj", dotNetPackSettings);
});	

Task("Install-PythonDependencies")
    .Does(() =>
{
    StartProcessThrowOnError("python", "-m pip install --upgrade pip");
    StartProcessThrowOnError("pip", "install -r src/Cmdty.Curves.Python/requirements.txt");
    StartProcessThrowOnError("pip", "install pytest");
    StartProcessThrowOnError("pip", "install twine");
    StartProcessThrowOnError("pip", "install -e src/Cmdty.Curves.Python");
});

var testPythonTask = Task("Test-Python")
	.IsDependentOn("Test-C#")
	.Does(() =>
{
    StartProcessThrowOnError("python", "-m pytest src/Cmdty.Curves.Python/tests --junitxml=junit/test-results.xml");
});

if (isRunningOnBuildServer)
{
    testPythonTask.IsDependentOn("Install-PythonDependencies");
}

Task("Pack-Python")
    .IsDependentOn("Test-Python")
.Does(setupContext =>
{
    CleanDirectory("src/Cmdty.Curves.Python/build");
    CleanDirectory("src/Cmdty.Curves.Python/dist");
    var originalWorkingDir = setupContext.Environment.WorkingDirectory;
    string pythonProjDir = System.IO.Path.Combine(originalWorkingDir.ToString(), "src", "Cmdty.Curves.Python");
    setupContext.Environment.WorkingDirectory = pythonProjDir;
    try
    {    
        StartProcessThrowOnError("python", "setup.py", "sdist", "bdist_wheel");
    }
    finally
    {
        setupContext.Environment.WorkingDirectory = originalWorkingDir;
    }
});

Task("Push-NuGetToCmdtyFeed")
    .IsDependentOn("Add-NuGetSource")
    .IsDependentOn("Pack-NuGet")
    .Does(() =>
{
    var nupkgPath = GetFiles(artifactsDirectory.ToString() + "/*.nupkg").Single();
    Information($"Pushing NuGetPackage in {nupkgPath} to Cmdty feed");
    NuGetPush(nupkgPath, new NuGetPushSettings 
    {
        Source = "Cmdty",
        ApiKey = "VSTS"
    });
});

private void StartProcessThrowOnError(string applicationName, params string[] processArgs)
{
    var argsBuilder = new ProcessArgumentBuilder();
    foreach(string processArg in processArgs)
    {
        argsBuilder.Append(processArg);
    }
    int exitCode = StartProcess(applicationName, new ProcessSettings {Arguments = argsBuilder});
    if (exitCode != 0)
        throw new ApplicationException($"Starting {applicationName} in new process returned non-zero exit code of {exitCode}");
}

private string GetEnvironmentVariable(string envVariableName)
{
    string envVariableValue = EnvironmentVariable(envVariableName);
    if (string.IsNullOrEmpty(envVariableValue))
        throw new ApplicationException($"Environment variable '{envVariableName}' has not been set.");
    return envVariableValue;
}

var publishTestPyPiTask = Task("Publish-TestPyPI")
    .Does(() =>
{
    string testPyPiPassword = GetEnvironmentVariable("TEST_PYPI_PASSWORD");
    StartProcessThrowOnError("python", "-m twine upload --repository-url https://test.pypi.org/legacy/ src/Cmdty.Curves.Python/dist/*",
                                        "--username fowja", "--password " + testPyPiPassword);
});

var publishPyPiTask = Task("Publish-PyPI")
    .Does(() =>
{
    string pyPiPassword = GetEnvironmentVariable("PYPI_PASSWORD");
    StartProcessThrowOnError("python", "-m twine upload src/Cmdty.Curves.Python/dist/*",
                                        "--username cmdty", "--password " + pyPiPassword);
});

var publishNuGetTask = Task("Publish-NuGet")
    .Does(() =>
{
    string nugetApiKey = GetEnvironmentVariable("CURVES_NUGET_API_KEY");

    var nupkgPath = GetFiles(artifactsDirectory.ToString() + "/*.nupkg").Single();

    NuGetPush(nupkgPath, new NuGetPushSettings 
    {
        ApiKey = nugetApiKey,
        Source = "https://api.nuget.org/v3/index.json"
    });
});

if (!publishWithoutBuild)
{
    publishTestPyPiTask.IsDependentOn("Pack-Python");
    publishPyPiTask.IsDependentOn("Pack-Python");
    publishNuGetTask.IsDependentOn("Pack-NuGet");
}
else
{
    Information("Publishing without first building as PublishWithoutBuild variable set to true.");
}


Task("Default")
	.IsDependentOn("Pack-NuGet")
    .IsDependentOn("Pack-Python");

Task("CI")
	.IsDependentOn("Push-NuGetToCmdtyFeed")
    .IsDependentOn("Pack-Python");

RunTarget(target);