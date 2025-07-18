using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;


public class Player : MonoBehaviour
{
    [Header("Movement")]
    public Vector2 inputVec;
    public float speed;
    private bool isBackstepping = false;
    private bool facingRight = true;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldownDuration = 1f;

    [Header("Attack")]
    public Hands weaponAttack;
    public float attackCooldownDuration = 1f;

    [Header("Guard")]
    public Hands shieldGuard;
    public float guardCooldownDuration = 3f;


    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    PlayerInput playerInput;
    InputAction shiftAction;

    private bool isDashing = false;
    private Vector2 dashDirection;
    private float dashRemainingTime;
    private float dashCooldownTimer;
    private Vector2 lastMoveDirection = Vector2.down;

    private bool isAttacking = false;
    private float attackRemainingTime;
    private float attackCooldownTimer;

    private bool isGuard = false;
    private float guardRemainingTime;
    private float guardCooldownTimer;



    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        Hands[] allHands = GetComponentsInChildren<Hands>();

        foreach (var h in allHands)
        {
            if (h.isLeft)
                shieldGuard = h;
            else
                weaponAttack = h;
        }

        if (weaponAttack != null)
            weaponAttack.OnAttackComplete += OnWeaponAttackComplete;

        if (shieldGuard != null)    
            shieldGuard.OnGuardComplete  += OnShieldGuardComplete;


        facingRight = !spriter.flipX;

        playerInput = GetComponent<PlayerInput>();
        shiftAction = playerInput.actions["Shift"];

        shiftAction.performed += OnShiftPerformed;
        shiftAction.canceled += OnShiftCanceled;
    }

    void FixedUpdate()
    {
        UpdateTimers();

        if (isDashing)
        {
            Vector2 dashVec = dashDirection * dashSpeed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + dashVec);

            if (dashRemainingTime <= 0)
            {
                EndDash();
            }
        }
        else
        {
            Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);

            if (inputVec != Vector2.zero)
            {
                lastMoveDirection = inputVec.normalized;
            }
        }
    }

    void UpdateTimers()
    {
        if (isDashing)
        {
            dashRemainingTime -= Time.fixedDeltaTime;
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.fixedDeltaTime;
        }

        if (isAttacking)
        {
            attackRemainingTime -= Time.fixedDeltaTime;
        }

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.fixedDeltaTime;
        }

        if (isGuard)
        {
            guardRemainingTime -= Time.fixedDeltaTime;
        }

        if (guardCooldownTimer > 0)
        {
            guardCooldownTimer -= Time.fixedDeltaTime;
        }
    }

    void OnDash(InputValue value)
    {
        Debug.Log("OnDash ȣ���! isPressed: " + value.isPressed);
        if (value.isPressed && CanDash())
        {
            Debug.Log("��� ����!");
            StartDash();
        }

    }

    void OnAttack(InputValue value)
    {
        if (value.isPressed && weaponAttack != null && CanAttack())
        {
            Debug.Log($"Onattack isPressed = {value.isPressed}");
            StartAttack();
            weaponAttack.Attack();
        }

    }

    void OnGuard(InputValue value)
    {
        if (value.isPressed && shieldGuard != null && CanGuard())
        {
            Debug.Log($"Onguard isPressed = {value.isPressed}");
            StartGuard();
            shieldGuard.Guard();
        }
    }

    bool CanDash()
    {
        return !isDashing && dashCooldownTimer <= 0;
    }

    void StartDash()
    {
        isDashing = true;
        dashRemainingTime = dashDuration;

        if (inputVec != Vector2.zero)
        {
            dashDirection = inputVec.normalized;
        }
        else
        {
            dashDirection = lastMoveDirection;
        }
    }

    void EndDash()
    {
        isDashing = false;
        dashCooldownTimer = dashCooldownDuration;
    }

    bool CanAttack()
    {
        return !isAttacking && attackCooldownTimer <= 0;
    }

    void StartAttack()
    {
        isAttacking = true;
        attackRemainingTime = weaponAttack.leftAttackDuration;
    }

    void EndAttack()
    {
        isAttacking = false;
        attackCooldownTimer = attackCooldownDuration;
    }

    

    bool CanGuard()
    {
        return !isGuard && guardCooldownTimer <= 0;
    }

    void StartGuard()
    {
        isGuard = true;
        guardRemainingTime = shieldGuard.leftAttackDuration;
    }

    void EndGuard()
    {
        Debug.Log(">> EndGuard() 실행, isGuard→false, cooldown 시작");
        isGuard = false;
        guardCooldownTimer = guardCooldownDuration;
    }

    void OnShieldGuardComplete()
    {
        Debug.Log(">> OnShieldGuardComplete 호출!");
        EndGuard();
    }

    void OnWeaponAttackComplete()
    {
        EndAttack();
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    void OnDestroy()
    {
        if (shiftAction != null)
        {
            shiftAction.performed -= OnShiftPerformed;
            shiftAction.canceled -= OnShiftCanceled;
        }

        if (weaponAttack != null)
        {
            weaponAttack.OnAttackComplete -= OnWeaponAttackComplete;
        }
    }

    void OnShiftPerformed(InputAction.CallbackContext context)
    {
        isBackstepping = true;
    }

    void OnShiftCanceled(InputAction.CallbackContext context)
    {
        isBackstepping = false;

        if (inputVec.x != 0)
        {
            UpdateFacingDirection();
        }
    }

    void LateUpdate()
    {
        anim.SetFloat("Speed", inputVec.magnitude);
        if (inputVec.x != 0 && !isBackstepping)
        {
            UpdateFacingDirection();
        }
    }

    private void UpdateFacingDirection()
    {
        if (inputVec.x > 0)
        {
            facingRight = true;
            spriter.flipX = false;
        }
        else if (inputVec.x < 0)
        {
            facingRight = false;
            spriter.flipX = true;
        }
    }
}

