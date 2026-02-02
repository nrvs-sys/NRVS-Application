using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

/// <summary>
/// TODO: Several methods currently only implemented for Oculus on Android. Needs to be implemented for OpenXR, most likely by updating to Unity 6+ and related OpenXR plugins.
/// </summary>
public static class XRRefreshRates
{
    public const int defaultRefreshRate = 72;

    public static float GetCurrentRefreshRate()
    {
#if UNITY_ANDROID
        return OVRPlugin.systemDisplayFrequency;
#else
        var xrDisplay = GetCurrentXRDisplaySubsystem();

        if (xrDisplay == null)
        {
            Debug.LogWarning("GetCurrentRefreshRate: No XR Display Subsystem found.");
            return defaultRefreshRate;
        }

        return xrDisplay.TryGetDisplayRefreshRate(out var rate) ? rate : defaultRefreshRate;
#endif
    }

    public static float[] GetAvailableRefreshRates()
    {
#if UNITY_ANDROID
        return OVRPlugin.systemDisplayFrequenciesAvailable;
#else
        var xrDisplay = GetCurrentXRDisplaySubsystem();

        if (xrDisplay == null)
        {
            Debug.LogWarning("GetAvailableRefreshRates: No XR Display Subsystem found.");
            return new float[] { defaultRefreshRate };
        }

        Debug.LogWarning("GetAvailableRefreshRates is not currently supported on this platform.");
        //return xrDisplay.TryGetAvailableDisplayRefreshRates(out var rates) ? rates : new float[] { defaultRefreshRate };

        return new float[] { defaultRefreshRate };
#endif
    }

    public static bool TrySetRefreshRate(float rate)
    {
#if UNITY_ANDROID
        OVRPlugin.systemDisplayFrequency = rate;
        return true;
#else
        var xrDisplay = GetCurrentXRDisplaySubsystem();

        if (xrDisplay == null)
        {
            Debug.LogWarning("SetRefreshRate: No XR Display Subsystem found.");
            return false;
        }

        Debug.LogWarning("SetRefreshRate is not currently supported on this platform.");
        //return xrDisplay.TrySetDisplayRefreshRate(rate);

        return false;
#endif
    }

    static XRDisplaySubsystem GetCurrentXRDisplaySubsystem()
    {
        return XRGeneralSettings.Instance?
            .Manager?
            .activeLoader?
            .GetLoadedSubsystem<XRDisplaySubsystem>();
    }
}
