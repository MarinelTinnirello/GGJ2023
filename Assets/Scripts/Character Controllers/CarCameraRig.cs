using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class CarCameraRig : CameraRig
{
    public CarController carController;
    public LayerMask cameraTriggerLayers;

    public bool debugMode;

    private float currentCameraAngle;
    private float lastCheckedCameraAngle;
    private float lastCameraAngle;

    private string currentCameraAngleName;

    private Transform target;

    private int angleCheckCount = 100;
    private int currentAngleCheckCount = 0;

    public bool isActive = true;

    // Start is called before the first frame update
    private void Start()
    {
        if (!mainCamera) mainCamera = Camera.main;
        if (carController) target = carController.gameObject.transform;

        AddTargetToCameraRig(carController.transform, true);
    }

    // Update is called once per frame
    public override void Update()
    {
        if (isActive) base.Update();

        if (!mainCamera || target == null) return;
        mainCamera.transform.LookAt(target);

        //transform.position = target.transform.position + cameraOffset;
    }

    public override void LateUpdate()
    {
        if (isActive) base.LateUpdate();

        //Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Ray ray = new Ray(mainCamera.transform.position, target.transform.position - mainCamera.transform.position);
        RaycastHit raycastHit;
        bool raycastHitTarget = Physics.Raycast(ray, out raycastHit, Mathf.Infinity, cameraTriggerLayers);
        //bool raycastHitTarget = Physics.SphereCast(mainCamera.transform.position, 0.1f, target.transform.position, out raycastHit, Mathf.Infinity, cameraTriggerLayers);

        if (raycastHitTarget)
        {
            currentCameraAngleName = raycastHit.transform.name;

            //Vector3 localHit = target.transform.InverseTransformPoint(raycastHit.point);
            //print(localHit);

            //Debug.Log(raycastHit.transform.name + " got hit at: " + target.transform.InverseTransformPoint(raycastHit.point));
        }


        if (debugMode)
        {
            float dist = Vector3.Distance(mainCamera.transform.position, target.transform.position);
            Debug.DrawRay(ray.origin, ray.direction * dist, raycastHitTarget ? Color.red : Color.green);
        }

        //print("Camera = " + mainCamera.transform.forward + " ||  Target = " + target.transform.forward);

        //Vector3 relative;
        //relative = mainCamera.transform.InverseTransformDirection(target.transform.forward);
        //Debug.Log( Math.Round(relative.z, 2) );


        OnCameraUpdate();
    }

    /*private void OnCameraUpdate()
    {

        Vector3 relative;
        //relative = mainCamera.transform.InverseTransformDirection(target.transform.forward);
        relative = target.transform.InverseTransformDirection(Camera.main.transform.forward);
        //Camera.main.transform.localEulerAngles.y
        //Debug.Log( Math.Round(relative.z, 3) );

        currentCameraAngle = mainCamera.transform.rotation.y - target.transform.rotation.y;

        if (lastCameraAngle == currentCameraAngle) return;
        lastCameraAngle = currentCameraAngle;

        //print(currentCameraAngle);

        carController?.OnCameraChange(currentCameraAngle);
    }*/

    private void OnCameraUpdate()
    {
        switch (currentCameraAngleName)
        {
            case "Left":
                currentCameraAngle = 0f;
                break;
            case "RearLeft":
                currentCameraAngle = 1f;
                break;
            case "Rear":
                currentCameraAngle = 2f;
                break;
            case "RearRight":
                currentCameraAngle = 3f;
                break;
            case "Right":
                currentCameraAngle = 4f;
                break;
            case "FrontRight":
                currentCameraAngle = 5f;
                break;
            case "Front":
                currentCameraAngle = 6f;
                break;
            case "FrontLeft":
                currentCameraAngle = 7f;
                break;
            default:
                return;
        }

        //if (lastCameraAngle == currentCameraAngle) return;

        //if (!CheckIsValid()) return;

        lastCameraAngle = currentCameraAngle;
        currentAngleCheckCount = 0;

        //print(currentCameraAngle);

        carController?.OnCameraChange(currentCameraAngle);
    }

    private bool CheckIsValid()
    {
        if(lastCheckedCameraAngle == currentCameraAngle)
        {
            currentAngleCheckCount++;
        } else
        {
            lastCheckedCameraAngle = currentCameraAngle;
            currentAngleCheckCount = 0;
        }

        return currentAngleCheckCount >= angleCheckCount;
    }
}