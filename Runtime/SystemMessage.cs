using Services;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SystemMessage : Singleton<SystemMessage>
{
	[Header("References")]
	public PlatformErrorData platformErrorData;

	private Queue<Message> messages = new Queue<Message>();

	public struct Message
	{
		public string messageText;
		public DateTime time;
	}


	protected override void OnSingletonInitialized() { }

	public static void Log(string message)
	{
		if (Instance == null)
			return;

		Instance.messages.Enqueue(new Message { messageText = message, time = DateTime.Now });
	}

	public static void LogError(string platformName, string error)
	{
		if (Instance == null)
			return;

		var platformError = Instance.platformErrorData.GetEntry(platformName, error);

		platformError.localizedError.GetLocalizedStringAsync().Completed += handle =>
		{
			Log(handle.Result);
		};
	}

	public static bool TryConsume(out Message message)
	{
		if (Instance != null && Instance.messages.Count > 0)
		{
			message = Instance.messages.Dequeue();
			return true;
		}

		message = default;
		return false;
	}
}