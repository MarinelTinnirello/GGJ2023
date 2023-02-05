using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    #region Variables
    private Camera camera;

    [Header("Mouse")]
    public Texture texture;

    [Header("Spawned Obj")]
    [SerializeField]
    GameObject spawnObjPrefab;

    [Header("Controllers")]
    [SerializeField]
    GameObject regionToSpawnUnder;
    [SerializeField]
    float timeUntilDestroyed = 3.0f;
    [SerializeField]
    int amountToAllow = 3;
    #endregion

    void Start()
    {
        camera = Camera.main;   
    }

    void Update()
    {
        SpawnObjAtPos();
    }

    void SpawnObjAtPos()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //GameObject go = Instantiate(spawnObjPrefab, hit.point, Quaternion.identity);
                // allows us to spawn above the ground based on the prefab's Y
                GameObject go = Instantiate(spawnObjPrefab, 
                    new Vector3(hit.point.x, hit.point.y + spawnObjPrefab.transform.position.y, hit.point.z), 
                    Quaternion.identity);

                go.transform.parent = regionToSpawnUnder.transform;
                Destroy(go, timeUntilDestroyed);

                DestroyIfAboveLimit();
            }
        }
    }

    void DestroyIfAboveLimit()
    {
        if (regionToSpawnUnder.transform.childCount > amountToAllow)
        {
            Destroy(regionToSpawnUnder.transform.GetChild(0).gameObject);
        }
    }
}
