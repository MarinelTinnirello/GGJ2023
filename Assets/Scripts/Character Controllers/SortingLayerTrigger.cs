using UnityEngine;

#if UNITY_EDITOR
[SortingLayerNameList("onEnterSortingLayer", "onExitSortingLayer")]
#endif

[RequireComponent(typeof(Collider))]
public class SortingLayerTrigger : MonoBehaviour
{
    [HideInInspector]
    public string onEnterSortingLayer, onExitSortingLayer;

    public bool ingoreExitTrigger;

    private void OnTriggerEnter(Collider other)
    {
        CharacterAnimationController characterAnimationController = other.gameObject.GetComponent<CharacterAnimationController>();
        SetSpriteSortingLayer setSpriteSortingLayer = other.gameObject.GetComponent<SetSpriteSortingLayer>();

        if (characterAnimationController) characterAnimationController.UpdateSortingLayers(onEnterSortingLayer);
        if (setSpriteSortingLayer) setSpriteSortingLayer.UpdateSortingLayers(onEnterSortingLayer);
    }

    private void OnTriggerExit(Collider other)
    {
        if (ingoreExitTrigger) return;

        CharacterAnimationController characterAnimationController = other.gameObject.GetComponent<CharacterAnimationController>();
        SetSpriteSortingLayer setSpriteSortingLayer = other.gameObject.GetComponent<SetSpriteSortingLayer>();

        if (characterAnimationController) characterAnimationController.UpdateSortingLayers(onExitSortingLayer);
        if (setSpriteSortingLayer) setSpriteSortingLayer.UpdateSortingLayers(onExitSortingLayer);
    }
}