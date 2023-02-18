using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct MarkerSettings
{
    public Sprite icon;
    public string label;
    public Color32 color;
}

public class MinimapMarker : MonoBehaviour
{
    public MarkerSettings markerSettings;

    private void Start()
    {
        MinimapController.Instance?.RegisterMarker(this);
    }

    void Update()
    {
        
    }
}
