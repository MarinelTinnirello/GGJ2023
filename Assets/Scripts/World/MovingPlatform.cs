using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public ActiveAxis moveOnAxis = ActiveAxis.X;
    [Space]
    public Vector3 movement;
    public Vector3 endPos;
    private Vector3 startPos;
    [Space]
    public bool haltOnGameOver = true;
    public bool deactiveOnEnd = true;

    private MovingPlatformManager platformManager;

    private int platformID;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        if (GameManager.Instance)
            if (GameManager.Instance.gamePaused || GameManager.Instance.gameOverCalled && haltOnGameOver) return;

        transform.localPosition = transform.localPosition + movement;

        CheckIfAtEnd();
    }

    public void SetPlatform(int setID, MovingPlatformManager movingPlatformManager, Vector3 endAtPos, Vector3 movementSpeed, ActiveAxis moveAxis)
    {
        platformID = setID;
        platformManager = movingPlatformManager;
        endPos = endAtPos;
        movement = movementSpeed;
        moveOnAxis = moveAxis;
    }

    private void CheckIfAtEnd()
    {
        if (!deactiveOnEnd) return;

        if (moveOnAxis == ActiveAxis.X)
        {
            if (endPos.x < startPos.x && transform.localPosition.x <= endPos.x)
            {
                platformManager?.OnObjectRemoved(platformID);
            }

            if (endPos.x > startPos.x && transform.localPosition.x >= endPos.x)
            {
                platformManager?.OnObjectRemoved(platformID);
            }
        }

        if (moveOnAxis == ActiveAxis.Z)
        {
            if (endPos.z < startPos.z && transform.localPosition.z <= endPos.z)
            {
                platformManager?.OnObjectRemoved(platformID);
            }

            if (endPos.z > startPos.z && transform.localPosition.z >= endPos.z)
            {
                platformManager?.OnObjectRemoved(platformID);
            }
        }
    }
}