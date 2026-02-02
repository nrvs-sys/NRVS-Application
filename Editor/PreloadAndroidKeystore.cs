using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class PreloadAndroidKeystore
{
    public const string keystorePropertiesFilePath = "keystore_properties.json";
    public const string KeystoreFilePathKey = "KeystoreFilePath";

    static PreloadAndroidKeystore()
    {
#if UNITY_ANDROID
        string jsonFilePath = EditorPrefs.GetString(KeystoreFilePathKey, Path.Combine(Application.dataPath, keystorePropertiesFilePath));

        if (File.Exists(jsonFilePath))
        {
            Debug.Log("Preload Android Keystore: JSON file found: " + jsonFilePath);

            string jsonContent = File.ReadAllText(jsonFilePath);
            KeystoreProperties keystoreProperties = JsonUtility.FromJson<KeystoreProperties>(jsonContent);

            PlayerSettings.Android.keystoreName = keystoreProperties.keystoreName;
            PlayerSettings.Android.keystorePass = EncryptionUtility.Decrypt(keystoreProperties.keystorePass);
            PlayerSettings.Android.keyaliasName = keystoreProperties.keyaliasName;
            PlayerSettings.Android.keyaliasPass = EncryptionUtility.Decrypt(keystoreProperties.keyaliasPass);

            Debug.Log("Preload Android Keystore: Keystore properties set.");
        }
        else
        {
            Debug.Log("Preload Android Keystore: JSON file not found: " + jsonFilePath);
        }
#endif
    }

    [System.Serializable]
    public class KeystoreProperties
    {
        public string keystoreName;
        public string keystorePass;
        public string keyaliasName;
        public string keyaliasPass;
    }
}
