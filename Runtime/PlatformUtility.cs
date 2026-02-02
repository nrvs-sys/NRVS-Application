using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlatformUtility : MonoBehaviour
{
	/// <summary>
	/// Specifies the quality settings when using the Android target in-editor - either Quest2 or Quest3. This is ignored in builds, or if a Quest 2/3 override is set (this is preferred).
	/// </summary>
	const AndroidPlatformOverride androidEditorPlatform = AndroidPlatformOverride.Quest3;

	public enum AndroidPlatformOverride
	{
		Quest2,
		Quest3
	}

	[Header("Configuration")]
	[Tooltip("If true, will log detected platform to console")]
	public bool logToConsole = false;

	[Header("Events")]
	public UnityEvent onQuest2;
	public UnityEvent onQuest3;
	[Tooltip("Fired when an Oculus headset is detected other than Quest 2 or 3")]
	public UnityEvent onOculusOther;
	public UnityEvent onPC;


	private void Start()
	{
#if UNITY_EDITOR && UNITY_ANDROID
		if (logToConsole)
			Debug.Log("[PlatformUtility] Detected Android platform in Editor, simulating " + androidEditorPlatform);

		switch (androidEditorPlatform)
		{
			case AndroidPlatformOverride.Quest2:
			onQuest2?.Invoke();
			break;

			case AndroidPlatformOverride.Quest3:
			onQuest3?.Invoke();
			break;
		}

		return;
#endif
		if (Application.platform == RuntimePlatform.Android)
		{
			var headsetType = OVRPlugin.GetSystemHeadsetType();

			if (logToConsole)
				Debug.Log($"[PlatformUtility] Detected Android platform with headset type: {headsetType}");

			switch (headsetType)
			{
				case OVRPlugin.SystemHeadset.Oculus_Quest_2:
					onQuest2?.Invoke();
					break;
				case OVRPlugin.SystemHeadset.Meta_Quest_3:
				case OVRPlugin.SystemHeadset.Meta_Quest_3S:
					onQuest3?.Invoke();
					break;

				case OVRPlugin.SystemHeadset.None:
				case OVRPlugin.SystemHeadset.Oculus_Quest:
				case OVRPlugin.SystemHeadset.Meta_Quest_Pro:
				case OVRPlugin.SystemHeadset.Placeholder_13:
				case OVRPlugin.SystemHeadset.Placeholder_14:
				case OVRPlugin.SystemHeadset.Rift_DK1:
				case OVRPlugin.SystemHeadset.Rift_DK2:
				case OVRPlugin.SystemHeadset.Rift_CV1:
				case OVRPlugin.SystemHeadset.Rift_CB:
				case OVRPlugin.SystemHeadset.Rift_S:
				case OVRPlugin.SystemHeadset.Oculus_Link_Quest:
				case OVRPlugin.SystemHeadset.Oculus_Link_Quest_2:
				case OVRPlugin.SystemHeadset.Meta_Link_Quest_Pro:
				case OVRPlugin.SystemHeadset.Meta_Link_Quest_3:
				case OVRPlugin.SystemHeadset.Meta_Link_Quest_3S:
				case OVRPlugin.SystemHeadset.PC_Placeholder_4106:
				case OVRPlugin.SystemHeadset.PC_Placeholder_4107:
				default:
					onOculusOther?.Invoke();
					break;
			}
		}
		else
		{
			// Assume PC
			onPC?.Invoke();
		}
	}
}
