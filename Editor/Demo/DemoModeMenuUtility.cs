using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DemoModeMenuUtility
{
    const string symbolDemoMode = "DEMO_MODE";

    public static bool IsDemoModeActive() => GetSymbols().Contains(symbolDemoMode);

    public static void SetDemoMode(bool active)
    {
        var symbols = GetSymbols();
        if (active)
        {
            if (!symbols.Contains(symbolDemoMode)) symbols.Add(symbolDemoMode);
        }
        else
        {
            symbols.Remove(symbolDemoMode);
        }
        SetSymbols(symbols);
    }

    #region helpers

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
        return new List<string>(string.IsNullOrEmpty(raw)
            ? System.Array.Empty<string>()
            : raw.Split(';'));
    }

    static void SetSymbols(List<string> symbols) =>
        PlayerSettings.SetScriptingDefineSymbols(CurrentTarget, string.Join(";", symbols));

    #endregion

    #region menu item

    const string menuRoot = "Utilities";
    const string menuPathToggle = menuRoot + "/Demo Mode Utilities";

    const int menuPriority = 95;             // sits just under the platform items (90–92)

    [MenuItem(menuPathToggle + "/Toggle Demo Mode", priority = menuPriority)]
    static void ToggleDemoMode() => SetDemoMode(!IsDemoModeActive());

    [MenuItem(menuPathToggle + "/Toggle Demo Mode", true)]
    static bool ToggleDemoModeValidate()
    {
        Menu.SetChecked(menuPathToggle + "/Toggle Demo Mode", IsDemoModeActive());
        return true;
    }

    #endregion
}