using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] private int hp;
   
    [SerializeField] private float findRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform playerPos;
    
    private bool playerCheck;
    private bool isHit;
    protected bool isAttack;

    protected Animator anim;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigid;

    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashHit = Animator.StringToHash("Hit");
    
    private readonly int hashWalk = Animator.StringToHash("IsWalk");

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    protected void AnimationControl()
    {
        anim.SetBool(hashWalk, playerCheck);
    }

    protected void LookAtPlayer()
    {
        if (Physics2D.OverlapCircle(transform.position, findRange, playerLayer) == null && playerCheck == true)
        {
            playerCheck = false;
            playerPos = null;
            rigid.velocity = Vector2.zero;
        }
        else if (Physics2D.OverlapCircle(transform.position, findRange, playerLayer) != null && playerCheck == false)
        {
            playerCheck = true;
            playerPos = Physics2D.OverlapCircle(transform.position, findRange, playerLayer).transform;
        }

        if (playerCheck && !isHit && !isAttack)
        {
            float distance = playerPos.position.x - transform.position.x;
            if(Mathf.Abs(distance) > 3) rigid.velocity = new Vector2(Mathf.Sign(distance) * moveSpeed, rigid.velocity.y);
            transform.rotation = Quaternion.Euler(0, distance > 0 ? 0 : 180, 0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, findRange);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            HpDown();
        }
    }

    private IEnumerator Hit()
    {
        anim.SetTrigger(hashHit);
        isHit = true;
        rigid.velocity = Vector2.zero;
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);

        yield return new WaitForSeconds(0.5f);

        spriteRenderer.color = new Color(1, 1, 1, 1f);
        isHit = false;
    }

    private void HpDown()
    {
        hp--;
        if (hp <= 0)
        {
            anim.SetTrigger(hashDie);
            rigid.velocity = Vector2.zero;
            this.gameObject.layer = 10;
            this.enabled = false;
        }
        else
        {
            StartCoroutine(Hit());
        }
    }
}
