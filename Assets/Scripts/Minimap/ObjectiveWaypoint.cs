using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveWaypoint : MonoBehaviour
{
    #region Variables
    public RectTransform rect;
    public LineRenderer path;
    #endregion

    void Start()
    {
        FindObjectOfType<DeliveryWaypoint>().AddObjectivePoint(this);
    }
}
