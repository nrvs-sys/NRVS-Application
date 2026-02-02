using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PlayerPlatformUtility
{
    const string symbolStandalone = "PLAYERPLATFORM_STANDALONE";
    const string symbolOculus = "PLAYERPLATFORM_OCULUS";
    const string symbolSteam = "PLAYERPLATFORM_STEAM";

    public enum PlayerPlatform
    {
        None,
        Standalone,
        Oculus,
        Steam
    }


    static UnityEditor.Build.NamedBuildTarget CurrentTarget
    {
        get
        {
            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (group == BuildTargetGroup.Standalone &&
                EditorUserBuildSettings.standaloneBuildSubtarget == StandaloneBuildSubtarget.Server)
            {
                return UnityEditor.Build.NamedBuildTarget.Server;
            }
            return UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(group);
        }
    }

    static List<string> GetSymbols()
    {
        var raw = PlayerSettings.GetScriptingDefineSymbols(CurrentTarget);
        if (string.IsNullOrEmpty(raw))
            return new List<string>();
        var list = new List<string>(raw.Split(';'));
        list.RemoveAll(s => string.IsNullOrWhiteSpace(s));
        return list;
    }

    static void SetSymbols(List<string> symbols)
    {
        // Deduplicate while preserving order
        var seen = new HashSet<string>();
        var filtered = new List<string>();
        foreach (var s in symbols)
        {
            if (string.IsNullOrWhiteSpace(s)) continue;
            if (seen.Add(s)) filtered.Add(s);
        }
        PlayerSettings.SetScriptingDefineSymbols(CurrentTarget, string.Join(";", filtered));
    }

    public static string GetPlayerPlatformSymbol(PlayerPlatform platform)
    {
        switch (platform)
        {
            case PlayerPlatform.Standalone:
                return symbolStandalone;
            case PlayerPlatform.Oculus:
                return symbolOculus;
            case PlayerPlatform.Steam:
                return symbolSteam;
            default:
                return string.Empty;
        }
    }

    public static void SetPlayerPlatform(PlayerPlatform platform)
    {
        var platformSymbol = GetPlayerPlatformSymbol(platform);
        var symbols = GetSymbols();

        // Remove existing platform-specific symbols
        symbols.Remove(symbolStandalone);
        symbols.Remove(symbolOculus);
        symbols.Remove(symbolSteam);

        // Add selected platform symbol
        if (!string.IsNullOrEmpty(platformSymbol))
            symbols.Add(platformSymbol);

        SetSymbols(symbols);
    }

    public static PlayerPlatform GetPlayerPlatform()
    {
        var symbols = GetSymbols();

        if (symbols.Contains(symbolStandalone))
            return PlayerPlatform.Standalone;
        if (symbols.Contains(symbolOculus))
            return PlayerPlatform.Oculus;
        if (symbols.Contains(symbolSteam))
            return PlayerPlatform.Steam;
        return PlayerPlatform.None;
    }

    public static bool IsPlatformActive(PlayerPlatform platform) => platform == GetPlayerPlatform();

    #region Editor Menu Items

    const string menuRoot = "Utilities";
    const string playerPlatformPathStandalone = menuRoot + "/Set Player Platform/Standalone";
    const string playerPlatformPathOculus = menuRoot + "/Set Player Platform/Oculus";
    const string playerPlatformPathSteam = menuRoot + "/Set Player Platform/Steam";

    [MenuItem(playerPlatformPathStandalone, priority = 90)]
    public static void SetPlayerPlatformStandalone()
    {
        SetPlayerPlatform(PlayerPlatform.Standalone);
    }

    [MenuItem(playerPlatformPathStandalone, true)]
    public static bool SetPlayerPlatformStandaloneValidate()
    {
        Menu.SetChecked(playerPlatformPathStandalone, IsPlatformActive(PlayerPlatform.Standalone));
        return true;
    }

    [MenuItem(playerPlatformPathOculus, priority = 91)]
    public static void SetPlayerPlatformOculus()
    {
        SetPlayerPlatform(PlayerPlatform.Oculus);
    }

    [MenuItem(playerPlatformPathOculus, true)]
    public static bool SetPlayerPlatformOculusValidate()
    {
        Menu.SetChecked(playerPlatformPathOculus, IsPlatformActive(PlayerPlatform.Oculus));
        return true;
    }

    [MenuItem(playerPlatformPathSteam, priority = 92)]
    public static void SetPlayerPlatformSteam()
    {
        SetPlayerPlatform(PlayerPlatform.Steam);
    }

    [MenuItem(playerPlatformPathSteam, true)]
    public static bool SetPlayerPlatformSteamValidate()
    {
        Menu.SetChecked(playerPlatformPathSteam, IsPlatformActive(PlayerPlatform.Steam));
        return true;
    }

    #endregion
}
