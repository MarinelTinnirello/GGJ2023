using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterAnimationController))]

public class AIController : CharacterTrigger
{
    [Header("Movement Settings")]
    public float speed = 6f;
    public float runSpeed = 6f;
    public float dragSpeed = 2f;
    public float jumpHeight = 4.5f;
    public float gravity = -100f;
    private float speedMultiplyer = 1;
    [Space]
    public ActiveAxis lockAxis;
    public bool enabledLockedAxis;

    [Header("AI Settings")]
    public CharacterType targetCharacter = CharacterType.Enemy;
    public GameObject followTarget;
    public Vector3 followTargetOffset;
    public float followSmoothTime = .5f;//.5f;
    [HideInInspector]
    public Vector3 directionToTarget;
    [Space]
    public bool autoFireWhenInRange;
    public bool lookAtTarget;

    [HideInInspector]
    public CharacterState state;

    [HideInInspector]
    public float jumpLine;

    [HideInInspector]
    public BeatTracker tracker;

    [Header("Character States")]
    public bool isOnStage;

    [Space]
    public EnviromentAreas currentArea;
    
    [HideInInspector]
    public float horizontal, vertical;

    [HideInInspector]
    public bool variantSet;

    private Vector3 direction;

    [HideInInspector]
    public Vector3 origPos, previousPos, vel;

    public override void Start()
    {
        base.Start();

        if (!characterController) characterController = GetComponent<CharacterController>();
        if (!characterAnimationController) characterAnimationController = GetComponent<CharacterAnimationController>();
        if (!characterVariantController) characterVariantController = GetComponent<CharacterVariantController>();
        
        if (GameManager.Instance)
        {
            jumpLine = GameManager.Instance.GetJumpPoint();
            tracker = GameManager.Instance.beatTracker;
        }

        origPos = transform.position;

        SetWeaponType(defaultWeaponID);
        SetMoveToTarget();

        characterAnimationController?.SetLookAtTarget(followTarget);
    }

    public override void Update()
    {
        direction = new Vector3(horizontal, 0, vertical).normalized;
        isMoving = direction.magnitude >= 0.1f;

        isGrounded = characterController ? characterController.isGrounded : true;
        
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        //speed = (state == CharacterState.dragging || state == CharacterState.beingDragged) ? dragSpeed : runSpeed;

        if (wasJustHit)
        {
            direction = (direction / 2) * -1;
            //speed = speed * 4f;
        }

        speedMultiplyer = 1;

        if (isMoving)
        {
            characterController?.Move(GetMoveToPosition(direction * speed * Time.deltaTime));
        }

        velocity.y += gravity * Time.deltaTime;

        /*if (characterController)
        {
            velocity = GetMoveToPosition(velocity);
        }*/

        characterController?.Move(GetMoveToPosition(velocity * Time.deltaTime));

        horizontal = vertical = 0f;

        if (autoFireWhenInRange)
        {
            if (!isAttacking && !wasJustHit) CallAttack(AttackType.Low);
        }

        UpdateAnimator();

        base.Update();
    }

    public virtual void SetCharacterVariantByID(int characterType)
    {
        if (!characterVariantController) return;
        characterVariantController.currentVariantID = (CharacterList)characterType;
        variantSet = true;
    }

    public virtual void UpdateAnimator()
    {
        if (!characterAnimationController) return;

        characterAnimationController?.SetAnimationType(isGrounded? isMoving ? CharacterAnimationStates.isWalking : CharacterAnimationStates.isIdle : CharacterAnimationStates.isJumping);
        characterAnimationController?.SetGroundState(isGrounded);
    }

    public virtual void SetMoveToTarget()
    {
        if (GameManager.Instance)
        {
            switch (targetCharacter)
            {
                case CharacterType.Enemy:
                    //
                    break;
                case CharacterType.MainPlayer:
                    followTarget = GameManager.Instance.mainCharacter;
                    break;
                case CharacterType.NPC:
                    followTarget = GameManager.Instance.nonPlayableCharacter.gameObject;
                    break;
                default:
                    //
                    break;
            }
        }
    }

    public virtual void MoveTowardsTarget(GameObject targetObject)
    {

    }

    public virtual Vector3 GetMoveToPosition(Vector3 targetPos)
    {
        if (enabledLockedAxis)
        {
            if (lockAxis == ActiveAxis.X)
            {
                targetPos.x = origPos.x;
            }
            else if (lockAxis == ActiveAxis.Y)
            {
                targetPos.y = origPos.y;
            }
            else if (lockAxis == ActiveAxis.Z)
            {
                targetPos.z = origPos.z;
            }
            else if (lockAxis == ActiveAxis.XY)
            {
                targetPos.x = origPos.x;
                targetPos.y = origPos.y;
            }
        }

        return targetPos;
    }

    public virtual void RotateTowardsTarget(Vector3 targetPos)
    {
        Vector3 lookDir = targetPos - transform.position;
        lookDir.y = 0;

        Quaternion _rot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, _rot, 7.5f * Time.deltaTime);
    }

    public virtual void Jump()
    {
        if (!isGrounded) return;

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public virtual void CallAttack(AttackType attackType)
    {
        if (isAttacking || wasJustHit || isKnockedOut) return;

        characterAnimationController?.PreformAttack(attackType);
        activeWeapon?.Attack();

        attackStrenghType = attackType;
        attackCalled = isAttacking = true;

        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(OnAttack(attackTime, attackWaitTime));
    }

    public virtual void UpdateCurrentArea(EnviromentAreas activeArea)
    {
        currentArea = activeArea;

        isOnStage = activeArea == EnviromentAreas.Stage;
    }
}
