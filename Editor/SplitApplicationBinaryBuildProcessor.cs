using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public class SplitApplicationBinaryBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.Android && PlayerSettings.Android.splitApplicationBinary)
        {
            // get the name of the application binary
            string buildFileName = Constants.BuildInfo.executableName;
            // strip the file and extension from the path
            string buildDirectory = Path.GetDirectoryName(report.summary.outputPath);
            string obbFileName = $"{buildFileName}.main.obb";
            // get the path to the OBB file
            string obbFilePath = Path.Combine(buildDirectory, obbFileName);

            if (File.Exists(obbFilePath))
            {
                string bundleVersionCode = PlayerSettings.Android.bundleVersionCode.ToString();
                string packageName = PlayerSettings.applicationIdentifier;
                string newObbFileName = $"main.{bundleVersionCode}.{packageName}.obb";
                string newObbFilePath = Path.Combine(buildDirectory, newObbFileName);

                File.Move(obbFilePath, newObbFilePath);
                Debug.Log($"Split Application Binary Build Processor: OBB file renamed from `{obbFileName}` to `{newObbFileName}`");
            }
            else
            {
                Debug.LogWarning($"Split Application Binary Build Processor: OBB file not found at path: {obbFilePath}");
            }
        }
    }
}