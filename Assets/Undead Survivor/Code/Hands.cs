using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hands : MonoBehaviour
{
    public bool isLeft;
    public SpriteRenderer spriter;
    public SpriteRenderer bodyRenderer;
    SpriteRenderer player;

    Vector3 rightPos = new Vector3(0.35f, -0.15f, 0);
    Vector3 rightPosReverse = new Vector3(0.1f, -0.15f, 0);

        Vector3 leftPos = new Vector3(-0.15f, -0.25f, 0);
        Vector3 leftPosReverse = new Vector3(-0.25f, -0.25f, 0);




    void Awake()
    {
        player = bodyRenderer;
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
}
