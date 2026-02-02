using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to display messages for story, tutorials, and training in the player HUD
/// </summary>
public class InfoMessage : Singleton<InfoMessage>
{
	private List<Message> messages = new List<Message>();
	private Message currentMessage;

	public class Message
	{
		// TODO - use an enum for completion mode instead of multiple bools/fields
		public enum CompletionMode
		{
			Duration,
			Progress,
			Manual
		}

		public string messageText;
		public float duration = -1;
		public int currentProgress = -1;
		public int totalProgress = -1;
		public bool manualProgression = false;
		public bool readyToProgress = false;
		/// <summary>
		/// Used to skip progression holds on the message display
		/// </summary>
		public bool completeImmediately = false;
		public bool skipFades = false;
		public float displayDelay = 0f;
		public Action CompleteCallback;
	}


	protected override void OnSingletonInitialized() { }

	public static void Log(Message message)
	{
		if (Instance == null)
			return;
		//Debug.LogError($"logging message: {message.messageText}");
		Instance.messages.Add(message);
	}

	public static bool TryConsume(out Message message)
	{
		if (Instance != null && Instance.messages.Count > 0)
		{
			message = Instance.messages[0];
			Instance.messages.RemoveAt(0);
			//Debug.LogError($"consuming message: {message.messageText}");
			Instance.currentMessage = message;

			return true;
		}
		else
		{
			message = null;
			return false;
		}
	}

	public static void Remove(Message message)
	{
		if (Instance != null)
		{
			//Debug.LogError($"removing message: {message.messageText}");
			Instance.messages.Remove(message);
		}
	}

	public static bool Exists(Message message) => Instance != null && Instance.messages.Contains(message);

	public static void Clear()
	{
		if (Instance == null)
			return;

		Instance.messages.Clear();
	}

	public static void CompleteCurrent()
	{
		if (Instance == null)
			return;

		if (Instance.currentMessage != null)
			Instance.currentMessage.completeImmediately = true;
	}
}