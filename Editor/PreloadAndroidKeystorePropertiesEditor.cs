using UnityEditor;
using UnityEngine;
using System.IO;

public class PreloadAndroidKeystorePropertiesEditor : EditorWindow
{
    private string keystoreName = "";
    private string keystorePass = "";
    private string keyaliasName = "";
    private string keyaliasPass = "";
    private string keystoreFilePath = "";

    private const string KeystoreFilePathKey = "KeystoreFilePath";

    [MenuItem("Tools/Edit Preloaded Android Keystore Properties")]
    public static void ShowWindow()
    {
        var window = GetWindow<PreloadAndroidKeystorePropertiesEditor>("Edit Preloaded Android Keystore Properties");
        window.LoadExistingKeystoreProperties();
    }

    private void OnEnable()
    {
        keystoreFilePath = EditorPrefs.GetString(KeystoreFilePathKey, Path.Combine(UnityEngine.Application.dataPath, PreloadAndroidKeystore.keystorePropertiesFilePath));
    }

    private void OnGUI()
    {
        GUILayout.Label("Keystore Properties", EditorStyles.boldLabel);

        keystoreName = EditorGUILayout.TextField("Keystore Name", keystoreName);
        keystorePass = EditorGUILayout.PasswordField("Keystore Password", keystorePass);
        keyaliasName = EditorGUILayout.TextField("Key Alias Name", keyaliasName);
        keyaliasPass = EditorGUILayout.PasswordField("Key Alias Password", keyaliasPass);

        GUILayout.Space(10);

        GUILayout.Label("Keystore Properties File Path", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        keystoreFilePath = EditorGUILayout.TextField(keystoreFilePath);
        if (GUILayout.Button("Browse", GUILayout.Width(75)))
        {
            string selectedPath = EditorUtility.SaveFilePanel("Select Keystore Properties File", UnityEngine.Application.dataPath, "keystore_properties.json", "json");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                keystoreFilePath = selectedPath;
                EditorPrefs.SetString(KeystoreFilePathKey, keystoreFilePath);
                LoadExistingKeystoreProperties(); // Load properties if the file exists
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create/Edit keystore_properties.json"))
        {
            CreateKeystorePropertiesFile();
        }

        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
        GUILayout.EndHorizontal();
    }

    private void LoadExistingKeystoreProperties()
    {
        if (File.Exists(keystoreFilePath))
        {
            string jsonContent = File.ReadAllText(keystoreFilePath);
            PreloadAndroidKeystore.KeystoreProperties keystoreProperties = JsonUtility.FromJson<PreloadAndroidKeystore.KeystoreProperties>(jsonContent);

            keystoreName = keystoreProperties.keystoreName;
            keystorePass = EncryptionUtility.Decrypt(keystoreProperties.keystorePass);
            keyaliasName = keystoreProperties.keyaliasName;
            keyaliasPass = EncryptionUtility.Decrypt(keystoreProperties.keyaliasPass);

            Debug.Log("Loaded existing keystore properties from: " + keystoreFilePath);
        }
        else
        {
            Debug.Log("No existing keystore properties file found at: " + keystoreFilePath);
        }
    }

    private void CreateKeystorePropertiesFile()
    {
        PreloadAndroidKeystore.KeystoreProperties keystoreProperties = new PreloadAndroidKeystore.KeystoreProperties
        {
            keystoreName = keystoreName,
            keystorePass = EncryptionUtility.Encrypt(keystorePass),
            keyaliasName = keyaliasName,
            keyaliasPass = EncryptionUtility.Encrypt(keyaliasPass)
        };

        string jsonContent = JsonUtility.ToJson(keystoreProperties, true);

        File.WriteAllText(keystoreFilePath, jsonContent);
        AssetDatabase.Refresh();

        Debug.Log("Keystore properties file created at: " + keystoreFilePath);
    }
}
