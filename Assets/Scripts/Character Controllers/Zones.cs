using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Zones : MonoBehaviour
{
    public EnviromentAreas currentZone;

    private void OnTriggerEnter(Collider other)
    {
        AIController aiController = other.gameObject.GetComponent<AIController>();

        aiController?.UpdateCurrentArea(currentZone);
    }
}