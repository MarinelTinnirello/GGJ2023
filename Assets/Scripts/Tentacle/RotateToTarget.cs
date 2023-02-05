using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToTarget : MonoBehaviour
{
    #region Variables
    public bool rotateToCursor = true;

    public float rotationSpeed;
    private Vector3 direction;
    public float moveSpeed;

    public Transform target;
    #endregion

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
        // Convert mouse to 3D space to accomodate the camera
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;

        direction = Camera.main.ScreenToWorldPoint(mousePos) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    void MoveTowardCursor()
    {
        // Convert mouse to 3D space to accomodate the camera
        Vector3 cursorPos = Input.mousePosition;
        cursorPos.z = transform.position.z - Camera.main.transform.position.z;

        cursorPos = Camera.main.ScreenToWorldPoint(cursorPos);
        transform.position = Vector3.MoveTowards(transform.position, cursorPos, moveSpeed * Time.deltaTime);
    }

    void RotateToObj()
    {
        Vector3 targetDir = target.position - transform.position;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotationSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
