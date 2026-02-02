using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEditor.XR.Management;

public static class XRPluginUtility
{
    /// <summary>
    /// Returns the current list of registered XRLoaders for the given group.
    /// </summary>
    public static List<XRLoader> GetCurrentLoaders(BuildTargetGroup group)
    {
        var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(group);
        if (settings?.Manager == null)
            return new List<XRLoader>();
        // make a copy so caller can stash it
        return new List<XRLoader>(settings.Manager.loaders);
    }

    /// <summary>
    /// Overwrites the XR loaders for the given group with exactly this list.
    /// </summary>
    public static void SetLoaders(BuildTargetGroup group, List<XRLoader> loaders)
    {
        var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(group);
        if (settings?.Manager == null)
            return;
        settings.Manager.loaders = new List<XRLoader>(loaders);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Clears out *all* XR loaders for the given group.
    /// </summary>
    public static void ClearLoaders(BuildTargetGroup group)
    {
        var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(group);
        if (settings?.Manager == null)
            return;
        settings.Manager.loaders.Clear();
        AssetDatabase.SaveAssets();
    }
}
