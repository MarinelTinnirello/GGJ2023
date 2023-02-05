using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;



public class CameraRig : MonoBehaviour
{
    [System.Serializable]
    public struct CameraAngles
    {
        public Vector3 CameraPosition;
        public Vector3 CameraRotation;

        public CameraAngles(Vector3 cpos = default, Vector3 crot = default)
        {
            CameraPosition = cpos;
            CameraRotation = crot;
        }
    }

    [Header("Camera Target Settings")]
    public List<Transform> targets;
    public Vector3 offset = new Vector3(0,4,-6);
    public Vector3 gameEndOffset = new Vector3(0, 0, -6);
    public float singleTargetSmoothTime = 0.1f;
    public float multiTargetSmoothTime = 0.4f;
    [Space]
    public bool trackAllCharacterInScene;
    private CharacterTrigger[] activeCharacters;

    [Header("Zoom Settings")]
    public Transform zoomContainer;
    public float minZoom = 2.5f;
    public float maxZoom = 10f;

    private readonly float zoomLimiter = 10f;
    private readonly float zoomOffset = 15f;

    private Vector3 cameraFocalPoint;
    private Transform lockedFocalPoint;
    private Vector3 velocity;
    private Vector3 zoomVelocity;

    [Header("General Settings")]
    public Camera mainCamera;
    public Transform shakeContainer;

    [Header("Camera Angles")]
    public CameraAngles[] powerCameraAngles;
    private Sequence CameraAngleTweenSequence;

    private Tween cameraShakeTween;
    private bool isShaking, isLockedToFocalPoint;

    [HideInInspector]
    public bool gameInProgress = true;

    public virtual void Update()
    {
        if (trackAllCharacterInScene)
        {
            if (targets != null )targets.Clear();
            activeCharacters = FindObjectsOfType<CharacterTrigger>();
            for (int i = 0; i < activeCharacters.Length; i++) targets.Add(activeCharacters[i].transform);
        }

        if (GameManager.Instance)
        {
            gameInProgress = !GameManager.Instance.gameOverCalled;
        }

        if (!gameInProgress)
        {
            //offset = gameEndOffset;
        }
    }

    public virtual void LateUpdate()
    {
        PanCamera();
        SetZoomLevel();
    }

    public virtual void SetFocus(Transform target, bool changeMainTarget = false)
    {
        lockedFocalPoint = target;
        isLockedToFocalPoint = true;

        if (changeMainTarget)
        {
            AddTargetToList(target, true);
        }
    }

    public virtual void AddTargetToCameraRig(Transform target, bool clearAllTargets = false)
    {
        AddTargetToList(target, clearAllTargets);
    }

    private void AddTargetToList(Transform target, bool clearAllTargets = false)
    {
        if (!targets.Contains(target))
        {
            if(clearAllTargets) targets.Clear();
            targets.Add(target);
        }
    }

    public virtual void UpdateSmoothTime(float newSingleTargetSmoothTime = 0.1f, float newMultiTargetSmoothTime = 0.4f)
    {
        singleTargetSmoothTime = newSingleTargetSmoothTime;
        multiTargetSmoothTime = newMultiTargetSmoothTime;
    }

    public virtual void ShakeCamera(float amount = 1f, float duration = .5f, float delay = 0f)
    {
        isShaking = true;

        cameraShakeTween.Kill();
        cameraShakeTween = shakeContainer?.DOShakePosition(duration, amount).SetDelay(delay).OnComplete(OnCameraShakeComplete);
    }

    private void OnCameraShakeComplete()
    {
        shakeContainer.localPosition = Vector3.zero;
        isShaking = false;
    }

    private void PanCamera()
    {
        cameraFocalPoint = isLockedToFocalPoint? lockedFocalPoint.position : GetCenterPoint();

        transform.position = Vector3.SmoothDamp(transform.position, cameraFocalPoint + offset, ref velocity, !isLockedToFocalPoint ? targets.Count > 1 ? multiTargetSmoothTime : singleTargetSmoothTime : singleTargetSmoothTime);
    }

    private Vector3 GetCenterPoint()
    {
        if (targets.Count == 1 && targets!=null && targets[0])
        {
            return targets[0].position;
        }
        else if (targets.Count>1)
        {
            var bounds = new Bounds(targets[0].position, Vector3.zero);
            for ( int i = 0; i < targets.Count; i++)
            {
                bounds.Encapsulate(targets[i].position);
            }
            return bounds.center;
        } else
        {
            return Vector3.zero;
        }
    }

    private void SetZoomLevel()
    {
        float newZoom = isLockedToFocalPoint ? maxZoom : Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        zoomContainer.transform.localPosition = Vector3.SmoothDamp(zoomContainer.transform.localPosition, new Vector3(zoomContainer.transform.localPosition.x, zoomContainer.transform.localPosition.y, newZoom - zoomOffset), ref zoomVelocity, !isLockedToFocalPoint ? 0.1f : 0.5f);
    }

    private float GetGreatestDistance()
    {
        if (targets.Count <= 1) return 0;

        float totalDistance = 0f;

        for (int i = 1; i < targets.Count; i++)
        {
            totalDistance =+  Vector3.Distance(targets[i-1].position, targets[i].position);
        }

        return totalDistance;
    }

    public virtual void SetCameraAngleByID(int angleID = 0)
    {
        if (!mainCamera) return;

        Vector3 pos = powerCameraAngles[angleID].CameraPosition;
        Quaternion rot = Quaternion.Euler(powerCameraAngles[angleID].CameraRotation);

        CameraAngleTweenSequence.Kill();
        CameraAngleTweenSequence = DOTween.Sequence();

        CameraAngleTweenSequence.Append(mainCamera.transform.DOLocalMove(pos, 1f).SetEase(Ease.Linear))
            .Join(mainCamera.transform.DORotateQuaternion(rot, 1f).SetEase(Ease.Linear).SetDelay(1f).OnComplete(ResetCameraAngle));
    }

    public virtual void ResetCameraAngle()
    {
        if (!mainCamera) return;

        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        mainCamera.transform.DOLocalMove(pos, 2f).SetEase(Ease.Linear);
        mainCamera.transform.DORotateQuaternion(rot, 2f).SetEase(Ease.Linear);
    }
}