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
    #endregion

    void Start()
    {
        FindObjectOfType<DeliveryWaypoint>().AddObjectivePoint(this);
    }
}
