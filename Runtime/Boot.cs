using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;


#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class Boot : MonoBehaviour
{
    const string vrArg = "-vr";

    public bool initializeXR
    {
        get;
        set;
    } = false;

    [Header("Settings")]

    [SerializeField]
    private BoolConstant initializeXRInEditor;

    [SerializeField]
    private bool useFlatControllerForMockHMD = false;

    [Header("References")]

    [SerializeField]
    ConditionBehavior startLoadingBootScenesConditionBehavior;

    [SerializeField]
    private SceneReference coreScene;

    [Space(10)]

    [SerializeField]
    private SceneReferenceValueList serverBootScenes;

    [SerializeField]
    private SceneReferenceValueList xrClientBootScenes;

    [SerializeField]
    private SceneReferenceValueList flatClientBootScenes;

    [Space(10)]

    [SerializeField]
    private GameObject bootXRRig;

    [SerializeField]
    private XROrigin bootXROrigin;

    [SerializeField]
    private GameObject xrSplash;

    [Header("Events")]

    public UnityEvent onInitializeServer;
    public UnityEvent onInitializeXR;
    public UnityEvent onInitializeXRFail;
    public UnityEvent onInitializeFlat;


    private bool xrInitialized = false;


    void Awake()
    {
        xrSplash.SetActive(false);
    }


    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        StartCoroutine(DoInitialize());
    }

    private IEnumerator DoInitialize()
    {
        yield return EnsureMicPermission();

        var bootScenes = serverBootScenes.List;

#if UNITY_SERVER
        bootScenes = serverBootScenes.List;

        ApplicationInfo.applicationMode = ApplicationInfo.ApplicationMode.Server;

        onInitializeServer?.Invoke();
#else

#if UNITY_EDITOR
        initializeXR = initializeXRInEditor.Value && ParrelSyncManager.type == ParrelSyncManager.ParrelInstanceType.Main;
#else
#if PLAYERPLATFORM_OCULUS
        initializeXR = true;
#else
        string[] args = Environment.GetCommandLineArgs();

		initializeXR = args.Any(a => a.Equals(vrArg, StringComparison.OrdinalIgnoreCase));
#endif
#endif

        if (initializeXR)
        {
            yield return InitializeXR();

            if (xrInitialized)
            {
                var deviceName = XRSettings.loadedDeviceName;

                Debug.Log("Loaded XR Device: " + deviceName);

                var isMockHMD = deviceName == "Mock HMD" || deviceName == "MockHMDDisplay";

                if (isMockHMD && useFlatControllerForMockHMD)
                {
                    bootScenes = flatClientBootScenes.List;

                    ApplicationInfo.applicationMode = ApplicationInfo.ApplicationMode.Flat;
                }
                else
                {
                    bootScenes = xrClientBootScenes.List;

                    ApplicationInfo.applicationMode = ApplicationInfo.ApplicationMode.XR;
                }
            }
            else
            {
                onInitializeXRFail?.Invoke();

                yield break;
            }

            // Wait until XR subsystems are running and head pose is valid
            yield return WaitForXROriginReady();

            // Enable the Splash Screen
            xrSplash?.SetActive(true);

            // Wait a moment to allow the splash screen to Fade In
            yield return new WaitForSeconds(0.5f);

            onInitializeXR?.Invoke();
        }
        else if (ParrelSyncManager.type == ParrelSyncManager.ParrelInstanceType.Server)
        {
            bootScenes = serverBootScenes.List;

            ApplicationInfo.applicationMode = ApplicationInfo.ApplicationMode.Server;

            onInitializeServer?.Invoke();
        }
        else
        {
            bootScenes = flatClientBootScenes.List;

            ApplicationInfo.applicationMode = ApplicationInfo.ApplicationMode.Flat;

            onInitializeFlat?.Invoke();
        }
#endif

        // If using FMOD:
        // Wait for the master FMOD bank to load before doing anything else
        // This is to prevent any errors from playing audio before the master bank is loaded,
        // which may happen when the application is split into multiple asset bundles
        // (e.g. the OBB file included with the Android build)
        while (startLoadingBootScenesConditionBehavior != null && !startLoadingBootScenesConditionBehavior.If())
            yield return null;

        // Load the core scene first
        if (coreScene != null && !string.IsNullOrEmpty(coreScene.ScenePath))
            SceneManager.LoadSceneAsync(coreScene, LoadSceneMode.Additive);

        // Wait for the core scene to be loaded
        if (coreScene != null && !string.IsNullOrEmpty(coreScene.ScenePath))
        {
            var core = SceneManager.GetSceneByPath(coreScene.ScenePath);
            while (!core.isLoaded)
                yield return null;
        }

        var asyncOps = new List<AsyncOperation>();

        foreach (var scene in bootScenes)
        {
            var op = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            op.allowSceneActivation = false;
            asyncOps.Add(op);
        }

        // Wait for all scenes to be ready to be activated
        foreach (var op in asyncOps)
        {
            while (op.progress < 0.9f)
                yield return null;
        }

        // Fade out the splash screen
        if (xrSplash != null && xrSplash.activeInHierarchy && xrSplash.TryGetComponent(out Animator xrSplashAnimator))
        {
            xrSplashAnimator.SetBool("Fade Out", true);
            yield return new WaitForSeconds(1.0f);
        }

        // Disable the boot camera and splash screen
        bootXRRig?.SetActive(false);
        xrSplash?.SetActive(false);

        // Allow recentering after the initial setup
        if (xrInitialized)
            OpenXRSettings.SetAllowRecentering(true);

        // Activate all scenes
        foreach (var op in asyncOps)
            op.allowSceneActivation = true;
    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR || !UNITY_ANDROID
        DeinitializeXR();
#endif
    }

    private IEnumerator InitializeXR()
    {
        xrInitialized = false;

        Debug.Log("Initializing XR...");

        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("XR Initialization Failed.");
        }
        else
        {
            Debug.Log("Active XR loader set: " + XRGeneralSettings.Instance.Manager.activeLoader.name);
            Debug.Log("XR Initialization Finished. Starting XR Subsystems...");

            //Try to start all subsystems and check if they were all successfully started (thus HMD prepared).
            bool loaderSuccess = XRGeneralSettings.Instance.Manager.activeLoader.Start();
            if (loaderSuccess)
            {
                xrInitialized = true;

                Debug.Log("All XR Subsystems Started!");
            }
            else
            {
                Debug.LogError("Starting XR Subsystems Failed.");
            }
        }
    }

    private void DeinitializeXR()
    {
        if (XRGeneralSettings.Instance?.Manager?.activeLoader != null)
        {
            Debug.Log("Deinitializing XR Loader...");

            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();

            Debug.Log("XR Loader deinitialized completely.");
        }

        xrInitialized = false;
    }


    public IEnumerator EnsureMicPermission()
    {
#if UNITY_ANDROID
        // Already granted? continue.
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
            yield break;

        bool done = false;
        bool granted = false;

        var callbacks = new PermissionCallbacks();
        callbacks.PermissionGranted += _ => { granted = true; done = true; };
        callbacks.PermissionDenied += _ => { granted = false; done = true; };

        Permission.RequestUserPermission(Permission.Microphone, callbacks);

        // Wait until user chooses
        while (!done)
            yield return null;

        if (!granted)
        {
            // Handle denial: show UI, disable voice features, or open Settings intent
            // e.g., show a panel explaining how to enable mic in Quest Settings.
            Debug.LogWarning("Microphone permission denied.");
        }

#else
        yield break;
#endif
    }

    // Waits for XR subsystems to be running and for head pose to be valid for a couple frames.
    private IEnumerator WaitForXROriginReady()
    {

        XRInputSubsystem input = null;
        XRDisplaySubsystem display = null;

        // Wait for subsystems + camera availability
        while (true)
        {
            input = GetRunningInputSubsystem();
            display = GetRunningDisplaySubsystem();

            if (input != null && input.running &&
                display != null && display.running &&
                bootXROrigin?.Camera != null)
            {
                Debug.Log("XR subsystems running and camera found.");
                break;
            }

            yield return null;
        }

        if (input == null || !input.running || display == null || !display.running || bootXROrigin?.Camera == null)
        {
            Debug.LogError("XR subsystems failed to start or camera not found.");
            yield break;
        } 

        // Ensure tracking origin is configured
        var supported = input.GetSupportedTrackingOriginModes();
        var targetMode = (supported & TrackingOriginModeFlags.Floor) != 0
            ? TrackingOriginModeFlags.Floor
            : TrackingOriginModeFlags.Device;
        input.TrySetTrackingOriginMode(targetMode);

        // Wait until the HMD reports a valid pose for at least 5 consecutive frames
        int goodFrames = 0;
        while (true)
        {
            var head = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            bool hasPos = head.TryGetFeatureValue(CommonUsages.devicePosition, out _);
            bool hasRot = head.TryGetFeatureValue(CommonUsages.deviceRotation, out _);

            if (hasPos && hasRot)
            {
                // check if the camera transform is valid is not 0,0,0 and not NaN
                var camPos = bootXROrigin.Camera.transform.localPosition;
                if (camPos.Approximately(Vector3.zero) || float.IsNaN(camPos.x) || float.IsNaN(camPos.y) || float.IsNaN(camPos.z))
                {
                    goodFrames = 0;
                    yield return null;
                    continue;
                }

                goodFrames++;
                if (goodFrames >= 50)
                {
                    Debug.Log("XR head pose is valid.");
                    break;
                }
            }
            else
            {
                goodFrames = 0;
            }

            yield return null;
        }

        // One extra frame to let XROrigin apply transforms
        yield return null;

        Debug.Log("XR Origin should be ready.");
    }

    private static XRInputSubsystem GetRunningInputSubsystem()
    {
        var list = new List<XRInputSubsystem>();
        SubsystemManager.GetSubsystems(list);
        for (int i = 0; i < list.Count; i++)
            if (list[i].running) return list[i];
        return list.Count > 0 ? list[0] : null;
    }

    private static XRDisplaySubsystem GetRunningDisplaySubsystem()
    {
        var list = new List<XRDisplaySubsystem>();
        SubsystemManager.GetSubsystems(list);
        for (int i = 0; i < list.Count; i++)
            if (list[i].running) return list[i];
        return list.Count > 0 ? list[0] : null;
    }
}
