using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ApplicationVersion))]
public class ApplicationVersionEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.BeginHorizontal();
		{
			GUI.enabled = false;
			EditorGUILayout.TextField("Application Version", UnityEngine.Application.version);
			GUI.enabled = true;

			Rect versionRect = GUILayoutUtility.GetLastRect();
			if (Event.current.type == EventType.MouseDown && versionRect.Contains(Event.current.mousePosition))
			{
				// Open Player Settings
				Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
			}
		}
		EditorGUILayout.EndHorizontal();
	}
}
