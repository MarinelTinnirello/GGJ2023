using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSettings : MonoBehaviour
{
    public CursorLockMode cursorLockMode;
    public bool cursorVisible = true;

    void Start()
    {
        CursorActive(cursorVisible, cursorLockMode);
    }

    public void CursorActive(bool state, CursorLockMode setCursorLockMode = CursorLockMode.Confined)
    {
        Cursor.visible = state;
        Cursor.lockState = setCursorLockMode;
    }
}
