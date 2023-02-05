using System.Collections.Generic;
using UnityEngine;

public class SetLayerOrderByDistance : MonoBehaviour
{
    public Transform sortingIndexStartPoint;
    private int distanceFromMarker, prevDistanceFromMarker;

    private SpriteRenderer[] spriteRenderers;
    private Dictionary<SpriteRenderer, int> sortingLayerOrder;

    private void Start()
    {
        if (GameManager.Instance)
        {
            if (!sortingIndexStartPoint) sortingIndexStartPoint = GameManager.Instance.SortIndexStartPoint;
        }

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        sortingLayerOrder = new Dictionary<SpriteRenderer, int>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            sortingLayerOrder.Add(spriteRenderer, spriteRenderer.sortingOrder);
    }

    private void Update()
    {
        GetDistanceFromMarker();
    }

    private void GetDistanceFromMarker() {
        if (sortingIndexStartPoint)
        {
            distanceFromMarker = Mathf.RoundToInt(GameManager.Instance.SortIndexStartPoint.position.z - transform.position.z);

            if (prevDistanceFromMarker != distanceFromMarker)
            {
                prevDistanceFromMarker = distanceFromMarker;
                UpdateSpriteOrderInLayer(distanceFromMarker);
            }
        }
    }

    public void UpdateSpriteOrderInLayer(int _offset)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            spriteRenderer.sortingOrder = sortingLayerOrder[spriteRenderer] + _offset;
    }
}