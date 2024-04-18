using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Skeleton : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float findRange;
    [SerializeField] private Transform playerPos;
    [SerializeField] private float moveSpeed;
    private bool isMove;
    Rigidbody2D rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        isMove = Physics2D.OverlapCircle(transform.position, findRange, playerLayer);

        if (isMove == true)
        {
            playerPos = Physics2D.OverlapCircle(transform.position, findRange, playerLayer).transform;
        }
        else
        {
            playerPos = null;
        }

        if(playerPos != null)
        {
            float distance = playerPos.position.x - transform.position.x;
            rigid.velocity = new Vector2(distance > 0 ? 1 : -1 * moveSpeed, rigid.velocity.y);
        }
        else
        {
            rigid.velocity = rigid.velocity;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, findRange);
    }
}
