using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    GameObject targetObj;
    public float speed = 3.5f;
    public bool smooth;

    private void Start()
    {
        targetObj = Camera.main.gameObject;
    }

    private void LateUpdate()
    {
        if (smooth)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetObj.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        } else
        {
            transform.LookAt(Camera.main.transform.position);
        }
    }
}
