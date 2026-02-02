using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "System Message_ New", menuName = "Behaviors/System Message")]
public class SystemMessageBehavior : ScriptableObject
{
	[Header("Settings")]
	public LocalizedString localizedMessage;

	public void LogMessage()
	{
		localizedMessage.GetLocalizedStringAsync().Completed += handle =>
		{
			SystemMessage.Log(handle.Result);
		};
	}
}