using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using Unity.VisualScripting;

public class Hands : MonoBehaviour
{
    public Transform weaponPivot;
    public Transform shieldPivot;
    public bool isLeft;
    public SpriteRenderer spriter;
    public SpriteRenderer bodyRenderer;
    public float damage;
    SpriteRenderer player;
    [HideInInspector] public bool isGuard;

    Vector3 rightPos = new Vector3(0.127f, -0.116f, 0);
    Vector3 rightPosReverse = new Vector3(-0.153f, 0.117f, 0);

    Vector3 leftPos = new Vector3(-0.15f, -0.25f, 0);
    Vector3 leftPosReverse = new Vector3(-0.25f, -0.25f, 0);

    Vector3 pivotPos = new Vector3(0.192f, -0.2f, 0);
    Vector3 pivotPosReverse = new Vector3(0.13f, -0.25f, 0);

    private Vector3 OriginalRotation;
    private Vector3 OriginalPivotRotation;
    private Vector3 OriginalShieldPosition;

    private float rightAttackReadyDuration;
    public float rightAttackDuration = 0.15f;
    public float leftAttackDuration = 0.20f;

    public float guardDuration = 0.01f;

    public System.Action OnAttackComplete;
    public System.Action OnGuardComplete;

    public Collider2D weaponCollider;
    public Collider2D shieldCollider;

    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    void Awake()
    {
        player = bodyRenderer;
        OriginalRotation = transform.eulerAngles;
        if (weaponPivot == null)
        {
            weaponPivot = transform.parent;
        }
        if (shieldPivot == null)
        {
            shieldPivot = transform.parent;
        }
        OriginalPivotRotation = weaponPivot.eulerAngles;
        OriginalShieldPosition = shieldPivot.localPosition;

        rightAttackReadyDuration = leftAttackDuration - rightAttackDuration;

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
        if (shieldCollider != null)
        {
            shieldCollider.enabled = false;
        }
    }

    void LateUpdate()
    {
        bool isReverse = player.flipX;
        if (isGuard) return;
        if (isLeft)
        {
            transform.localPosition = isReverse ? leftPosReverse : leftPos;
            spriter.flipY = isReverse;
        }
        else
        {
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            weaponPivot.localPosition = isReverse ? pivotPosReverse : pivotPos;
            spriter.flipY = isReverse;
            spriter.sortingOrder = isReverse ? 3 : 1;
        }
    }

    public void Attack()
    {

        hitTargets.Clear();

        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }

        bool isReverse = player.flipX;
        if (!isLeft)
        {
            if (isReverse)
            {
                weaponPivot.DORotate(new Vector3(0, 0, 240), leftAttackDuration, RotateMode.FastBeyond360)
                            .SetEase(Ease.OutQuad)
                            .OnComplete(() =>
                            {
                                weaponPivot.DORotate(OriginalPivotRotation, leftAttackDuration, RotateMode.FastBeyond360)
                                    .SetEase(Ease.InQuad)
                                    .OnComplete(() =>
                                    {
                                        transform.localPosition = rightPosReverse;
                                        OnAttackComplete?.Invoke();
                                        weaponCollider.enabled = false;
                                    });
                            });
            }
            else
            {
                weaponPivot.DORotate(new Vector3(0, 0, 90), rightAttackReadyDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        weaponPivot.DORotate(new Vector3(0, 0, -120), rightAttackDuration, RotateMode.FastBeyond360)
                            .SetEase(Ease.OutQuad)
                            .OnComplete(() =>
                            {
                                weaponPivot.DORotate(OriginalPivotRotation, leftAttackDuration)
                                    .SetEase(Ease.InQuad)
                                    .OnComplete(() =>
                                    {
                                        transform.localPosition = rightPos;
                                        OnAttackComplete?.Invoke();
                                        weaponCollider.enabled = false;
                                    });
                            });
                    });
            }
        }
    }

    public void Guard()
    {
        if (isLeft)
        {
            isGuard = true;
            if (shieldCollider != null)
            {
                shieldCollider.enabled = true;
            }
            bool isReverse = player.flipX;
            if (isReverse)
            {
                shieldPivot.DOLocalMove(OriginalShieldPosition - Vector3.left * 0.08f, guardDuration * 0.5f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        shieldPivot.DOLocalMove(OriginalShieldPosition + Vector3.left * 0.25f + Vector3.up * 0.05f, guardDuration)
                            .SetEase(Ease.OutQuad)
                            .OnComplete(() =>
                            {
                                shieldPivot.DOLocalMove(OriginalShieldPosition, guardDuration * 0.5f)
                                .SetEase(Ease.OutQuad)
                                .OnComplete(() =>
                                {
                                    transform.localPosition = leftPosReverse;
                                    OnGuardComplete?.Invoke();
                                    isGuard = false;
                                    shieldCollider.enabled = false;
                                });
                            });
                    });

            }
            else
            {
                shieldPivot.DOLocalMove(OriginalShieldPosition - Vector3.left * 0.08f, guardDuration * 0.5f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        shieldPivot.DOLocalMove(OriginalShieldPosition + Vector3.left * 0.25f + Vector3.up * 0.05f, guardDuration)
                            .SetEase(Ease.OutQuad)
                            .OnComplete(() =>
                            {
                                shieldPivot.DOLocalMove(OriginalShieldPosition, guardDuration * 0.5f)
                                .SetEase(Ease.OutQuad)
                                .OnComplete(() =>
                                {
                                    transform.localPosition = leftPosReverse;
                                    OnGuardComplete?.Invoke();
                                    isGuard = false;
                                    shieldCollider.enabled = false;
                                });
                            });
                    });
            }
        }
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable == null)
            return;

        GameObject target = collision.gameObject;

        if (hitTargets.Contains(target))
        {
            return;
        }

        hitTargets.Add(target);
        damageable.TakeDamage(damage);
    }
}
