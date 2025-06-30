using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Hands : MonoBehaviour
{
    public bool isLeft;
    public SpriteRenderer spriter;
    public SpriteRenderer bodyRenderer;
    SpriteRenderer player;

    Vector3 rightPos = new Vector3(0.037f, -0.013f, 0);
    Vector3 rightPosReverse = new Vector3(-0.153f, 0.117f, 0);

    Vector3 leftPos = new Vector3(-0.15f, -0.25f, 0);
    Vector3 leftPosReverse = new Vector3(-0.25f, -0.25f, 0);

    private Vector3 OriginalRotation;


    void Awake()
    {
        player = bodyRenderer;
        OriginalRotation = transform.eulerAngles;
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
                transform.DORotate(new Vector3(0, 0, 0), 0.15f, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        transform.DORotate(new Vector3(0, 0, 210), 0.15f, RotateMode.FastBeyond360)
                            .SetEase(Ease.OutQuad)
                            .OnComplete(() =>
                            {
                                transform.DORotate(OriginalRotation, 0.15f)
                                    .SetEase(Ease.InQuad)
                                    .OnComplete(() =>
                                    {
                                        transform.localPosition = rightPosReverse;
                                    });
                            });
                    });
            }
            else
            {
                transform.DORotate(new Vector3(0, 0, 90), 0.15f , RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        transform.DORotate(new Vector3(0, 0, -120), 0.15f, RotateMode.FastBeyond360)
                            .SetEase(Ease.OutQuad)
                            .OnComplete(() =>
                            {
                                transform.DORotate(OriginalRotation, 0.15f)
                                    .SetEase(Ease.InQuad)
                                    .OnComplete(() =>
                                    {
                                        transform.localPosition = rightPos;
                                    });
                            });
                    });
            }
        }
    }
}
