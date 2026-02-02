using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Clipboard
{
    public static void Copy(string text) => GUIUtility.systemCopyBuffer = text;
    public static string Paste() => GUIUtility.systemCopyBuffer;
}
