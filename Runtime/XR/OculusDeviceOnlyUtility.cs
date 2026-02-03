using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OculusDeviceOnlyUtility : MonoBehaviour
{
#if PLAYERPLATFORM_OCULUS
	[Header("Settings")]
	public OVRPlugin.SystemHeadset oculusDevice;

	[Tooltip("When true, the object will be enabled when either Quest 3 OR Quest 3S are detected")]
	public bool combineQuest3s = true;

	[Header("Events")]
	public UnityEvent onDeviceMatch;


	private void OnEnable()
	{
		var headsetType = OVRPlugin.GetSystemHeadsetType();

		if (IsDeviceMatch(headsetType))
		{
			onDeviceMatch?.Invoke();

			return;
		}


		gameObject.SetActive(false);
	}


	private bool IsDeviceMatch(OVRPlugin.SystemHeadset headset)
	{
		// * Treat links as a device match

		// When combineQuest3s is true, treat Meta Quest 3 and Meta Quest 3S as the same
		if (combineQuest3s)
		{
			switch (headset)
			{
				case OVRPlugin.SystemHeadset.Meta_Quest_3:
				case OVRPlugin.SystemHeadset.Meta_Link_Quest_3:
				case OVRPlugin.SystemHeadset.Meta_Quest_3S:
				case OVRPlugin.SystemHeadset.Meta_Link_Quest_3S:
					return 
						oculusDevice == OVRPlugin.SystemHeadset.Meta_Quest_3S || 
						oculusDevice == OVRPlugin.SystemHeadset.Meta_Link_Quest_3S ||
						oculusDevice == OVRPlugin.SystemHeadset.Meta_Quest_3 || 
						oculusDevice == OVRPlugin.SystemHeadset.Meta_Link_Quest_3;
			}
		}

		switch (headset)
		{
			case OVRPlugin.SystemHeadset.Oculus_Quest:
			case OVRPlugin.SystemHeadset.Oculus_Link_Quest:
				return oculusDevice == OVRPlugin.SystemHeadset.Oculus_Quest || oculusDevice == OVRPlugin.SystemHeadset.Oculus_Link_Quest;
			case OVRPlugin.SystemHeadset.Oculus_Quest_2:
			case OVRPlugin.SystemHeadset.Oculus_Link_Quest_2:
				return oculusDevice == OVRPlugin.SystemHeadset.Oculus_Quest_2 || oculusDevice == OVRPlugin.SystemHeadset.Oculus_Link_Quest_2;
			case OVRPlugin.SystemHeadset.Meta_Quest_Pro:
			case OVRPlugin.SystemHeadset.Meta_Link_Quest_Pro:
				return oculusDevice == OVRPlugin.SystemHeadset.Meta_Quest_Pro || oculusDevice == OVRPlugin.SystemHeadset.Meta_Link_Quest_Pro;
			case OVRPlugin.SystemHeadset.Meta_Quest_3:
			case OVRPlugin.SystemHeadset.Meta_Link_Quest_3:
				return oculusDevice == OVRPlugin.SystemHeadset.Meta_Quest_3 || oculusDevice == OVRPlugin.SystemHeadset.Meta_Link_Quest_3;
			case OVRPlugin.SystemHeadset.Meta_Quest_3S:
			case OVRPlugin.SystemHeadset.Meta_Link_Quest_3S:
				return oculusDevice == OVRPlugin.SystemHeadset.Meta_Quest_3S || oculusDevice == OVRPlugin.SystemHeadset.Meta_Link_Quest_3S;
		}

		return headset == oculusDevice;
	}
#else
	private void OnEnable()
	{
		Debug.LogWarning("Oculus Device Only Utility requires Oculus Integration package to be installed and enabled in Player Settings.");
		gameObject.SetActive(false);
	}
#endif
}
