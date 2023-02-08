using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterTrigger : MonoBehaviour
{
    [Header("Character Setup")]
    public CharacterController characterController;
    public CharacterVariantController characterVariantController;
    public LayerMask groundCheckLayers;
    [HideInInspector]
    public bool isGrounded, isNearGround, isHumanoid, characterIsSetup, wasGrounded, wasNearGround, wasAirborne, isAirborne;
    [HideInInspector]
    public CharacterSetup characterSetupController;

    [HideInInspector]
    public MovementType characterRenderType = MovementType.is3D;

    [Header("Character Display Settings")]
    public CharacterAnimationController characterAnimationController;
    public GameObject defaultDisplayObject;
    public GameObject onKnockOutDisplayObject;
    [Space]
    public Transform centerPoint;
    [Space]
    public GameObject[] enableOnMove;
    [Space]
    public bool hideDefaultDisplayOnKO = true;
    [HideInInspector]
    public Camera mainCamera;

    [Header("Character Health")]
    public CharacterHealth healthBar;
    public float maxHealth = 150f;

    [Header("Character Effects")]
    public CharacterEffects characterEffects;

    [Header("Sound Effects")]
    public CharacterSoundEffects characterSoundEffects;

    [Header("Trigger Settings")]
    public CharacterType characterType;
    public DamageDealer[] damageDealers;
    public float takeDamageWaitTime = 0.5f;
    public Coroutine takeDamageWaitTimeCO;
    public bool isTakingDamage;
    [Space]
    public Collider seperateHitCollider;
    [HideInInspector]
    public Transform leftFoot, rightFoot;
    [HideInInspector]
    public Collider characterCollider;
    [HideInInspector]
    public CharacterFootstepSystem characterFootstepSystem;

    [Header("Combat Settings")]
    public float[] attackDamageAmount = new float[] { 50f, 75f, 150f };
    public AttackType attackStrenghType;
    public float attackRadius = 1f;
    public float attackTime = .25f;
    public float attackWaitTime = .5f;
    public float jumpKickDashAmmount = 3f;
    public bool gettingHitHaltsAttack = true;
    public bool canBlock;
    [Space]
    public bool destroyOnKnockOut = true;

    [Header("Run Settings")]
    public float runSpeedMultiplier = 2.1f;
    private bool wasRunning;

    [HideInInspector]
    public float moveSpeed;

    [Header("Weapons Types")]
    public Weapon[] weapons;
    public int defaultWeaponID;
    [HideInInspector]
    public Weapon activeWeapon;
    [HideInInspector]
    public int activeWeaponID;

    [HideInInspector]
    public Coroutine attackCoroutine;

    [HideInInspector]
    public Vector3 velocity;

    [HideInInspector]
    public bool isRunning, attackCalled, isAttacking, isKnockedOut, wasJustHit, gameOverCalled, isBlocking, isMoving, wasMoving;

    [HideInInspector]
    public float lastTime;

    public virtual void Start()
    {
        characterAnimationController?.SetCharacterInputController(this);
        healthBar?.SetParentTransform(transform);
        healthBar?.SetMaxHealth(maxHealth);

        characterSetupController = GetComponent<CharacterSetup>();
        characterCollider = GetComponent<Collider>();

        mainCamera = Camera.main;
    }

    public virtual void OnControllerColliderHit(ControllerColliderHit hit)
    {

    }

    public virtual void OnCollisionEnter(Collision collision)
    {

    }

    public virtual void Update()
    {
        CheckRunningState();
    }

    public virtual void FixedUpdate()
    {
        isGrounded = GetGroundedState()[0];
        isNearGround = GetGroundedState()[1];
    }

    public virtual void LateUpdate()
    {
        float _dt = Time.deltaTime;
        float _time = Time.realtimeSinceStartup;
        float _delta = _time - lastTime;

        lastTime = _time;

        //UpdateAnimator(_dt);
    }

    public virtual void SetCharacterRenderType(MovementType renderType)
    {
        characterRenderType = renderType;
    }

    public virtual void SetCharacterCollider(Collider col)
    {
        characterCollider = col;
    }

    public virtual void SetCharacterType(CharacterType characterType)
    {
        if (!characterSetupController) characterSetupController = GetComponent<CharacterSetup>();
        characterSetupController?.SetCharacterType(characterType);
    }

    public virtual bool[] GetGroundedState()
    {
        float groundOffset = 0.1f;
        bool grounded = false;
        bool nearground = false;

        RaycastHit raycastHit;
        RaycastHit raycastFarHit;

        if (characterRenderType == MovementType.is3D)
        {
            if (Physics.Raycast(GetColliderCenterPoint(), transform.TransformDirection(Vector3.down), out raycastHit, GetColliderHeight() + groundOffset, groundCheckLayers))
            {
                grounded = true;
            }

            if (Physics.Raycast(GetColliderCenterPoint(), transform.TransformDirection(Vector3.down), out raycastFarHit, GetColliderHeight() * 2.5f, groundCheckLayers))
            {
                nearground = true;
            }

            Debug.DrawRay(GetColliderCenterPoint(), Vector2.down * (GetColliderHeight() * 1.5f), raycastHit.collider != null ? Color.clear : raycastFarHit.collider != null ? Color.yellow : Color.red);
            Debug.DrawRay(GetColliderCenterPoint(), Vector2.down * (GetColliderHeight() + groundOffset), raycastHit.collider != null ? Color.green : Color.red);

        }
        else
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(GetColliderCenterPoint(), Vector2.down, GetColliderHeight() + groundOffset, groundCheckLayers);
            RaycastHit2D raycastFarHit2D = Physics2D.Raycast(GetColliderCenterPoint(), Vector2.down, GetColliderHeight() * 2.5f, groundCheckLayers);

            grounded = raycastHit2D.collider != null;
            nearground = raycastFarHit2D.collider != null;

            Debug.DrawRay(GetColliderCenterPoint(), Vector2.down * (GetColliderHeight() * 2.0f), raycastHit2D.collider != null ? Color.clear : raycastFarHit2D.collider != null ? Color.yellow : Color.red);
            Debug.DrawRay(GetColliderCenterPoint(), Vector2.down * (GetColliderHeight() + groundOffset), raycastHit2D.collider != null ? Color.green : Color.red);
        }

        return new bool[] { grounded, nearground };
    }

    public virtual Vector3 GetColliderCenterPoint()
    {
        return characterCollider ? characterCollider.bounds.center : characterController? characterController.center : Vector3.zero;
    }

    public virtual float GetColliderHeight()
    {
        return characterCollider ? characterCollider.bounds.extents.y : characterController ? characterController.height : 1.0f;
    }

    public virtual void CheckGroundState()
    {
        isGrounded = characterController ? characterController.isGrounded : isGrounded;
        
        if (wasNearGround != isNearGround)
        {
            characterAnimationController?.SetNearGroundState(isNearGround);
            wasNearGround = isNearGround;

            if (!isGrounded && !isNearGround) wasAirborne = true;
        }

        characterAnimationController?.SetAnimationType(isGrounded ? isMoving ? CharacterAnimationStates.isWalking : CharacterAnimationStates.isIdle : CharacterAnimationStates.isJumping);
        
        if (wasGrounded == isGrounded) return;

        characterEffects?.OnGroundStateChange(isGrounded, wasAirborne);
        characterAnimationController?.SetGroundState(isGrounded);

        if (isGrounded)
        {
            //if (umbrellaIsOpen) playerCharacterController?.characterAudioController?.PlaySoundEffect("FootStepLand");

            if (wasAirborne)
            {
                characterSoundEffects?.PlaySoundEffect(characterSoundEffects.heavyLand);
            } else
            {
                characterSoundEffects?.PlaySoundEffect(characterSoundEffects.land);
            }

            wasAirborne = false;
        }

        wasGrounded = isGrounded;
    }
    
    public virtual void CheckRunningState()
    {
        //characterAnimationController?.SetAnimationType(isGrounded ? isMoving ? CharacterAnimationStates.isWalking : CharacterAnimationStates.isIdle : CharacterAnimationStates.isJumping);

        if (isRunning && !isMoving) isRunning = false;

        if (wasRunning == isRunning) return;
        characterAnimationController?.SetRunState(isRunning);
        wasRunning = isRunning;
    }

    public virtual void OnIsMovingChange()
    {
        if (isMoving != wasMoving)
        {
            EnableMoveObjects(isMoving);
            wasMoving = isMoving;
        }
    }

    public virtual void EnableMoveObjects(bool _state)
    {
        for (int i = 0; i < enableOnMove.Length; i++) enableOnMove[i].SetActive(_state);
    }

    public virtual IEnumerator OnAttack(float duration, float attackResetTime)
    {
        EnableDamageDealers(true);

        yield return new WaitForSeconds(duration);

        EnableDamageDealers(false);

        if (attackResetTime > 1 && attackResetTime > duration) yield return new WaitForSeconds(attackResetTime-duration);

        isAttacking = false;
    }

    public virtual void SetWeaponType(int weaponID)
    {
        if (weapons.Length <= weaponID || weapons.Length <= 0 || weapons[0] == null) return;

        for (int i = 0; i < weapons.Length; i++) weapons[i].gameObject.SetActive(i == weaponID);

        activeWeapon = weapons[weaponID];
        activeWeaponID = weaponID;

        activeWeapon?.SetCharacter(this);
    }

    public virtual void EnableDamageDealers(bool state)
    {
        for (int i = 0; i < damageDealers.Length; i++)
        {
            damageDealers[i].AttackStateChange(state);
        }
    }

    public virtual void AdjustHealthPercentage(float ammount)
    {
        healthBar?.AddHealthByPercent(ammount);
    }

    public virtual void SetModelInformation(CharacterModelSetup currentModel)
    {
        CharacterModelSettings modelSettings = currentModel.modelSettings;

        weapons = modelSettings.weapons;
        leftFoot = modelSettings.leftFoot;
        rightFoot = modelSettings.rightFoot;

        if (weapons.Length > 0)
        {
            SetWeaponType(modelSettings.defaultWeaponID);
        }
        else
        {
            activeWeapon = null;
        }

        if (characterAnimationController)
        {
            characterAnimationController.characterAnimator = modelSettings.animator;
            characterAnimationController.headLookAt = modelSettings.headLookAt;
            
            if (characterAnimationController.characterAnimator){
                leftFoot = !leftFoot ? characterAnimationController.characterAnimator.GetBoneTransform(HumanBodyBones.LeftFoot) : leftFoot;
                rightFoot = !rightFoot ? characterAnimationController.characterAnimator.GetBoneTransform(HumanBodyBones.RightFoot) : rightFoot;

                characterAnimationController.characterAnimator.enabled = false;
                characterAnimationController.characterAnimator.enabled = true;

                isHumanoid = characterAnimationController.characterAnimator.isHuman;

                characterFootstepSystem?.SetFootstepSystem(leftFoot, rightFoot);
            }
        }

        if (characterEffects)
        {
            characterEffects.centerPoint = modelSettings.centerPoint;
        }
    }
    public virtual void OnHit(float damageAmount, AttackType hitType)
    {
        if (wasJustHit && gettingHitHaltsAttack || isKnockedOut || gameOverCalled || isBlocking) return;

        wasJustHit = true;

        if (healthBar)
        {
            healthBar?.TakeDamage(damageAmount);

            if (healthBar.GetHealthPercentage()*100 <= 0) OnKnockOut();
        }

        if (hitType == AttackType.High) GameManager.Instance?.cameraRig?.ShakeCamera(.5f, .75f);
        else if (hitType == AttackType.Medium) GameManager.Instance?.cameraRig?.ShakeCamera(.5f, .25f);

        characterSoundEffects?.CallHitSoundEffect(hitType);
        characterEffects?.OnHit();

        takeDamageWaitTimeCO = StartCoroutine(AfterHit());
    }

    public virtual IEnumerator AfterHit()
    {
        yield return new WaitForSeconds(takeDamageWaitTime);
        wasJustHit = false;
    }

    public virtual void OnGameOver(GameOverState gameOverState, bool playerWon = false)
    {
        gameOverCalled = true;
    }

    public virtual void OnKnockOut(float knockOutTime = 1f)
    {
        if (isKnockedOut) return;

        isKnockedOut = true;

        SetBlocking(false);

        if (hideDefaultDisplayOnKO && defaultDisplayObject) defaultDisplayObject.SetActive(false);

        if (destroyOnKnockOut) StartCoroutine(DestroyAfterDelay(knockOutTime));
    }

    public IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        DestroyThisObject();
    }

    public virtual void DestroyThisObject()
    {
        Destroy(gameObject);
    }

    public virtual void SetBlocking(bool state)
    {
        isBlocking = wasJustHit? false : isKnockedOut? false : state;
    }

    /*public virtual void UpdateAnimator(float dt)
    {
        //if (!characterAnimationController.characterAnimator) return;
        //isHumanoid
    }*/
}