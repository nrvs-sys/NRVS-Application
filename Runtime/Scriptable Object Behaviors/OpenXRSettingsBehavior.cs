using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR.OpenXR;

[CreateAssetMenu(fileName = "OpenXR Settings Behavior_ New", menuName = "Behaviors/Application/OpenXR Settings")]
public class OpenXRSettingsBehavior : ScriptableObject
{
    public void SetDepthSubmissionMode(OpenXRSettings.DepthSubmissionMode mode) => OpenXRSettings.Instance.depthSubmissionMode = mode;
    public void SetOptimizeBufferDiscards(bool optimizeBufferDiscards) => OpenXRSettings.Instance.optimizeBufferDiscards = optimizeBufferDiscards;
    public void SetRenderMode(OpenXRSettings.RenderMode mode) => OpenXRSettings.Instance.renderMode = mode;
    public void SetSymmetricProjection(bool symmetricProjection) => OpenXRSettings.Instance.symmetricProjection = symmetricProjection;

    public void RefreshRecenterSpace() => OpenXRSettings.RefreshRecenterSpace();

    public void SetAllowRecentering(bool allowRecentering) => OpenXRSettings.SetAllowRecentering(allowRecentering);
    public void SetAllowRecentering(bool allowRecentering, float floorOffset) => OpenXRSettings.SetAllowRecentering(allowRecentering, floorOffset);
}
