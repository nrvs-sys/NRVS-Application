using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Build_ New", menuName = "Behaviors/Build")]
public class BuildBehavior : ScriptableObject
{
    #region Types

    //
    // Summary:
    //     Building options. Multiple options can be combined together.
    [Serializable, Flags]
    public enum BuildOptions
    {
        //
        // Summary:
        //     Perform the specified build without any special settings or extra tasks.
        None = 0,
        //
        // Summary:
        //     Build a development version of the player.
        Development = 1,
        //
        // Summary:
        //     Run the built player.
        AutoRunPlayer = 4,
        //
        // Summary:
        //     Show the built player.
        ShowBuiltPlayer = 8,
        //
        // Summary:
        //     For internal use
        BuildAdditionalStreamedScenes = 0x10,
        //
        // Summary:
        //     Used when building Xcode (iOS) or Eclipse (Android) projects.
        AcceptExternalModificationsToPlayer = 0x20,
        InstallInBuildFolder = 0x40,
        //
        // Summary:
        //     Clear all cached build results, resulting in a full rebuild of all scripts and
        //     all player data.
        CleanBuildCache = 0x80,
        //
        // Summary:
        //     Start the player with a connection to the profiler in the editor.
        ConnectWithProfiler = 0x100,
        //
        // Summary:
        //     Allow script debuggers to attach to the player remotely.
        AllowDebugging = 0x200,
        //
        // Summary:
        //     Symlink runtime libraries when generating iOS Xcode project. (Faster iteration
        //     time).
        [Obsolete("BuildOptions.SymlinkLibraries is obsolete. Use BuildOptions.SymlinkSources instead (UnityUpgradable) -> [UnityEditor] BuildOptions.SymlinkSources", false)]
        SymlinkLibraries = 0x400,
        //
        // Summary:
        //     Symlink sources when generating the project. This is useful if you're changing
        //     source files inside the generated project and want to bring the changes back
        //     into your Unity project or a package.
        SymlinkSources = 0x400,
        //
        // Summary:
        //     Don't compress the data when creating the asset bundle.
        UncompressedAssetBundle = 0x800,
        [Obsolete("Use BuildOptions.Development instead")]
        StripDebugSymbols = 0,
        [Obsolete("Texture Compression is now always enabled")]
        CompressTextures = 0,
        //
        // Summary:
        //     Sets the Player to connect to the Editor.
        ConnectToHost = 0x1000,
        //
        // Summary:
        //     Determines if the player should be using the custom connection ID.
        CustomConnectionID = 0x2000,
        //
        // Summary:
        //     Options for building the standalone player in headless mode.
        [Obsolete("Use StandaloneBuildSubtarget.Server instead")]
        EnableHeadlessMode = 0x4000,
        //
        // Summary:
        //     Only build the scripts in a Project.
        BuildScriptsOnly = 0x8000,
        //
        // Summary:
        //     Patch a Development app package rather than completely rebuilding it. Supported
        //     platforms: Android.
        PatchPackage = 0x10000,
        [Obsolete("BuildOptions.IL2CPP is deprecated and has no effect. Use PlayerSettings.SetScriptingBackend() instead.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Il2CPP = 0,
        //
        // Summary:
        //     Include assertions in the build. By default, the assertions are only included
        //     in development builds.
        ForceEnableAssertions = 0x20000,
        //
        // Summary:
        //     Use chunk-based LZ4 compression when building the Player.
        CompressWithLz4 = 0x40000,
        //
        // Summary:
        //     Use chunk-based LZ4 high-compression when building the Player.
        CompressWithLz4HC = 0x80000,
        //
        // Summary:
        //     Force full optimizations for script compilation in Development builds.
        [Obsolete("Specify IL2CPP optimization level in Player Settings.")]
        ForceOptimizeScriptCompilation = 0,
        ComputeCRC = 0x100000,
        //
        // Summary:
        //     Do not allow the build to succeed if any errors are reporting during it.
        StrictMode = 0x200000,
        //
        // Summary:
        //     Build will include Assemblies for testing.
        IncludeTestAssemblies = 0x400000,
        //
        // Summary:
        //     Will force the buildGUID to all zeros.
        NoUniqueIdentifier = 0x800000,
        //
        // Summary:
        //     Sets the Player to wait for player connection on player start.
        WaitForPlayerConnection = 0x2000000,
        //
        // Summary:
        //     Enables code coverage. You can use this as a complimentary way of enabling code
        //     coverage on platforms that do not support command line arguments.
        EnableCodeCoverage = 0x4000000,
        //
        // Summary:
        //     Enables Deep Profiling support in the player.
        EnableDeepProfilingSupport = 0x10000000,
        //
        // Summary:
        //     Generates more information in the BuildReport.
        DetailedBuildReport = 0x20000000,
        //
        // Summary:
        //     Enable Shader Livelink support.
        [Obsolete("Shader LiveLink is no longer supported.")]
        ShaderLivelinkSupport = 0
    }

    #endregion

    [Header("Build Platforms")]

    [SerializeField]
    bool androidOculusActive = true;

    [Space(10)]

    [SerializeField]
    bool windowsStandaloneActive = true;
    [SerializeField]
    bool windowsDedicatedServerActive = true;
    [SerializeField]
    bool windowsOculusActive = true;
    [SerializeField]
    bool windowsSteamActive = true;

    [Header("Build Settings")]
    [SerializeField]
    BuildOptions buildOptions;

    public static bool Build(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, string buildPath, UnityEditor.BuildOptions buildOptions, out UnityEditor.Build.Reporting.BuildReport buildReport, int subTarget = 0)
    {
        if (buildTarget != EditorUserBuildSettings.activeBuildTarget)
        {
            UnityEngine.Debug.Log($"Switching Active Build Target from {EditorUserBuildSettings.activeBuildTarget} to {buildTarget}");
            EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        }

        var currentPlatform = PlayerPlatformUtility.GetPlayerPlatform();
        var platformSymbol = PlayerPlatformUtility.GetPlayerPlatformSymbol(currentPlatform);

        string[] extraScriptingDefines = new string[] { platformSymbol };

        string executableExtension;

        switch (buildTarget)
        {
            case BuildTarget.Android:
                executableExtension = $".{Constants.BuildInfo.executableExtensionAndroid}";
                break;
            case BuildTarget.StandaloneWindows64:
                executableExtension = $".{Constants.BuildInfo.executableExtensionWindows}";
                break;
            default:
                executableExtension = "";
                break;
        }

        // Set build target and options
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
            locationPathName = System.IO.Path.Combine(buildPath, buildTarget.ToString(), platformSymbol.Replace("PLAYERPLATFORM_", ""), $"{Constants.BuildInfo.executableName}{executableExtension}"),
            target = buildTarget,
            subtarget = subTarget,
            options = buildOptions,
            extraScriptingDefines = extraScriptingDefines,
        };

        // Perform build
        buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log($"Build completed: {buildReport.summary.result} for {buildTarget} - {platformSymbol}");

        return buildReport.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded;
    }

    [Button("Build", EButtonEnableMode.Editor)]
    public void Build()
    {

#if UNITY_EDITOR

        bool areAnyPlatformsSelected = androidOculusActive || windowsStandaloneActive || windowsDedicatedServerActive || windowsOculusActive || windowsSteamActive;

        if (!areAnyPlatformsSelected)
        {
            Debug.LogError($"There are no player platforms selected to build! Unable to build all.");
            EditorApplication.Beep();
            return;
        }

        Debug.Log($"Build all starting");

        string timestamp = System.DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss");
        string buildVersion = $"_{UnityEngine.Application.version}_{timestamp}";
        string buildPath = Constants.BuildInfo.buildPath + buildVersion;

        var anyBuildSucceeded = false;

        if (androidOculusActive)
            if (Build(BuildTargetGroup.Android, BuildTarget.Android, buildPath, (UnityEditor.BuildOptions)buildOptions, out var buildReport))
                anyBuildSucceeded = true;

        // TODO - insert step where platform is set to the selected platform, before each build
        // then an asset domain reload is triggered and waited for completion before building

        if (windowsStandaloneActive)
            if (Build(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, buildPath, (UnityEditor.BuildOptions)buildOptions, out var buildReport))
                anyBuildSucceeded = true;

        // TODO - insert step where platform is set to the selected platform, before each build
        // then an asset domain reload is triggered and waited for completion before building

        if (windowsDedicatedServerActive)
            if (Build(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, buildPath, (UnityEditor.BuildOptions)buildOptions, out var buildReport, (int)StandaloneBuildSubtarget.Server))
                anyBuildSucceeded = true;

        // TODO - insert step where platform is set to the selected platform, before each build
        // then an asset domain reload is triggered and waited for completion before building

        if (windowsOculusActive)
            if (Build(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, buildPath, (UnityEditor.BuildOptions)buildOptions, out var buildReport))
                anyBuildSucceeded = true;

        // TODO - insert step where platform is set to the selected platform, before each build
        // then an asset domain reload is triggered and waited for completion before building

        if (windowsSteamActive)
            if (Build(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, buildPath, (UnityEditor.BuildOptions)buildOptions, out var buildReport))
                anyBuildSucceeded = true;

        if (!anyBuildSucceeded)
        {
            Debug.LogError($"No builds succeeded. Unable to open build folder location.");
            EditorApplication.Beep();
            return;
        }

        OpenBuildFolderLocation(buildVersion);

        Debug.Log($"Build all completed");

#endif

    }

    [Button("Open Player Settings", EButtonEnableMode.Editor)]
    public void OpenPlayerSettings()
    {
#if UNITY_EDITOR
        SettingsService.OpenProjectSettings("Project/Player");
#endif
    }

#if UNITY_EDITOR

    private void OpenBuildFolderLocation(string buildVersion)
    {
        string projectFolderPath = System.IO.Directory.GetParent(UnityEngine.Application.dataPath).FullName;
        string buildFolderPath = System.IO.Path.Combine(projectFolderPath, Constants.BuildInfo.buildPath + buildVersion);
        Debug.Log($"opening path ({buildFolderPath})");
        System.Diagnostics.Process.Start("explorer.exe", buildFolderPath);
    }
#endif

}
