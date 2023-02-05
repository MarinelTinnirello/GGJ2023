using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class DeliveryWaypoint : MonoBehaviour
{
    #region Variables
    [Header("Minimap")]
    public Camera camera;
    public RectTransform minimap;

    [Header("Prefabs")]
    public ObjectiveWaypoint objectivePrefab;
    public GameObject markerPrefab;
    List<ObjectiveWaypoint> waypoints;
    public GameObject player;
    public ObjectiveWaypoint ActiveInstance;

    [Header("Controllers")]
    [SerializeField]
    float PathHeightOffset = 1.25f;
    [SerializeField]
    float PathUpdateSpeed = 0.25f;
    [SerializeField]
    float SpawnHeightOffset = 1.5f;
    [SerializeField]
    float nearestDistance = 10000;
    ObjectiveWaypoint nearestObj;

    NavMeshTriangulation Triangulation;
    Coroutine DrawPathCoroutine;
    #endregion

    private void Awake()
    {
        waypoints = new List<ObjectiveWaypoint>();

        Triangulation = NavMesh.CalculateTriangulation();
    }

    void Start()
    {
        StartDrawingPath();
    }

    void Update()
    {
        ShowMarkerDistance();
    }

    public void StartDrawingPath()
    {
        //ActiveInstance = Instantiate(objectivePrefab,
        //    Triangulation.vertices[Random.Range(0, Triangulation.vertices.Length)] + Vector3.up * SpawnHeightOffset,
        //    Quaternion.Euler(90, 0, 0));
        ActiveInstance = waypoints[0];

        if (DrawPathCoroutine != null)
        {
            StopCoroutine(DrawPathCoroutine);
        }

        DrawPathCoroutine = StartCoroutine(DrawPathToObjective());
    }

    IEnumerator DrawPathToObjective()
    {
        WaitForSeconds Wait = new WaitForSeconds(PathUpdateSpeed);
        NavMeshPath path = new NavMeshPath();

        while (ActiveInstance != null)
        {
            if (NavMesh.CalculatePath(player.transform.position, ActiveInstance.transform.position, NavMesh.AllAreas, path))
            {
                ActiveInstance.path.positionCount = path.corners.Length;

                for (int i = 0; i < path.corners.Length; i++)
                {
                    ActiveInstance.path.SetPosition(i, path.corners[i] + Vector3.up * PathHeightOffset);
                }
            }
            else
            {
                Debug.LogError($"Unable to calculate a path on the NavMesh between {player.transform.position} and {ActiveInstance.transform.position}!");
            }

            yield return Wait;
        }
    }

    void CheckDistance()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            float distance = Vector3.Distance(ActiveInstance.transform.position, player.transform.position);
            if (distance < nearestDistance)
            {
                nearestObj = waypoints[i];
                nearestDistance = distance;
            }
        }

        ActiveInstance = nearestObj;

        if (Vector3.Distance(ActiveInstance.transform.position, player.transform.position) < ActiveInstance.metersAway)
        {
            RemoveObjectivePoint(ActiveInstance);
        }
    }

    public void AddObjectivePoint(ObjectiveWaypoint sender)
    {
        RectTransform rect = Instantiate(markerPrefab, minimap).GetComponent<RectTransform>();
        sender.rect = rect;
        LineRenderer path = Instantiate(sender.path, sender.transform).GetComponent<LineRenderer>();
        sender.path = path;
        waypoints.Add(sender);
    }

    public void RemoveObjectivePoint(ObjectiveWaypoint sender)
    {
        if (!waypoints.Exists(objective => objective == sender))
            return;

        ObjectiveWaypoint foundObj = waypoints.Find(objective => sender);
        Destroy(foundObj.rect.gameObject);
        Destroy(foundObj.path.gameObject);
        waypoints.Remove(foundObj);

        if (waypoints.Count != 0)
            ActiveInstance = waypoints[0];
    }

    void ShowMarkerDistance()
    {
        //foreach (ObjectiveWaypoint marker in waypoints)
        //{
        //    Vector3 offset = Vector3.ClampMagnitude(marker.transform.position - player.transform.position, camera.orthographicSize);
        //    offset = offset / camera.orthographicSize * (minimap.rect.width / 2);
        //    marker.rect.anchoredPosition = new Vector2(offset.x, offset.z);
        //    marker.rect.GetComponent<IndicatorMarker>().SetDistanceText(marker, player);
        //    //WaypointCamera(marker);
        //    CheckDistance();
        //}

        Vector3 offset = Vector3.ClampMagnitude(ActiveInstance.transform.position - player.transform.position, camera.orthographicSize);
        offset = offset / camera.orthographicSize * (minimap.rect.width / 2);
        ActiveInstance.rect.anchoredPosition = new Vector2(offset.x, offset.z);
        ActiveInstance.rect.GetComponent<IndicatorMarker>().SetDistanceText(ActiveInstance, player);
        //WaypointCamera(marker);
        CheckDistance();
    }

    void WaypointCamera(ObjectiveWaypoint marker)
    {
        Image indicator = marker.rect.GetComponent<Image>();
        float minX = indicator.GetPixelAdjustedRect().width / 2;
        float maxX = minimap.rect.width - minX;
        float minY = indicator.GetPixelAdjustedRect().height / 2;
        float maxY = minimap.rect.height - minY;

        Vector2 newPos = camera.ScreenToWorldPoint(marker.transform.position);

        // check if behind camera
        if (Vector3.Dot(marker.transform.position - transform.position, transform.forward) < 0)
        {
            // target if behind player
            if (newPos.x < minimap.rect.width / 2)
            {
                newPos.x = maxX;
            }
            else
            {
                newPos.x = minX;
            }
        }

        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        indicator.transform.position = newPos;
    }
}
