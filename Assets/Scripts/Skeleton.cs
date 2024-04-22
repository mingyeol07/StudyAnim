using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Skeleton : Enemy
{
    [SerializeField] private float findRange;
    [SerializeField] private Transform playerPos;
    [SerializeField] private float moveSpeed;
    private bool isMove;

    private void Update()
    {
        /*
        if (Physics2D.OverlapCircle(transform.position, findRange, playerLayer) != null && isMove)
        {
            isMove = false;
            playerPos = Physics2D.OverlapCircle(transform.position, findRange, playerLayer).transform;
        }
        else if(Physics2D.OverlapCircle(transform.position, findRange, playerLayer) == null && isMove == false)
        {
            isMove = true;
            playerPos = null;
        }

        if (playerPos != null)
        {
            float distance = playerPos.position.x - transform.position.x;
            rigid.velocity = new Vector2((distance > 0 ? 1 : -1) * moveSpeed, rigid.velocity.y);
        }
        else
        {
            rigid.velocity = rigid.velocity;
        }*/
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, findRange);
    }
}
