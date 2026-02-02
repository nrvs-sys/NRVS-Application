using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformOnly : MonoBehaviour
{
    public ApplicationInfo.Platform platform;

    private void OnEnable()
    {
        if (ApplicationInfo.platform != platform)
            gameObject.SetActive(false);
    }
}
