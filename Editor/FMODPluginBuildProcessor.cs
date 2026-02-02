using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using FMODUnity;

public class FMODPluginBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        // Call the FMODUnity EventManager Startup method
        // Fix ensures OBB mode is turned on in the FMOD settings for Android builds
        // Found: https://qa.fmod.com/t/unity-android-split-build-not-finding-banks-in-obb/17756/3
        EventManager.Startup();
        Debug.Log("FMODUnity EventManager Startup called before build.");
    }
}