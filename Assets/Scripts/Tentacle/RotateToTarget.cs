using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToTarget : MonoBehaviour
{
    #region Variables
    public bool rotateToCursor = true;

    public float rotationSpeed = 25f;
    private Vector3 direction;
    public float moveSpeed = 10f;

    public Transform lookAtTarget;
    public Transform objToRotate;
    [Space]
    public Camera camera;
    #endregion

    void Start()
    {
        if (!objToRotate)
            objToRotate = transform;
        if (!camera)
            camera = Camera.main;
    }

    public void AssignTarget(Transform _target)
    {
        lookAtTarget = _target;
    }

    void Update()
    {
        if (rotateToCursor)
        {
            RotateObjCursor();
            MoveTowardCursor();
        }
        else
        {
            RotateToObj();
        }
    }

    void RotateObjCursor()
    {
        if (!lookAtTarget)
            return;

        // Convert mouse to 3D space to accomodate the camera
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = camera.nearClipPlane;

        direction = camera.ScreenToWorldPoint(mousePos) - objToRotate.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        objToRotate.rotation = Quaternion.Slerp(objToRotate.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    void MoveTowardCursor()
    {
        if (!lookAtTarget)
            return;

        // Convert mouse to 3D space to accomodate the camera
        Vector3 cursorPos = Input.mousePosition;
        cursorPos.z = objToRotate.position.z - camera.transform.position.z;

        cursorPos = camera.ScreenToWorldPoint(cursorPos);
        objToRotate.position = Vector3.MoveTowards(objToRotate.position, cursorPos, moveSpeed * Time.deltaTime);
    }

    void RotateToObj()
    {
        if (!lookAtTarget)
            return;

        Vector3 targetDir = lookAtTarget.position - objToRotate.position;
        Vector3 newDir = Vector3.RotateTowards(objToRotate.forward, targetDir, rotationSpeed * Time.deltaTime, 0.0f);
        Quaternion targetRot = Quaternion.LookRotation(newDir);
        objToRotate.rotation = new Quaternion(targetRot.x, targetRot.y, targetRot.z, targetRot.w);
    }
}
