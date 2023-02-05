using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFootstepSystem : MonoBehaviour
{
    public CharacterFootstepTrigger footstepTriggerPrefab;

    [Space, Tooltip("Default Foot Sounds")]
    public AudioClip[] defaultFootSounds;

    [HideInInspector]
    public CharacterEffects characterEffects;

    [HideInInspector]
    public CharacterSoundEffects soundEffects;

    private CharacterTrigger characterMotor;

    private bool leftSet, rightSet;

    public void SetFootstepSystem(Transform _leftFoot, Transform _rightFoot)
    {
        if (!footstepTriggerPrefab) return;

        if (_leftFoot && !leftSet)
        {
            InstantiateGO(footstepTriggerPrefab, _leftFoot.position, _leftFoot);
            leftSet = true;
        }

        if (_rightFoot && !rightSet)
        {
            InstantiateGO(footstepTriggerPrefab, _rightFoot.position, _rightFoot);
            rightSet = true;
        }
    }

    public void CheckFootStepType(Vector3 _pos)
    {
        if (!characterMotor)
        {
            CreateDefaultFootstep(_pos);
            return;
        }

        if (!characterMotor.isRunning)
        {
            CreateDefaultFootstep(_pos);
            return;
        }
    }

    private void CreateDefaultFootstep(Vector3 _pos)
    {
        SendMessage("OnFootStep", _pos, SendMessageOptions.DontRequireReceiver);
        soundEffects?.PlayRandomSoundClip(defaultFootSounds);
    }

    public void EnableFootstepLoop(bool _state)
    {

    }

    public GameObject InstantiateGO(CharacterFootstepTrigger prefab, Vector3 position = default, Transform parent = null, float destroyTime = 0f, Vector3 _forward = default)
    {
        if (prefab == null)
            return null;

        CharacterFootstepTrigger obj = Instantiate(prefab);

        obj.transform.parent = parent;
        obj.transform.position = position;
        obj.transform.localRotation = prefab.transform.localRotation;

        if (_forward != Vector3.zero)
        {
            obj.transform.forward = _forward;
        }

        obj.footstepSystem = this;

        obj.gameObject.SetActive(true);

        if (destroyTime > 0f)
        {
            Destroy(obj, destroyTime);
        }

        return obj.gameObject;
    }
}
