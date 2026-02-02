using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quality Settings_ New", menuName = "Behaviors/Application/Quality Settings")]
public class QualitySettingsBehavior : ScriptableObject
{
    /// <summary>
    /// Sets the Vsync level.
    /// 
    /// 0 = Don't sync (utilizes Application.targetFramerate instead)
    /// 1 = Sync to monitor refresh rate
    /// 2 = Sync to half monitor refresh rate (60 FPS on 120Hz monitor)
    /// 3 = Sync to quarter monitor refresh rate (30 FPS on 120Hz monitor)
    /// 4 = Sync to one eighth monitor refresh rate (15 FPS on 120Hz monitor)
    /// </summary>
    /// <param name="level"></param>
    public void SetVsync(int level)
    {
        Debug.Log($"[QualitySettingsBehavior] Setting Vsync to {level}");
        QualitySettings.vSyncCount = level;
    }
}
