using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DevelopmentBuildUtility : MonoBehaviour
{
	public UnityEvent onDevelopmentBuildActive;
	public UnityEvent onDevelopmentBuildInactive;

	private void OnEnable()
	{
#if UNITY_EDITOR
        onDevelopmentBuildActive?.Invoke();

#else
        if (Debug.isDebugBuild)
			onDevelopmentBuildActive?.Invoke();
		else
			onDevelopmentBuildInactive?.Invoke();
#endif
	}	
}