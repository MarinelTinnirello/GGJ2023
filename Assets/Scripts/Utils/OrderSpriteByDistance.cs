using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OrderSpriteByDistance : MonoBehaviour
{
    private Dictionary<SpriteRenderer, int> sortingLayerOrder;
    private Component[] spriteRenderers;

    private bool useShortingMarker;
    private int distanceFromMarker;
    private int prevDistanceFromMarker = -1;

    private bool isReady;

    private void Start()
    {
        CreateSpriteAtlas();

        if (GameManager.Instance)
            if (GameManager.Instance.SortIndexStartPoint) useShortingMarker = true;

        isReady = true;
    }
    
    private void Update()
    {
        if (useShortingMarker && isReady)
        {
            distanceFromMarker = Mathf.RoundToInt(GameManager.Instance.SortIndexStartPoint.position.z - transform.position.z);

            if (prevDistanceFromMarker != distanceFromMarker)
            {
                prevDistanceFromMarker = distanceFromMarker;
                UpdateSpriteOrderInLayer(distanceFromMarker);
            }
        }
    }

    private void CreateSpriteAtlas()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        sortingLayerOrder = new Dictionary<SpriteRenderer, int>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            sortingLayerOrder.Add(spriteRenderer, spriteRenderer.sortingOrder);
    }
    
    public void UpdateSpriteOrderInLayer(int _offset)
    {
#if UNITY_EDITOR
        if (Application.isEditor && !EditorApplication.isPlaying) return;
#endif

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            spriteRenderer.sortingOrder = sortingLayerOrder[spriteRenderer] + _offset;
    }
}
