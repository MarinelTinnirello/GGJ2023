using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[SortingLayerNameList("defaultSortingLayer")]
#endif
public class ItemContainer : MonoBehaviour
{
    [Header("Item Settings")]
    public Item[] itemsToSpawn;
    public LayerMask collisionLayers;

    [Header("Display Settings")]
    public SpriteRenderer[] staticSpriteRenderers;
    public GameObject onDestroyPrefab;
    public Transform itemSpawnPoint;
    public Transform centerPoint;
    [Space]
    public bool destroyOnHit = true;

    [HideInInspector]
    public string defaultSortingLayer;
    private Component[] spriteRenderers;
    public virtual void Start()
    {
        spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        UpdateSortingLayers(defaultSortingLayer);
    }

    public virtual void Update()
    {
        
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & collisionLayers) != 0)
        {
            OnContainerHit(0f);
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.collider.gameObject.layer) & collisionLayers) != 0)
        {
            OnContainerHit(0f);
        }
    }

    public virtual void OnContainerHit(float destroyTime = 1f)
    {
        AddEffect(onDestroyPrefab, false);
        //characterSoundEffects?.PlayRandomSoundClip(characterSoundEffects.knockOut, 0f);

        InstantiateItem(itemsToSpawn[0].gameObject);

        if (destroyOnHit) StartCoroutine(DestroyAfterDelay(destroyTime));
    }

    public IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        DestroyThisObject();
    }

    public virtual void DestroyThisObject()
    {
        Destroy(gameObject);
    }

    public GameObject AddEffect(GameObject _prefab, bool _makeChildObject = true, float _destroyTime = 4f, Vector3 _offset = default, Vector3 _forward = default)
    {
        if (_prefab == null)
            return null;

        Vector3 _pos = transform.position;
        Transform _parent = null;

        if (centerPoint)
        {
            _pos = centerPoint.position;
            if (_makeChildObject) _parent = centerPoint;
        }
        else if (_makeChildObject)
        {
            _parent = transform;
        }

        GameObject _obj = InstantiateEffect(_prefab, _pos + _offset, _parent, _destroyTime, _forward);

        return _obj;
    }

    public virtual GameObject InstantiateItem(GameObject _prefab, bool _makeChildObject = false, float _destroyTime = 12f, Vector3 _offset = default, Vector3 _forward = default)
    {
        //InstantiateObjectAtCenterPoint(itemsToSpawn[0].gameObject);//make random based on length

        if (_prefab == null)
            return null;

        Vector3 _pos = transform.position;
        Transform _parent = null;

        if (itemSpawnPoint)
        {
            _pos = centerPoint.position;
            if (_makeChildObject) _parent = centerPoint;
        }
        else if (_makeChildObject)
        {
            _parent = transform;
        }

        GameObject _obj = InstantiateEffect(_prefab, _pos + _offset, _parent, _destroyTime, _forward);

        return _obj;
    }

    public void InstantiateObjectAtCenterPoint(GameObject prefab)
    {
        if (prefab == null) return;

        InstantiateEffect(prefab, centerPoint ? centerPoint.position : Vector3.zero);
    }
    public GameObject InstantiateEffect(GameObject prefab, Vector3 position = default, Transform parent = null, float destroyTime = 4f, Vector3 _forward = default)
    {
        if (prefab == null)
            return null;

        GameObject obj = Instantiate(prefab);

        obj.transform.parent = parent;
        obj.transform.position = position;
        obj.transform.localRotation = prefab.transform.localRotation;

        if (_forward != Vector3.zero)
        {
            obj.transform.forward = _forward;
        }

        obj.SetActive(true);

        if (destroyTime > 0f)
        {
            Destroy(obj, destroyTime);
        }

        return obj;
    }

    public virtual void UpdateSortingLayers(string targetLayer)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            spriteRenderer.sortingLayerName = targetLayer;
    }
}