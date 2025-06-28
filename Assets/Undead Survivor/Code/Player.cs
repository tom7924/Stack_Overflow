using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    [Header("Movement")]
    public Vector2 inputVec;
    public float speed;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldownDuration = 1f;


    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    private bool isDashing = false;
    private Vector2 dashDirection;
    private float dashRemainingTime;
    private float dashCooldownTimer;
    private Vector2 lastMoveDirection = Vector2.down;
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
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
    }

    void OnDash(InputValue value)
    {
        Debug.Log("OnDash 호출됨! isPressed: " + value.isPressed);
        if (value.isPressed && CanDash())
        {
            Debug.Log("대시 시작!");
            StartDash();   
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

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    void LateUpdate()
    {
        anim.SetFloat("Speed", inputVec.magnitude);
        if (inputVec.x != 0) {
            spriter.flipX = inputVec.x < 0;
        }
    }

}

