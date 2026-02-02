using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCVROnly : MonoBehaviour
{
	private void OnEnable()
	{
		if (ApplicationInfo.platform == ApplicationInfo.Platform.PC && ApplicationInfo.applicationMode == ApplicationInfo.ApplicationMode.XR)
			return;

		gameObject.SetActive(false);
	}
}