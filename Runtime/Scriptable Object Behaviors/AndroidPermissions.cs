using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

[CreateAssetMenu(fileName = "Android Permissions_ New", menuName = "Behaviors/Application/Android Permissions")]
public class AndroidPermissions : ScriptableObject
{
    public void RequestMicrophone()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            var callbacks = new PermissionCallbacks();

            Permission.RequestUserPermission(Permission.Microphone, callbacks);
        }
#endif
    }
}
