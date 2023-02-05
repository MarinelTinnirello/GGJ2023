using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterAnimationController))]
public class CharacterInputController : CharacterTrigger
{
    [Header("NPC Setup")]
    public NPCController NPC;

    [Header("Movement Settings")]
    public MovementType movementType;
    public ActiveAxis movementAxis = ActiveAxis.XY;
    public float speed = 6f;
    public float rotationSpeed = 30f;
    public float jumpHeight = 4.5f;
    public float gravity = -100f;//-9.81f;
    public bool canJump = true;
    private float dashAmmount = 1f;

    private float horizontal;
    private float vertical;

    private Vector3 direction;

    [Header("Power Up Setup")]
    public CharacterPowerTypes currentPowerType;
    public bool enableRotationTimer = true;

    private float changePowerTime = 10f;

    private bool jumpCalled;
    
    private bool isDashing;

    private bool inputEnabled = true;

    public override void Start()
    {
        if (!characterAnimationController) characterAnimationController = GetComponent<CharacterAnimationController>();
        if (!characterVariantController) characterVariantController = GetComponent<CharacterVariantController>();
        if (!characterSoundEffects) characterSoundEffects = GetComponent<CharacterSoundEffects>();
        if (!characterEffects) characterEffects = GetComponent<CharacterEffects>();

        if (characterEffects)
        {
            if (characterAnimationController) characterEffects.characterAnimationController = characterAnimationController;
            if (characterSoundEffects) characterEffects.characterSoundEffects = characterSoundEffects;
        }

        SetWeaponType(defaultWeaponID);

        if (!GameManager.Instance) StartPowerChangeTimer(8.0f, 10.0f);
        
        characterAnimationController?.SetCharacterController(characterController);

        base.Start();
    }

    public override void Update()
    {
        if (GameManager.Instance)
        if (!GameManager.Instance.GameActive()) return;

        horizontal = inputEnabled && movementAxis != ActiveAxis.Y ? Input.GetAxisRaw("Horizontal") : 0;
        vertical = inputEnabled && movementAxis != ActiveAxis.X ? Input.GetAxisRaw("Vertical") : 0;

        direction = new Vector3(horizontal, 0, vertical);//.normalized;
        isMoving = direction.normalized.magnitude >= 0.1f;

        CheckGroundState();

        if (isBlocking)
        {
            horizontal = vertical = 0f;
            isMoving = false;
        }

        attackCalled = false;

        if (isGrounded) jumpCalled = isDashing = false;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        GetInputKeys();

        if (!isDashing && jumpCalled && attackCalled)
        {
            isDashing = true;
            velocity.y = Mathf.Sqrt(jumpHeight/2 * -2f * gravity);
        }
        
        if (attackCalled)
        {
            if (isGrounded)
                characterSoundEffects?.CallComboSoundEffect();
            else
                characterSoundEffects?.CallAttackSoundEffect();
        }

        if (isMoving)
        {
            dashAmmount = isDashing? jumpKickDashAmmount : 1f;

            if (isRunning) dashAmmount = dashAmmount * runSpeedMultiplier;

            moveSpeed = speed * dashAmmount;

            characterController?.Move(direction * speed * dashAmmount * Time.deltaTime);
            
            characterAnimationController?.UpdateCharacterFacingDireciton( horizontal > 0 ? FacingDirection.Right : FacingDirection.Left, true);
            characterAnimationController?.SetSpeed(moveSpeed*0.1f);
        }

        characterAnimationController?.Movement(horizontal, vertical);

        velocity.y += gravity * Time.deltaTime;
        characterController?.Move(velocity * Time.deltaTime);

        //characterAnimationController?.SetAnimationType(isGrounded? isMoving ? CharacterAnimationStates.isWalking : CharacterAnimationStates.isIdle : CharacterAnimationStates.isJumping);
        //characterAnimationController?.SetGroundState(isGrounded);

        if(movementType == MovementType.is3D)
        {
            RotateTowardsMovementDir();
        }

        base.Update();
    }
    public Vector3 MovementInput { 
        get { return CameraRelativeInput(horizontal, vertical); }
    }

    private Vector3 CameraRelativeInput(float inputX, float inputZ)
    {
        Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        Vector3 relativeVelocity = horizontal * right + vertical * forward;

        if (relativeVelocity.magnitude > 1) { relativeVelocity.Normalize(); }

        return relativeVelocity;
    }
    private void RotateTowardsMovementDir()
    {
        if (MovementInput != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(MovementInput), Time.deltaTime * rotationSpeed);
            //transform.rotation = new Quaternion(0f, transform.rotation.y + (horizontal * 1f), 0f, transform.eulerAngles.magnitude);
            //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y+(horizontal * 1f), 0);
        }
    }
    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - new Vector3(transform.position.x, 0, transform.position.z));
        transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, (rotationSpeed * Time.deltaTime) * rotationSpeed);
    }

    private void GetInputKeys()
    {
        if (!inputEnabled) return;

        if (Input.GetButtonUp("StartGamePad"))
        {
            GameManager.Instance?.TogglePause();
        }

        if (Input.GetButtonDown("Block"))
        {
            SetBlocking(true);
        }
        else if (Input.GetButtonUp("Block"))
        {
            SetBlocking(false);
        }

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = isMoving;
        } else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (!isAttacking) CallAttack(GameManager.Instance ? GameManager.Instance.OnPlayerInputAttack() : AttackType.High);
        } else if (Input.GetButtonUp("Fire1"))
        {
            GameManager.Instance?.OnPlayerReleaseAttackButton();
        }
    }

    public void InputActiveState(bool state)
    {
        inputEnabled = state;
    }

    public void StartPowerChangeTimer(float delayStartTime = 8.0f, float repeatTime = 10.0f)
    {
        if (!enableRotationTimer) return;

        changePowerTime = repeatTime;
        InvokeRepeating("SetRandomPowerType", delayStartTime, repeatTime);
    }

    public void EndPowerChangeTimer()
    {
        CancelInvoke("SetRandomPowerType");
    }

    public void Jump()
    {
        if (!isGrounded || !canJump) return;

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        jumpCalled = true;

        characterSoundEffects?.PlaySoundEffect(characterSoundEffects.jump);
    }

    public void CallAttack(AttackType attackType)
    {
        if (wasJustHit && gettingHitHaltsAttack || isKnockedOut) return;

        characterAnimationController?.PreformAttack(attackType);
        activeWeapon?.Attack();

        attackStrenghType = attackType;
        attackCalled = isAttacking = true;

        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(OnAttack(attackTime, attackWaitTime));
    }

    public override IEnumerator OnAttack(float duration, float attackResetTime)
    {
        return base.OnAttack(duration, attackResetTime);
    }

    public void SetRandomPowerType()
    {
        int randomID = (int)currentPowerType;
        int powerCount = System.Enum.GetValues(typeof(CharacterPowerTypes)).Length;

        while (randomID == (int)currentPowerType) randomID = Random.Range(0, powerCount);

        if (randomID > powerCount - 1) randomID = powerCount - 1;

        SetPowerType(randomID);
    }

    public void SetPowerType(int powerTypeID = 0)
    {
        currentPowerType = (CharacterPowerTypes)powerTypeID;
        OnTransform((int)currentPowerType);

        SetWeaponType(powerTypeID);

        //GameManager.Instance?.cameraRig?.SetCameraAngleByID(powerTypeID);
    }

    private void OnTransform(int powerTypeID = 0)
    {
        characterAnimationController?.OnTransform(powerTypeID);
        characterSoundEffects?.CallTransformSounds(powerTypeID);
        characterEffects?.OnTransform(powerTypeID, changePowerTime);
        if(NPC && NPC.isActiveAndEnabled)NPC?.OnTransform(powerTypeID);
    }

    public override void EnableDamageDealers(bool state)
    {
        for (int i = 0; i < damageDealers.Length; i++)
        {
            damageDealers[i].AttackStateChange(state, attackDamageAmount[(int)attackStrenghType], attackStrenghType, attackRadius + (.2f*(int)attackStrenghType) );
        }
    }

    public override void OnKnockOut(float knockOutTime = 1)
    {
        base.OnKnockOut(knockOutTime);

        EndPowerChangeTimer();

        GameManager.Instance?.OnPlayerFail();
    }

    public override void OnGameOver(GameOverState gameOverState, bool playerWon = false)
    {
        base.OnGameOver(gameOverState, playerWon);

        InputActiveState(false);
    }
}
