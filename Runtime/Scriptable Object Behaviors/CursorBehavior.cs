using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cursor Behavior_ New", menuName = "Behaviors/Application/Cursor Behavior")]
public class CursorBehavior : ScriptableObject
{
    public void SetCursorLockState(CursorLockMode cursorLockMode) => Cursor.lockState = cursorLockMode;
    public void SetCursorLockStateNone() => SetCursorLockState(CursorLockMode.None);
    public void SetCursorLockStateConfined() => SetCursorLockState(CursorLockMode.Confined);
    public void SetCursorLockStateLocked() => SetCursorLockState(CursorLockMode.Locked);

    public void ToggleCursorStateConfined() => SetCursorLockState(Cursor.lockState == CursorLockMode.Confined ? CursorLockMode.None : CursorLockMode.Confined);
    public void ToggleCursorStateLocked() => SetCursorLockState(Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked);
}