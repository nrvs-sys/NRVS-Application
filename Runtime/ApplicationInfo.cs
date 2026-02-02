using System;
using System.Threading;
using System.Threading.Tasks;

public static class ApplicationInfo
{
	public static ApplicationMode applicationMode;
	public enum ApplicationMode
	{ 
		XR,
		Flat,
		Server,
	}

	public static Platform platform =>
#if UNITY_ANDROID //&& !UNITY_EDITOR
		Platform.Quest;
#else
		Platform.PC;
#endif

	public static bool demoMode =>
#if DEMO_MODE
		true;
#else
		false;
#endif

	public enum Platform 
	{ 
		PC,
		Quest
	}

    public static bool infoPageShown;

	public static bool debugMenuEnabled;


	private static InternetAvailabilityStatus _internetAvailabilityStatus = InternetAvailabilityStatus.Unknown;
	public static InternetAvailabilityStatus internetAvailabilityStatus
	{
		get => _internetAvailabilityStatus;
		set
		{
			if (_internetAvailabilityStatus == value)
				return;

			_internetAvailabilityStatus = value;

			OnInternetAvailabilityChanged?.Invoke(_internetAvailabilityStatus);
		}
	}
	public static event InternetAvailabilityChanged OnInternetAvailabilityChanged;
	public delegate void InternetAvailabilityChanged(InternetAvailabilityStatus status);

	public enum InternetAvailabilityStatus
	{
		Unknown,
		Offline,
		Online,
	}

	/// Waits until internetAvailabilityStatus is not Unknown.
	/// Returns the resulting status (Online/Offline). If it times out or is cancelled,
	/// it returns ApplicationInfo.InternetAvailabilityStatus.Unknown.
	public static async Task<InternetAvailabilityStatus> WaitForKnownInternetStatusAsync(TimeSpan? timeout = null, CancellationToken ct = default)
	{
		// fast path if already known
		if (internetAvailabilityStatus != InternetAvailabilityStatus.Unknown)
			return internetAvailabilityStatus;

		var tcs = new TaskCompletionSource<InternetAvailabilityStatus>(TaskCreationOptions.RunContinuationsAsynchronously);

		void Handler(InternetAvailabilityStatus s)
		{
			if (s != InternetAvailabilityStatus.Unknown)
				tcs.TrySetResult(s);
		}

		OnInternetAvailabilityChanged += Handler;

		CancellationTokenSource? timeoutCts = null;
		CancellationTokenRegistration ctr = default;

		try
		{
			if (timeout is TimeSpan t)
			{
				timeoutCts = new CancellationTokenSource(t);
				timeoutCts.Token.Register(() => tcs.TrySetResult(InternetAvailabilityStatus.Unknown));
			}

			ctr = ct.Register(() => tcs.TrySetCanceled(ct));

			// re-check in case it changed between the first check and our subscription
			Handler(internetAvailabilityStatus);

			return await tcs.Task.ConfigureAwait(false);
		}
		finally
		{
			OnInternetAvailabilityChanged -= Handler;
			ctr.Dispose();
			timeoutCts?.Dispose();
		}
	}
}