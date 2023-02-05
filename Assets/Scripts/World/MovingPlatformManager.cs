using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformManager : MonoBehaviour
{
    public GameObject[] objectPool;
    [Space]
    public Transform startPos;
    public Transform endPos;
    private Vector3 startAtPos;
    public int maxActiveObjects = 2;
    [Space]
    public ActiveAxis moveOnAxis = ActiveAxis.X;
    public Vector3 movement;

    private int currentActiveObjects;
    private List<GameObject> objectList;
    private List<int> availableObjectList;

    //private Dictionary<GameObject, bool> objectList = new Dictionary<GameObject, bool>();

    private void Start()
    {
        objectList = new List<GameObject>();

        startAtPos = startPos.localPosition;

        for (int i = 0; i < objectPool.Length; i++)
        {
            GameObject currentObj = objectPool[i];
            MovingPlatform currentMovingPlatform = currentObj.GetComponent<MovingPlatform>();

            currentMovingPlatform?.SetPlatform(i, this, endPos.localPosition, movement, moveOnAxis);

            objectList.Add(currentObj);
            
            if (currentObj.activeInHierarchy) currentActiveObjects++;
        }

        CheckForActiveObject();
    }

    private void Update()
    {
        
    }

    private void CheckForActiveObject()
    {
        if (currentActiveObjects < maxActiveObjects)
        {
            availableObjectList = new List<int>();

            for (int i = 0; i < objectPool.Length; i++)
            {
                GameObject currentObj = objectPool[i];

                if (!currentObj.activeInHierarchy && currentActiveObjects < maxActiveObjects)
                {
                    availableObjectList.Add(i);
                }
            }

            while (currentActiveObjects < maxActiveObjects)
            {
                GameObject randomObj = objectPool[ availableObjectList[Random.Range(0, availableObjectList.Count)] ];

                if (!randomObj.activeInHierarchy)
                {
                    randomObj.transform.localPosition = startAtPos;
                    randomObj.SetActive(true);
                    currentActiveObjects++;
                }
            }
        }
    }

    public void OnObjectRemoved(int platformID)
    {
        objectPool[platformID].SetActive(false);
        currentActiveObjects--;

        CheckForActiveObject();

        /*int count = 0;

        foreach (Transform t in gameObject.GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject.activeInHierarchy) currentActiveObjects++;
        }*/
    }
}
