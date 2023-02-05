using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleFullscreen : MonoBehaviour {

    public void OnMouseDown()
    {
        //Screen.fullScreen = !Screen.fullScreen;
    }

    public void ToggleFullscreenMode()
    {
        SetFullScreenMode(!Screen.fullScreen);
    }

    public void SetFullScreenMode(bool state)
    {
        Screen.fullScreen = state;
    }
}
