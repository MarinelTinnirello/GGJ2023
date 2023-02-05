using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[SortingLayerNameList("defaultSortingLayer")]
#endif

[RequireComponent(typeof(SpriteRenderer))]
public class SetSpriteSortingLayer : MonoBehaviour
{
    [HideInInspector]
    public string defaultSortingLayer;

    private Component[] spriteRenderers;

    private void Awake()
    {
        spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        UpdateSortingLayers(defaultSortingLayer);
    }

    public void UpdateSortingLayers(string _targetLayer)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            spriteRenderer.sortingLayerName = _targetLayer;
    }
}