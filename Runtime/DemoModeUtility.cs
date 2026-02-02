using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DemoModeUtility : MonoBehaviour
{
	public bool demoModeEnabled = true;
	[Tooltip("Invoked when the specified demo mode state is active on enable. Trigger actions here that should only happen when Demo Mode matches the 'demoModeEnabled' flag.")]
	public UnityEvent onDemoModeActive;

	private void OnEnable()
	{
		if (ApplicationInfo.demoMode == demoModeEnabled)
			onDemoModeActive?.Invoke();
	}
}