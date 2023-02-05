using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    #region Variables
    [Header("Line Renderer")]
    public int length;
    public LineRenderer lineRenderer;
    private Vector3[] segments;
    private Vector3[] segmentVelocities;

    [Header("Target Tails")]
    public Transform targetDirection;
    public float targetDistance;
    public float smoothSpeed;

    [Header("Wiggle")]
    public Transform wiggleDirection;
    public float wiggleMagnitude;
    public float wiggleSpeed;

    [Header("Body Parts")]
    public Transform tailEnd;
    public Transform[] bodyParts;
    #endregion

    void Awake()
    {
        lineRenderer.positionCount = length;
        segments = new Vector3[length];
        segmentVelocities = new Vector3[length];

        ResetPos();
    }

    void Update()
    {
        Wiggle();
        LineRendMovement();
    }

    void LineRendMovement()
    {
        segments[0] = targetDirection.position;

        for (int i = 1; i < segments.Length; i++)
        {
            Vector3 targetPos = segments[i - 1] + (segments[i] - segments[i - 1]).normalized * targetDistance;
            segments[i] = Vector3.SmoothDamp(segments[i], targetPos, ref segmentVelocities[i], smoothSpeed);

            //if (bodyParts.Length != 0)
            //    bodyParts[i - 1].transform.position = segments[i];
        }

        lineRenderer.SetPositions(segments);

        if (tailEnd != null)
            tailEnd.position = segments[length - 1];
    }

    void Wiggle()
    {
        wiggleDirection.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnitude);
    }

    void ResetPos()
    {
        segments[0] = targetDirection.position;

        for (int i = 1; i < segments.Length; i++)
        {
            segments[i] = segments[i - 1] + targetDirection.right * targetDistance;
        }

        lineRenderer.SetPositions(segments);
    }
}
