using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPref_ New", menuName = "Behaviors/Application/PlayerPref")]
public class PlayerPrefBehavior : ScriptableObject
{
    [SerializeField] 
    string key;


    [Button]
    void LogString() => Debug.Log($"PlayerPrefs Key: {key}, String Value: {GetString()}");
    [Button]
    void LogBool() => Debug.Log($"PlayerPrefs Key: {key}, Bool Value: {GetBool()}");
    [Button]
    void LogInt() => Debug.Log($"PlayerPrefs Key: {key}, Int Value: {GetInt()}");
    [Button]
    void LogFloat() => Debug.Log($"PlayerPrefs Key: {key}, Float Value: {GetFloat()}");

    public void SetString(string value) => PlayerPrefs.SetString(key, value);
    public void SetBool(bool value) => PlayerPrefs.SetString(key, value.ToString());
    public void SetInt(int value) => PlayerPrefs.SetInt(key, value);
    public void SetFloat(float value) => PlayerPrefs.SetFloat(key, value);

    public string GetString() => PlayerPrefs.GetString(key);
    public bool GetBool() => bool.Parse(PlayerPrefs.GetString(key));
    public int GetInt() => PlayerPrefs.GetInt(key);
    public float GetFloat() => PlayerPrefs.GetFloat(key);

    public void Delete() => PlayerPrefs.DeleteKey(key);

    public void DeleteAll() => PlayerPrefs.DeleteAll();

    public bool HasKey() => PlayerPrefs.HasKey(key);

    [Button("Delete PlayerPref")]
    void DeleteButton()
    {
#if UNITY_EDITOR

        if (!UnityEditor.EditorUtility.DisplayDialog(
            $"Delete PlayerPref at Key: {key}?",
            $"Are you sure you want to delete the PlayerPref at Key: {key}?",
            "Confirm", "Cancel"))
            return;

#endif

        Delete();
    }

    [Button("Delete All PlayerPrefs")]
    void DeleteAllButton()
    {
#if UNITY_EDITOR

        if (!UnityEditor.EditorUtility.DisplayDialog(
            $"Delete all PlayerPrefs?",
            $"Are you sure you want to delete all PlayerPrefs?",
            "Confirm", "Cancel"))
            return;

#endif

        DeleteAll();
    }
}
