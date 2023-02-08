using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorMarker : MonoBehaviour
{
    #region Variables
    [SerializeField]
    Text distance;
    public Sprite arrow;
    public Sprite spot;
    #endregion

    public void SetDistanceText(ObjectiveWaypoint sender, GameObject player)
    {
        distance.text = ((int)Vector3.Distance(sender.transform.position, player.transform.position)).ToString() + "m";
    }
}
