using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DebugMenuUtility : MonoBehaviour
{
	public UnityEvent<bool> onDebugMenuActive;

	private void OnEnable()
	{
		bool isDebugMenuActive = ApplicationInfo.debugMenuEnabled;

#if UNITY_EDITOR
		isDebugMenuActive = true;
#endif

		onDebugMenuActive?.Invoke(isDebugMenuActive);
	}
}