using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapMarkerIcon : MonoBehaviour
{
    public MinimapMarker marker;
    public MarkerSettings markerSettings;

    public Image icon;

    private Camera targetCamera;

    private void Awake()
    {
        if (!icon) icon = GetComponent<Image>();
    }

    private void Update()
    {
        if (!marker || !targetCamera) return;

        Vector3 screenPos = targetCamera.WorldToScreenPoint(marker.transform.position);
        transform.localPosition = new Vector3(screenPos.x, screenPos.y, 0f);

        /*
                    // convert screen coords
                    Vector2 adjustedPosition = mCamera.WorldToScreenPoint(transform.position);
 
                    adjustedPosition.x *= mCanvas.rect.width / (float)mCamera.pixelWidth;
                    adjustedPosition.y *= mCanvas.rect.height / (float)mCamera.pixelHeight;
 
                    // set it
                    mAnchorInUi.anchoredPosition = adjustedPosition - mCanvas.sizeDelta / 2f;
        */
    }

    public void AssignMarker(MinimapMarker _marker, Camera _camera)
    {
        marker = _marker;
        markerSettings = _marker.markerSettings;
        targetCamera = _camera;

        if (icon && markerSettings.icon)
        {
            icon.sprite = markerSettings.icon;
        }
    }
}
