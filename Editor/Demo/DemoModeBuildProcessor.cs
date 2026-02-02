using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;


public class DemoModeBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 1;

    public static void SetDemoModePackageName()
    {
        // For Android, append ".demo" to the package name
        string packageName = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android);
        if (!packageName.EndsWith(".demo"))
        {
            packageName += ".demo";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, packageName);
            Debug.Log($"Demo Mode Build Preprocessor: Android Package name set to {packageName}");
        }
    }

    public static void RestoreOriginalPackageName()
    {
        // For Android, remove the ".demo" suffix if it exists
        string packageName = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android);
        if (packageName.EndsWith(".demo"))
        {
            packageName = packageName.Substring(0, packageName.Length - 5); // Remove ".demo"
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, packageName);
            Debug.Log($"Demo Mode Build Postprocessor: Android Package name restored to {packageName}");
        }
    }



    public void OnPreprocessBuild(BuildReport report)
    {
#if DEMO_MODE
        // If this is an Android build, change the package name to indicate demo mode
        if (report.summary.platform == BuildTarget.Android)
        {
            SetDemoModePackageName();
        }
#endif
    }

    public void OnPostprocessBuild(BuildReport report)
    {
#if DEMO_MODE
        // If this is an Android build, reset the package name to its original value
        if (report.summary.platform == BuildTarget.Android)
        {
            RestoreOriginalPackageName();
        }
#endif
    }
}
