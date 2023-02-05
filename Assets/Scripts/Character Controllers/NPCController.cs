using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : AIController
{
    public override void Start()
    {
        base.Start();

        SetCharacterVariantByID((int)CharacterList.NPC);
    }

    public override void Update()
    {
        base.Update();

        if (followTarget) MoveTowardsTarget(followTarget);
    }

    public override void MoveTowardsTarget(GameObject targetObject)
    {
        Vector3 targetPos = GetMoveToPosition(targetObject.transform.position);
        Vector3 posDifference;
        bool setMovement = false;

        float currentDistance = Vector3.Distance(transform.position, targetPos);
        float targetDistance = Vector3.Distance(targetPos, targetPos + followTargetOffset);

        if (wasMoving && currentDistance > targetDistance)
        {
            setMovement = true;
        }
        else if (!isMoving && currentDistance > targetDistance*2f)
        {
            setMovement = true;
        }

        if (setMovement)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, followSmoothTime);

            posDifference = (transform.position - previousPos) * 100f;

            previousPos = transform.position;

            characterAnimationController?.Movement(Mathf.Abs(posDifference.x), Mathf.Abs(posDifference.z));

            if (lookAtTarget) RotateTowardsTarget(targetPos);

            isMoving = wasMoving = true;
        }
        else
        {
            characterAnimationController?.SmoothHaltMovement();
            isMoving = wasMoving = false;
        }
    }
    public void OnTransform(int powerTypeID = 0)
    {
        characterAnimationController?.OnTransform(powerTypeID);
        characterSoundEffects?.CallTransformSounds(powerTypeID);
        characterEffects?.OnTransform(powerTypeID);
    }

    public override void OnGameOver(GameOverState gameOverState, bool playerWon = false)
    {
        base.OnGameOver(gameOverState, playerWon);

        if(gameOverState == GameOverState.Lose)
        {
            DestroyThisObject();
        }
    }
}