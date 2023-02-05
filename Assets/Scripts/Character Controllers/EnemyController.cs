using System.Collections;
using UnityEngine;

public class EnemyController : AIController
{
    public override void Start()
    {
        base.Start();

        characterAnimationController?.SetCharacterController(characterController);
        if (characterVariantController && !variantSet) SetCharacterVariantByID(UnityEngine.Random.Range(2, characterVariantController.characterVariantOptions.Length));
    }

    public override void Update()
    {
        if (!characterIsSetup) return;

        base.Update();
        
        CheckCharacterState();
    }

    private void CheckCharacterState()
    {
        if (gameOverCalled) return;

        state = isKnockedOut? CharacterState.knockedOut : CharacterState.IsAttacking;

        switch (state)
        {
            case CharacterState.IsAttacking:
                MoveTowardsTarget(followTarget);
                break;
            case CharacterState.knockedOut:
                //MoveTowardsTarget(targetGameObject);
                break;
            default:
                MoveTowardsTarget(followTarget);
                break;
        }

        //if (targetGameObject) characterAnimationController?.UpdateCharacterFacingDireciton(targetGameObject.transform.position.x > transform.position.x ? FacingDirection.Right : FacingDirection.Left, true);
    }

    public override void SetCharacterVariantByID(int characterType)
    {
        SetCharacterType(CharacterType.Enemy);

        base.SetCharacterVariantByID(characterType);
    }

    /*private void UpdateOnFloor()
    {
        vertical = 1;

        if (transform.position.z > jumpLine && !isOnStage) Jump();
    }

    private void UpdateOnStage()
    {
        MoveTowardsTarget(targetGameObject);
    }

    private void UpdateDragging()
    {
        horizontal = dragDirection.x;
        vertical = dragDirection.z;
    }*/

    public override void MoveTowardsTarget(GameObject targetObject)
    {
        if (!targetObject || isKnockedOut) return;

        /*directionToTarget = (targetObject.transform.position+ followTargetOffset) - transform.position;
        horizontal = directionToTarget.x;
        vertical = directionToTarget.z;*/


        Vector3 targetPos = GetMoveToPosition(targetObject.transform.position);
        Vector3 posDifference;
        bool setMovement = false;

        float currentDistance = Vector3.Distance(transform.position, targetPos);
        float targetDistance = Vector3.Distance(targetPos, targetPos + followTargetOffset);

        if (wasMoving && currentDistance > targetDistance)
        {
            setMovement = true;
        }
        else if (!isMoving && currentDistance > targetDistance * 2f)
        {
            setMovement = true;
        }

        if (setMovement)
        {
            directionToTarget = targetPos - transform.position;
            //directionToTarget = Vector3.SmoothDamp(transform.position, directionToTarget, ref vel, followSmoothTime);

            horizontal = directionToTarget.x;
            vertical = directionToTarget.z;

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

    public override void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            //StartDragging();
        }
    }

    public override void OnHit(float damageAmount, AttackType hitType)
    {
        base.OnHit(damageAmount, hitType);

        characterAnimationController?.OnHit();
    }

    public override void OnKnockOut(float knockOutTime = 4f)
    {
        characterSoundEffects?.PlayRandomSoundClip(characterSoundEffects.knockOut, 0f);

        base.OnKnockOut(knockOutTime);

        if (GameManager.Instance)
        {
            Transform newParent = GameManager.Instance.parentTransformOnKO;

            transform.parent = newParent? newParent : null;

            gameObject.layer = LayerMask.NameToLayer("KnockedOut");
        }
        
    }

    public override void OnGameOver(GameOverState gameOverState, bool playerWon = false)
    {
        base.OnGameOver(gameOverState, playerWon);

        if (gameOverState == GameOverState.Win)
        {
            OnKnockOut(0f);
        }
    }
}