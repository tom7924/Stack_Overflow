using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;


public class Enemy : MonoBehaviour, IDamageable
{
    public float speed;
    public Rigidbody2D target;

    public float health;

    bool isLive = true;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!isLive)
        {
            return;
        }
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.linearVelocity = Vector2.zero;
    }


    void LateUpdate()
    {
        if (!isLive)
        {
            return;
        }
        spriter.flipX = target.position.x < rigid.position.x;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            isLive = false;
            rigid.simulated = false;
            anim.SetBool("Dead", true);

            Dead();
        }       
        else
        {
            anim.SetTrigger("Hit");
        }

        
        
    }

   
    void Dead()
    {
        gameObject.SetActive(false);
    }
}
