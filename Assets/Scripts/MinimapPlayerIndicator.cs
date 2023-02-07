using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPlayerIndicator : MonoBehaviour
{
    #region Variables
    public float rotationSpeed = 25f;
    public float moveSpeed = 10f;

    public Transform lookAtTarget;
    public Transform objToRotate;
    [Space]
    public Camera m_camera;
    #endregion

    void Start()
    {
        if (!objToRotate)
            objToRotate = transform;
        if (!m_camera)
            m_camera = Camera.main;
    }

    void Update()
    {
        RotateToObj();
    }

    public void AssignTarget(Transform _target)
    {
        lookAtTarget = _target;
    }

    void RotateToObj()
    {
        if (!lookAtTarget)
            return;

        Vector3 targetDir = lookAtTarget.position - objToRotate.position;
        Vector3 newDir = Vector3.RotateTowards(objToRotate.forward, targetDir, rotationSpeed * Time.deltaTime, 0.0f);
        Quaternion targetRot = Quaternion.LookRotation(newDir);
        objToRotate.rotation = new Quaternion(targetRot.x, targetRot.y, targetRot.z, targetRot.w);

        Debug.Log(targetRot);
    }
}
