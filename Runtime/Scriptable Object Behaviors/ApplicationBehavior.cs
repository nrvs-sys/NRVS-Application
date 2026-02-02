using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Application_ New", menuName = "Behaviors/Application/Application")]
public class ApplicationBehavior : ScriptableObject
{
    public void SetTargetFramerate(int targetFramerate)
    {
        Application.targetFrameRate = targetFramerate;
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    public void PauseEditor()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = true;
#endif
    }

    public void EnableDebugMenu() => ApplicationInfo.debugMenuEnabled = true;

    public void UnloadUnusedAssets() => Resources.UnloadUnusedAssets();

    public void GCCollect() => System.GC.Collect();
}
