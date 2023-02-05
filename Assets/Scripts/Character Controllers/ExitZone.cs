using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ExitZone : MonoBehaviour
{
    public LayerMask targetLayers;
    private bool exitCalled;

    private void OnTriggerStay(Collider other)
    {
        if (exitCalled) return;

        EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();

        // want to check if it's the main enemy dragging, and if not let them exit off stage
        /*if (enemyController)
        {
            if (!enemyController.isDragging)
            {
                enemyController.RemoveFromDJ();
                enemyController.DestroyThisObject();
            }
        }*/
        
        //if ((targetLayers & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        if ((targetLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            GameManager.Instance?.OnPlayerFail();
            exitCalled = true;
        }
    }
}