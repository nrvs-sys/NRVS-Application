using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEditor;
using UnityEngine;

public static class DemoModeSceneStrippingUtility
{
    const string STRIPPED_SCENE_LIST_PATH = "Assets/_Project/Application/SceneReferenceValueList_ Stripped for Demo Build.asset";
    const string menuRoot = "Utilities";
    const string menuPath = menuRoot + "/Demo Mode Utilities";
    const int menuPriority = 96; // sits just under the Demo Mode toggle (95)

    /// <summary>
    /// Strips scenes from the build that are listed in the SceneReferenceValueList at STRIPPED_SCENE_LIST_PATH.
    /// </summary>
    [MenuItem(menuPath + "/Strip Scene List for Demo", priority = menuPriority)]
    public static void StripScenesForDemoBuild()
    {
        var originalScenes = EditorBuildSettings.scenes;

        var scenesToStrip = AssetDatabase.LoadAssetAtPath<SceneReferenceValueList>(STRIPPED_SCENE_LIST_PATH);

        if (scenesToStrip != null)
        {
            List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>();
            HashSet<string> strippedScenePaths = new HashSet<string>();
            foreach (var sceneRef in scenesToStrip.List)
            {
                if (sceneRef != null && !string.IsNullOrEmpty(sceneRef.ScenePath))
                {
                    strippedScenePaths.Add(sceneRef.ScenePath);
                }
            }

            foreach (var scene in originalScenes)
            {
                if (!strippedScenePaths.Contains(scene.path))
                {
                    newScenes.Add(scene);
                }
                else
                {
                    Debug.Log($"Demo Mode Scene Stripping Utility: Stripping scene {scene.path} from build.");
                }
            }

            EditorBuildSettings.scenes = newScenes.ToArray();
        }
    }

    /// <summary>
    /// Restores the stripped scenes to the build settings.
    /// </summary>
    [MenuItem(menuPath + "/Restore Stripped Scene List", priority = menuPriority + 1)]
    public static void RestoreStrippedSceneList()
    {
        var originalScenes = EditorBuildSettings.scenes;

        var scenesToStrip = AssetDatabase.LoadAssetAtPath<SceneReferenceValueList>(STRIPPED_SCENE_LIST_PATH);

        if (scenesToStrip != null)
        {
            List<EditorBuildSettingsScene> restoredScenes = new List<EditorBuildSettingsScene>(originalScenes);
            HashSet<string> existingScenePaths = new HashSet<string>();
            foreach (var scene in originalScenes)
            {
                existingScenePaths.Add(scene.path);
            }
            foreach (var sceneRef in scenesToStrip.List)
            {
                if (sceneRef != null && !string.IsNullOrEmpty(sceneRef.ScenePath) && !existingScenePaths.Contains(sceneRef.ScenePath))
                {
                    var newScene = new EditorBuildSettingsScene(sceneRef.ScenePath, true);
                    restoredScenes.Add(newScene);
                    Debug.Log($"Demo Mode Scene Stripping Utility: Restoring scene {sceneRef.ScenePath} to build.");
                }
            }
            EditorBuildSettings.scenes = restoredScenes.ToArray();
        }
    }
}
