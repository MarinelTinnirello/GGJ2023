using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class DeliveryWaypoint : MonoBehaviour
{
    #region Variables
    [Header("Minimap")]
    public Camera m_camera;
    public RectTransform minimap;

    [Header("Prefabs")]
    public ObjectiveWaypoint objectivePrefab;
    public GameObject markerPrefab;
    public List<ObjectiveWaypoint> waypoints;
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
    bool waypointsReady;
    [SerializeField]
    float indicatorBorderSize = 10f;
    [SerializeField]
    float metersAway = 15f;
    [SerializeField]
    Pathfinding m_Path = new Pathfinding();
    [SerializeField]
    Graph m_Graph;

    NavMeshTriangulation Triangulation;
    Coroutine DrawPathCoroutine;
    #endregion

    void Awake()
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

    public void AssignMinimapCamera(Camera _camera)
    {
        m_camera = _camera;
    }

    public void StartDrawingPath()
    {
        //ActiveInstance = Instantiate(objectivePrefab,
        //    Triangulation.vertices[Random.Range(0, Triangulation.vertices.Length)] + Vector3.up * SpawnHeightOffset,
        //    Quaternion.Euler(90, 0, 0));
        if (waypoints.Count < 1)
        {
            Debug.Log("nothing in the list");
            return;
        }

        ActiveInstance = waypoints[0];

        //if (DrawPathCoroutine != null)
        //{
        //    StopCoroutine(DrawPathCoroutine);
        //}

        //DrawPathCoroutine = StartCoroutine(DrawPathToObjective());

        waypointsReady = true;
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

    public void AddObjectivePoint(ObjectiveWaypoint sender)
    {
        RectTransform rect = Instantiate(markerPrefab, minimap).GetComponent<RectTransform>();
        sender.rect = rect;
        LineRenderer path = Instantiate(sender.path, sender.transform).GetComponent<LineRenderer>();
        sender.path = path;
        waypoints.Add(sender);
        sender.rect.gameObject.SetActive(false);
    }

    public void RemoveObjectivePoint(ObjectiveWaypoint sender)
    {
        if (!waypoints.Exists(objective => objective == sender))
            return;

        ObjectiveWaypoint foundObj = waypoints.Find(objective => sender);
        //Destroy(foundObj.rect.gameObject);
        //Destroy(foundObj.path.gameObject);
        foundObj.rect.gameObject.SetActive(false);
        foundObj.path.gameObject.SetActive(false);
        waypoints.Remove(foundObj);

        if (waypoints.Count != 0)
            ActiveInstance = waypoints[0];
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
        if (ActiveInstance.rect)
            ActiveInstance.rect.gameObject.SetActive(true);

        if (Vector3.Distance(ActiveInstance.transform.position, player.transform.position) < metersAway)
        {
            RemoveObjectivePoint(ActiveInstance);
        }
    }

    void ShowMarkerDistance()
    {
        if (!waypointsReady || !ActiveInstance.rect)
            return;

        //foreach (ObjectiveWaypoint marker in waypoints)
        //{
        //    Vector3 offset = Vector3.ClampMagnitude(marker.transform.position - player.transform.position, camera.orthographicSize);
        //    offset = offset / camera.orthographicSize * (minimap.rect.width / 2);
        //    marker.rect.anchoredPosition = new Vector2(offset.x, offset.z);
        //    marker.rect.GetComponent<IndicatorMarker>().SetDistanceText(marker, player);
        //    //WaypointCamera(marker);
        //    CheckDistance();
        //}

        // if we want multiple, just do a foreach with the CheckIfOnScreen()

        CheckIfOnScreen();
        CheckDistance();

        m_Path = m_Graph.GetShortestPath(player.gameObject.GetComponent<Node>(), ActiveInstance.gameObject.GetComponent<Node>());
    }

    void RotateIndicator()
    {
        Vector3 toPos = ActiveInstance.transform.position;
        Vector3 fromPos = m_camera.transform.position;
        fromPos.z = 0f;
        Vector3 dir = (toPos - fromPos).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        ActiveInstance.rect.eulerAngles = new Vector3(0, 0, angle);
    }

    void CheckIfOnScreen()
    {
        Vector3 targetScreenPos = m_camera.WorldToScreenPoint(ActiveInstance.transform.position);
        bool isOnScreen = targetScreenPos.x <= indicatorBorderSize ||
            targetScreenPos.x >= Screen.width ||
            targetScreenPos.y <= indicatorBorderSize ||
            targetScreenPos.y >= Screen.height;
        if (isOnScreen)
        {
            RotateIndicator();

            ActiveInstance.rect.GetComponent<Image>().sprite = ActiveInstance.rect.GetComponent<IndicatorMarker>().arrow;

            Vector3 clampedScreenPos = targetScreenPos;
            clampedScreenPos.x = Mathf.Clamp(clampedScreenPos.x, indicatorBorderSize, Screen.width - indicatorBorderSize);
            clampedScreenPos.y = Mathf.Clamp(clampedScreenPos.y, indicatorBorderSize, Screen.height - indicatorBorderSize);

            Vector3 indicatorPos = m_camera.ScreenToWorldPoint(clampedScreenPos);
            ActiveInstance.rect.position = indicatorPos;
            ActiveInstance.rect.localPosition = new Vector3(indicatorPos.x, indicatorPos.y, 0f);
        }
        else
        {
            ActiveInstance.rect.GetComponent<Image>().sprite = ActiveInstance.rect.GetComponent<IndicatorMarker>().spot;

            Vector3 indicatorPos = m_camera.ScreenToWorldPoint(targetScreenPos);
            ActiveInstance.rect.position = indicatorPos;
            ActiveInstance.rect.localPosition = new Vector3(indicatorPos.x, indicatorPos.y, 0f);

            ActiveInstance.rect.eulerAngles = Vector3.zero;
        }
    }
}
