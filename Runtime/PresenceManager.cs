using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles heartbeat + online player count using Supabase edge functions.
/// Only runs on client modes (XR / Flat) when internet is available.
/// </summary>
public class PresenceManager : MonoBehaviour
{
    [Header("Supabase URLs")]
    [Tooltip("Supabase edge function URL for /hb (heartbeat).")]
    [SerializeField] private string heartbeatUrl = "";

    [Tooltip("Supabase edge function URL for /count (online player count).")]
    [SerializeField] private string countUrl = "";

    [Header("Intervals (seconds)")]
    [SerializeField] private float heartbeatIntervalSeconds = 30f;
    [SerializeField] private float countRefreshIntervalSeconds = 30f;

    [Header("Debug")]
    [SerializeField] private bool logRequests = false;

    public static PresenceManager Instance { get; private set; }

    public enum PresenceServiceState
    {
        Unknown,
        Online,        // internet ok + service responding
        Offline,       // no internet
        ServiceError,  // internet ok, but hb/count failing
    }

    public PresenceServiceState ServiceState { get; private set; } = PresenceServiceState.Unknown;

    public bool OnlineCountValid { get; private set; } = false;

    public event Action<PresenceServiceState> OnPresenceStateChanged;

    public int OnlineCount { get; private set; }
    public event Action<int> OnOnlineCountChanged;

    Coroutine _heartbeatLoop;
    Coroutine _countLoop;

    bool _isClient =>
        ApplicationInfo.applicationMode == ApplicationInfo.ApplicationMode.XR ||
        ApplicationInfo.applicationMode == ApplicationInfo.ApplicationMode.Flat;

    void Awake()
    {
        // server builds should never run presence
        if (!_isClient)
        {
            if (logRequests)
                Debug.Log("[PresenceManager] Not a client mode, disabling presence.");
            Destroy(gameObject);
            return;
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        if (!_isClient) return;

        ApplicationInfo.OnInternetAvailabilityChanged += HandleInternetStatus;

        // sync with current status
        HandleInternetStatus(ApplicationInfo.internetAvailabilityStatus);
    }

    void OnDisable()
    {
        if (!_isClient) return;

        ApplicationInfo.OnInternetAvailabilityChanged -= HandleInternetStatus;
        StopPresenceLoops();
    }

    private void SetServiceState(PresenceServiceState newState)
    {
        if (ServiceState == newState)
            return;

        ServiceState = newState;
        OnPresenceStateChanged?.Invoke(ServiceState);
    }

    void HandleInternetStatus(ApplicationInfo.InternetAvailabilityStatus status)
    {
        if (logRequests)
            Debug.Log($"[PresenceManager] Internet status changed: {status}");

        if (status == ApplicationInfo.InternetAvailabilityStatus.Online)
        {
            StartPresenceLoops();
        }
        else
        {
            StopPresenceLoops();
            TrySetCountInvalid(PresenceServiceState.Offline);
        }
    }

    private void TrySetCountInvalid(PresenceServiceState reason)
    {
        OnlineCountValid = false;
        SetServiceState(reason);

        // don’t change OnlineCount here — preserve last known good value.
        // UI should ignore OnlineCount when OnlineCountValid == false.
    }

    void StartPresenceLoops()
    {
        if (string.IsNullOrWhiteSpace(heartbeatUrl) || string.IsNullOrWhiteSpace(countUrl))
        {
            if (logRequests)
                Debug.LogWarning("[PresenceManager] Heartbeat or count URL is not set.");
            return;
        }

        if (_heartbeatLoop == null)
            _heartbeatLoop = StartCoroutine(HeartbeatLoop());

        if (_countLoop == null)
            _countLoop = StartCoroutine(CountLoop());
    }

    void StopPresenceLoops()
    {
        if (_heartbeatLoop != null)
        {
            StopCoroutine(_heartbeatLoop);
            _heartbeatLoop = null;
        }

        if (_countLoop != null)
        {
            StopCoroutine(_countLoop);
            _countLoop = null;
        }
    }

    IEnumerator HeartbeatLoop()
    {
        // FIRST: send an immediate heartbeat...
        yield return SendHeartbeatOnce();

        // ...then immediately fetch the count once so the UI is correct right away
        yield return RefreshCountOnce();

        var wait = new WaitForSeconds(heartbeatIntervalSeconds);

        // now just do periodic heartbeats
        while (true)
        {
            yield return wait;
            yield return SendHeartbeatOnce();
        }
    }

    IEnumerator SendHeartbeatOnce()
    {
        var payload = JsonUtility.ToJson(new HeartbeatPayload
        {
            sessionId = PresenceSession.Id
        });

        if (logRequests)
            Debug.Log($"[PresenceManager] Sending heartbeat: {payload}");

        using (var req = new UnityWebRequest(heartbeatUrl, UnityWebRequest.kHttpVerbPOST))
        {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(payload);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                if (logRequests)
                    Debug.LogWarning($"[PresenceManager] Heartbeat failed: {req.error}");

                // internet might still be online; service may be down or misconfigured
                TrySetCountInvalid(PresenceServiceState.ServiceError);
                yield break;
            }

            if (logRequests)
                Debug.Log($"[PresenceManager] Heartbeat ok. Response: {req.downloadHandler.text}");
        }
    }

    IEnumerator CountLoop()
    {
        var wait = new WaitForSeconds(countRefreshIntervalSeconds);

        // we already did an initial RefreshCountOnce() inside HeartbeatLoop,
        // so this loop just waits then refreshes on a cadence.
        while (true)
        {
            yield return wait;
            yield return RefreshCountOnce();
        }
    }

    public IEnumerator RefreshCountOnce()
    {
        if (string.IsNullOrWhiteSpace(countUrl))
            yield break;

        using (var req = UnityWebRequest.Get(countUrl))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                if (logRequests)
                    Debug.LogWarning($"[PresenceManager] Get count failed: {req.error}");

                TrySetCountInvalid(PresenceServiceState.ServiceError);
                yield break;
            }

            var json = req.downloadHandler.text;
            if (logRequests)
                Debug.Log($"[PresenceManager] Count response: {json}");

            CountResponse response;
            try
            {
                response = JsonUtility.FromJson<CountResponse>(json);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[PresenceManager] Failed to parse count JSON: {e.Message}");
                yield break;
            }

            int newCount = response.count;

            OnlineCountValid = true;
            SetServiceState(PresenceServiceState.Online);

            if (newCount != OnlineCount)
            {
                OnlineCount = newCount;
                OnOnlineCountChanged?.Invoke(OnlineCount);
            }
        }
    }

    [Serializable]
    private struct HeartbeatPayload
    {
        public string sessionId;
    }

    [Serializable]
    private struct CountResponse
    {
        public int count;
    }
}