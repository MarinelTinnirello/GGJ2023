using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    #region Variables
    public CharacterTrigger player;
    public MinimapPlayerIndicator playerIndicatorPrefab;
    public DeliveryWaypoint deliveryWaypoint;
    #endregion

    void Start()
    {
        InstantiateObjectAtCenterPoint(playerIndicatorPrefab);
    }

    void LateUpdate()
    {
        Vector3 newPos = player.transform.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
    }

    public void InstantiateObjectAtFixedPoint(MinimapPlayerIndicator prefab, Vector3 location)
    {
        if (prefab == null) return;

        InstantiatePrefab(prefab, location, player.transform);
    }

    public void InstantiateObjectAtCenterPoint(MinimapPlayerIndicator prefab)
    {
        if (prefab == null) return;

        InstantiatePrefab(prefab, player.centerPoint ? player.centerPoint.position : Vector3.zero, player.transform);
    }

    public GameObject InstantiatePrefab(MinimapPlayerIndicator prefab, Vector3 position = default, Transform parent = null, float destroyTime = 4f, Vector3 _forward = default)
    {
        if (prefab == null)
            return null;

        MinimapPlayerIndicator obj = Instantiate(prefab);

        obj.transform.parent = parent;
        obj.transform.position = position;
        obj.transform.localRotation = prefab.transform.localRotation;

        if (_forward != Vector3.zero)
        {
            obj.transform.forward = _forward;
        }

        obj.gameObject.SetActive(true);

        deliveryWaypoint?.AssignMinimapCamera(obj.m_camera);
        obj.AssignTarget(player.transform);

        if (destroyTime > 0f)
        {
            Destroy(obj, destroyTime);
        }

        return obj.gameObject;
    }
}
