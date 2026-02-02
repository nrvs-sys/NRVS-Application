using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationModeOnly : MonoBehaviour
{
	public ApplicationInfo.ApplicationMode applicationMode;

	private void OnEnable()
	{
		if (ApplicationInfo.applicationMode != applicationMode)
			gameObject.SetActive(false);
	}
}