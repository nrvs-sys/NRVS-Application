using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Application Version", menuName = "Settings/Application Version")]
public class ApplicationVersion : ScriptableObject
{
    [SerializeField]
    private string versionName;

    public string GetVersion() => $"{(ApplicationInfo.demoMode ? "Demo" : versionName)}///{Application.version}";

    public void LogVersion() => Debug.Log($"Application Version: {GetVersion()}");
}