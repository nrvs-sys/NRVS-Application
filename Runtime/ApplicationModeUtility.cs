using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ApplicationModeUtility : MonoBehaviour
{
	public ApplicationInfo.ApplicationMode applicationMode;

	public UnityEvent onApplicationModeActive;

	private void OnEnable()
	{
		if (ApplicationInfo.applicationMode == applicationMode)
			onApplicationModeActive?.Invoke();
	}
}