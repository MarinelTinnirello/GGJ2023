using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //public CharacterType interactWithCharacter;
    [Header("Item Settings")]
    public ItemTypes itemType;
    public LayerMask interactWithLayerTypes;
    public bool destroyOnActivate = true;

    [Header("Display Settings")]
    public SpriteRenderer[] staticSpriteRenderers;
    public GameObject onDestroyPrefab;
    public Transform centerPoint;
    public virtual void Start()
    {
        
    }

    public virtual void Update()
    {
        
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        CharacterTrigger currentCharacter = other.gameObject.GetComponent<CharacterTrigger>();

        if (((1 << other.gameObject.layer) & interactWithLayerTypes) != 0)
        {
            OnItemActivate(currentCharacter, 0f);
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        CharacterTrigger currentCharacter = collision.collider.gameObject.GetComponent<CharacterTrigger>();

        if (((1 << collision.collider.gameObject.layer) & interactWithLayerTypes) != 0)
        {
            OnItemActivate(currentCharacter, 0f);
        }
    }

    public virtual void OnItemActivate(CharacterTrigger targetCharacter, float destroyTime = 1f)
    {
        AddEffect(onDestroyPrefab, false);
        //characterSoundEffects?.PlayRandomSoundClip(characterSoundEffects.knockOut, 0f);
        if (destroyOnActivate) StartCoroutine(DestroyAfterDelay(destroyTime));

        if (targetCharacter == null) return;

        switch (itemType)
        {
            case ItemTypes.Health:
                targetCharacter?.AdjustHealthPercentage(.22f);
                break;
            case ItemTypes.SpeedUp:

                break;
            case ItemTypes.PowerUp:

                break;
        }

        
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

        GameObject _obj = InstantiatePrefab(_prefab, _pos + _offset, _parent, _destroyTime, _forward);

        return _obj;
    }

    public void InstantiateEffectAtCenterPoint(GameObject prefab)
    {
        if (prefab == null) return;

        InstantiatePrefab(prefab, centerPoint ? centerPoint.position : Vector3.zero);
    }
    public GameObject InstantiatePrefab(GameObject prefab, Vector3 position = default, Transform parent = null, float destroyTime = 4f, Vector3 _forward = default)
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
}
