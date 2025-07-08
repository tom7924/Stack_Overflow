using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;


public class Enemy : MonoBehaviour, IDamageable
{
    public float speed;
    public Rigidbody2D target;
    public float health;
    public float knockbackrange;
    bool isLive = true;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    Collider2D coll;

    WaitForFixedUpdate wait;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        wait = new WaitForFixedUpdate();
        
        anim.SetTrigger("Hit");
        anim.ResetTrigger("Hit");
    }

    void FixedUpdate()
    {
        bool isHitState = anim.GetCurrentAnimatorStateInfo(0).IsName("Hit");

        if (!isLive || isHitState)
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
            coll.enabled = false;
            rigid.simulated = false;
            anim.SetBool("Dead", true);
        }
        else
        {
            anim.SetTrigger("Hit");
        }
        StartCoroutine("KnockBack");
    }

    IEnumerator KnockBack()
    {
        yield return wait;

        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * knockbackrange, ForceMode2D.Impulse);

    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}
