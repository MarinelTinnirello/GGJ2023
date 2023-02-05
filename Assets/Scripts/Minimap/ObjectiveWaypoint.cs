using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveWaypoint : MonoBehaviour
{
    #region Variables
    public RectTransform rect;
    public LineRenderer path;
    public int metersAway = 10;
    public int minimapSize = 16;
    #endregion

    void Start()
    {
        FindObjectOfType<DeliveryWaypoint>().AddObjectivePoint(this);
    }

    void LateUpdate()
    {
        //
    }

    public void WaypointCamera(Camera camera)
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, camera.transform.position.x - minimapSize, camera.transform.position.x + minimapSize),
            transform.position.y,
            Mathf.Clamp(transform.position.z, camera.transform.position.z - minimapSize, camera.transform.position.z + minimapSize));
    }
}
