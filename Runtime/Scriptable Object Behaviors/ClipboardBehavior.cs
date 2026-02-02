using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Clipboard_ New", menuName = "Behaviors/Application/Clipboard", order = 1)]
public class ClipboardBehavior : ScriptableObject
{
    [SerializeField]
    string textToCopy;

    public void Copy()
    {
        Clipboard.Copy(textToCopy);
    }

    public void Copy(string text)
    {
        Clipboard.Copy(text);
    }

    public string Paste()
    {
        return Clipboard.Paste();
    }
}
