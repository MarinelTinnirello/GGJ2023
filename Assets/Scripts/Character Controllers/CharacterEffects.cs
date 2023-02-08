using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffects : MonoBehaviour
{
    public Transform centerPoint;
    [Space, Space]
    public GameObject onHitPrefab;
    public GameObject onLandPrefab;
    public GameObject onLargeLandPrefab;
    public GameObject transformationEffect;
    public GameObject[] enableOnRun;
    private bool runObjectsOn;

    [Tooltip("Effect prefab to instantiate on character step.")]
    public GameObject FootStepPrefab;
    public ParticleSystem footStepLoopPS;
    private ParticleSystem.MainModule footStepLoop;
    public float footStepLoopY;
    private bool footLoopActive;

    [Space]
    public ParticleSystem[] dustTailPS;
    private ParticleSystem.MainModule[] dustTailPSMain;
    private bool dustTrailActive;


    [Space]
    public GameObject shadow;

    [Space]
    public bool shakeCameraOnLand;

    //[Space, Tooltip("Trail to enable when character is dashing")]
    //public SpriteTrail.SpriteTrail dashTrail;

    [HideInInspector]
    public CharacterSoundEffects characterSoundEffects;

    [HideInInspector]
    public CharacterAnimationController characterAnimationController;

    [HideInInspector]
    public bool isDashing;

    [HideInInspector]
    public bool isBlocking;

    private void Awake()
    {
        if (footStepLoopPS)
        {
            footStepLoop = footStepLoopPS.main;
            //footStepLoopY = footStepLoopPS.gameObject.transform.localPosition.y;
        }

        /*for (int i = 0; i < dustTailPS.Length; i++)
        {
            dustTailPSMain[i] = dustTailPS[i].main;
        }*/

        EnableObjectsOnRun(false, true);
    }

    private void Update()
    {
        CheckIfRunning();
    }

    private void CheckIfRunning()
    {
        if (!characterAnimationController) return;

        if (characterAnimationController.isGrounded && characterAnimationController.isRunning)
        {
            if (!footLoopActive && footStepLoopPS)
            {
                footStepLoopPS.Play();
                //footStepLoop.loop = true;

                footLoopActive = true;
            }
        } else
        {
            EndFootStepPS();
        }

        EnableObjectsOnRun(characterAnimationController.isRunning);
    }

    public void EnableObjectsOnRun(bool _state, bool forceUpdate = false)
    {
        if (runObjectsOn == _state && !forceUpdate) return;

        for (int i = 0; i < enableOnRun.Length; i++)
        {
            enableOnRun[i].SetActive(_state);
        }

        CameraRig.Instance?.EnableSpeedLines(_state);

        runObjectsOn = _state;
    }

    public void EndFootStepPS()
    {
        if (footStepLoopPS)
        {
            //footStepLoop.loop = false;
            footStepLoopPS?.Stop();
        }

        footLoopActive = false;
    }

    public void EnableDustTrail(bool _state)
    {
        //if (dustTailPS.Length < 1 || dustTailPSMain.Length < 1) return;

        for (int i = 0; i < dustTailPS.Length; i++)
        {
            if (_state)
            {
                dustTailPS[i].Play();
                //dustTailPSMain[i].loop = _state;

                dustTrailActive = _state;
            }
            else
            {
                dustTailPS[i].Stop();
                //dustTailPSMain[i].loop = _state;

                dustTrailActive = _state;
            }
        }
    }

    public void OnTransform(int powerTypeID = 0, float activeTime = 10f)
    {
        AddEffect(transformationEffect);

        StartCoroutine(TempShowMotionTrail(activeTime));
    }

    private IEnumerator TempShowMotionTrail(float activeTime = 10f)
    {
        EnableDashTrail(true);
        yield return new WaitForSeconds(1.5f);
        EnableDashTrail(false);
    }

    public void EnableDashTrail(bool state)
    {
        /*if (state)
            dashTrail?.EnableTrail();
        else
            dashTrail?.DisableTrail();*/
    }

    public void EnableSmokeTrail(bool state)
    {
        /*if (state)
            dashTrail?.EnableTrail();
        else
            dashTrail?.DisableTrail();*/
    }

    public void OnHit()
    {
        InstantiateEffectAtCenterPoint(onHitPrefab);
    }

    public void OnGroundStateChange(bool _isGrounded, bool largeLand = false)
    {
        if (shadow)
        {
            shadow.SetActive(_isGrounded);
        }

        if (_isGrounded) OnLand(largeLand);
    }

    public void OnLand(bool largeLand = false)
    {
        if (largeLand)
        {
            AddEffect(onLargeLandPrefab, false);
            if (shakeCameraOnLand) CameraRig.Instance?.ShakeCamera(0.9f,0.5f);
        } else
        {
            AddEffect(onLandPrefab, false);
        }
        
    }

    public void OnFootStep(Vector3 position)
    {
        InstantiatePrefab(FootStepPrefab, position, null);

        characterSoundEffects?.CallFootStepSoundEffect();
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
