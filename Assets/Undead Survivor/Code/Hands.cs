using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;

public class Hands : MonoBehaviour
{
    public Transform weaponPivot;
    public bool isLeft;
    public SpriteRenderer spriter;
    public SpriteRenderer bodyRenderer;
    SpriteRenderer player;

    Vector3 rightPos = new Vector3(0.127f, -0.116f, 0);
    Vector3 rightPosReverse = new Vector3(-0.153f, 0.117f, 0);

    Vector3 leftPos = new Vector3(-0.15f, -0.25f, 0);
    Vector3 leftPosReverse = new Vector3(-0.25f, -0.25f, 0);

    Vector3 pivotPos = new Vector3(0.192f, -0.2f, 0);
    Vector3 pivotPosReverse = new Vector3(0.13f, -0.25f, 0);

    private Vector3 OriginalRotation;
    private Vector3 OriginalPivotRotation;

    private float rightAttackReadyDuration;
    public float rightAttackDuration = 0.15f;
    public float leftAttackDuration = 0.20f;

    public System.Action OnAttackComplete;

    void Awake()
    {
        player = bodyRenderer;
        OriginalRotation = transform.eulerAngles;
        if (weaponPivot == null)
        {
            weaponPivot = transform.parent;
        }
        OriginalPivotRotation = weaponPivot.eulerAngles;

        rightAttackReadyDuration = leftAttackDuration - rightAttackDuration;
    }

    void LateUpdate()
    {
        bool isReverse = player.flipX;
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
                                    });
                            });
                    });
            }
        }
    }
}
