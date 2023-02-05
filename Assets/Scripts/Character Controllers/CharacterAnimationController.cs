using UnityEngine;

#if UNITY_EDITOR
[SortingLayerNameList("defaultSortingLayer")]
#endif

public class CharacterAnimationController : MonoBehaviour
{
    public Animator characterAnimator;
    [Space]
    public Transform characterContainer;
    [Space]
    public CharacterAnimationStates currentAnimationState;
    private CharacterAnimationStates prevAnimationState;
    [Space]
    public CharacterFacialExpressions currentFacialExpression;
    private CharacterFacialExpressions prevFacialExpression;

    public FacingDirection currentFacingDirection;
    private FacingDirection prevFacingDirection;
    private FacingDirection lockedFacingDirection;

    [Space]
    public SpriteRenderer[] staticSpriteRenderers;
    
    [HideInInspector]
    public string defaultSortingLayer;
    private Component[] spriteRenderers;

    [Space]
    public HeadLookController headLookAt;
    public DynamicBone hairMovement;

    private readonly string animatorLabelAnimationStateID = "StateID";
    private readonly string animatorLabelAttackID = "AttackID";
    private readonly string animatorLabelAttackTrigger = "Attack";
    private readonly string animatorLabelPowerTypeID = "PowerType";
    private readonly string animatorLabelOnHit = "OnHit";
    private readonly string animatorLabelWasJustHit = "WasJustHit";
    private readonly string animatorLabelIsMoving = "Moving";
    private readonly string animatorLabelMoveSpeed = "Speed";
    private readonly string animatorLabelIsGrounded = "IsGrounded";
    private readonly string animatorLabelIsNearGround = "IsNearGround";
    private readonly string animatorLabelIsRunning = "IsRunning";
    private readonly string animatorLabelIsBlocking = "Blocking";

    private float lastSpeed = -1.0f;

    [HideInInspector]
    public float moveSpeed, horizontal, vertical = 0f;

    [HideInInspector]
    public bool isGrounded, isNearGround = true;

    [HideInInspector]
    public bool isRunning;

    [Space]
    public CharacterHealth characterHealth;

    [Space]
    public bool canTurn;

    private CharacterController characterController;
    private CharacterTrigger characterInputController;
    private Rigidbody rb;

    private Quaternion originalRotation;
    private Vector3 originalEulerAngles;

    private void Start()
    {
        if (!characterAnimator) characterAnimator = GetComponent<Animator>();
        spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();

        originalRotation = characterContainer? characterContainer.localRotation : transform.localRotation;
        originalEulerAngles = characterContainer? characterContainer.eulerAngles : transform.eulerAngles;

        UpdateSortingLayers(defaultSortingLayer);
    }

    private void Update()
    {
        bool hasMovement = Mathf.Abs(horizontal) + Mathf.Abs(vertical) != 0f;

        if (currentAnimationState != prevAnimationState) SetAnimationType(currentAnimationState);
        if (currentFacingDirection != prevFacingDirection) UpdateCharacterFacingDireciton(currentFacingDirection);

        if (!characterAnimator) return;

        //characterAnimator?.SetFloat(animatorLabelMoveSpeed, moveSpeed);
        characterAnimator?.SetBool(animatorLabelIsMoving, hasMovement);
        characterAnimator?.SetBool(animatorLabelIsBlocking, characterInputController? characterInputController.isBlocking : false);

        //if(characterInputController) characterAnimator?.SetBool("Blocking", characterInputController.isBlocking);

        characterAnimator?.SetFloat("Velocity X", Mathf.Abs(horizontal));// transform.InverseTransformDirection(GetVelocity()).x);
        characterAnimator?.SetFloat("Velocity Y", characterInputController ? characterInputController.velocity.y : 0f);
        characterAnimator?.SetFloat("Velocity Z", Mathf.Abs(vertical));// transform.InverseTransformDirection(GetVelocity()).z);

        if (hairMovement)
        {
            hairMovement.enabled = characterInputController ? characterInputController.velocity.y > 0f ? false : true : true;
        }
    }

    private Vector3 GetVelocity()
    {
        if (characterController) return characterController.velocity;
        if (rb) return rb.velocity;
        return Vector3.zero;
    }

    public void Movement(float h, float v)
    {
        horizontal = h;
        vertical = v;
    }

    public void SmoothHaltMovement()
    {
        horizontal = horizontal > 0f ? Mathf.Abs(horizontal) - .1f : 0f;
        vertical = vertical > 0f ? Mathf.Abs(vertical) - .1f : 0f;
    }

    public void SetCharacterInputController(CharacterTrigger targetInputController)
    {
        characterInputController = targetInputController;
    }

    public void SetCharacterController(CharacterController targetController)
    {
        characterController = targetController;
    }

    public void SetLookAtTarget(GameObject lookAtTarget)
    {
        if (lookAtTarget && headLookAt)
            headLookAt.target = lookAtTarget;
        else if (headLookAt)
            headLookAt.enabled = false;
    }

    public void SetRigidbody(Rigidbody targetRB)
    {
        rb = targetRB;
    }

    public void SetGroundState(bool _state)
    {
        isGrounded = _state;
        characterAnimator?.SetBool(animatorLabelIsGrounded, isGrounded);
    }

    public void SetSpeed(float _speed)
    {
        if (lastSpeed == _speed) return;

        moveSpeed = lastSpeed = _speed;

        characterAnimator?.SetFloat(animatorLabelMoveSpeed, moveSpeed);

        print("_speed = " + _speed);
    }

    public void SetNearGroundState(bool _state)
    {
        isNearGround = isGrounded? true : _state;
        characterAnimator?.SetBool(animatorLabelIsNearGround, isNearGround);
    }

    public void SetRunState(bool _state)
    {
        isRunning = _state;
        characterAnimator?.SetBool(animatorLabelIsRunning, isRunning);
    }

    public void SetHitState(bool _state)
    {
        if (!characterAnimator) return;

        characterAnimator?.SetBool(animatorLabelWasJustHit, _state);
    }

    public void OnHit()
    {
        if (!characterAnimator) return;

        characterAnimator?.SetTrigger(animatorLabelOnHit);
    }

    public void OnTransform(int powerTypeID = 0)
    {
        if (!characterAnimator) return;

        characterAnimator?.SetInteger(animatorLabelPowerTypeID, powerTypeID);
    }

    public void SetAnimationType(CharacterAnimationStates _state, bool _isActive = true)
    {
        bool _updateAnimationTypeID = true;

        switch (_state)
        {
            case CharacterAnimationStates.isIdle: break;
            case CharacterAnimationStates.isWalking: break;
            default: break;
        }

        if (currentAnimationState == prevAnimationState) return;

        if (_updateAnimationTypeID && characterAnimator) characterAnimator?.SetInteger(animatorLabelAnimationStateID, (int)_state);

        currentAnimationState = prevAnimationState = _state;
    }

    public void UpdateCharacterFacingDireciton(FacingDirection _direction, bool _lock = true)
    {
        if(!canTurn)return;

        if (characterContainer) characterContainer.eulerAngles = new Vector3(0, _direction == FacingDirection.Right ? originalEulerAngles.y : originalEulerAngles.y+180, 0);
        if (_lock) lockedFacingDirection = _direction;

        currentFacingDirection = prevFacingDirection = _direction;

        FlipStaticSpriteRenderers(currentFacingDirection == FacingDirection.Left);
    }

    public void PreformAttack(AttackType attackType)
    {
        characterAnimator?.SetInteger(animatorLabelAttackID, (int)attackType);
        characterAnimator?.SetTrigger(animatorLabelAttackTrigger);
    }

    private void FlipStaticSpriteRenderers(bool flip)
    {
        for (int i = 0; i < staticSpriteRenderers.Length; i++)
        {
            staticSpriteRenderers[i].flipX = !flip;
        }
    }

    public void UpdateSortingLayers(string targetLayer)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            spriteRenderer.sortingLayerName = targetLayer;

        characterHealth?.SetSortingLayer(targetLayer);
    }
}
