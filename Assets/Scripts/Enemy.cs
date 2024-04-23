using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] private int hp;
   
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform playerPos;
    [SerializeField] private float attackCool;
    
    protected bool playerCheck;
    protected bool isHit;
    protected bool isAttack;
    protected bool attackAble;

    protected Animator anim;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigid;

    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashHit = Animator.StringToHash("Hit");
    protected readonly int hashWalk = Animator.StringToHash("IsWalk");
    private readonly int hashAttack = Animator.StringToHash("Attack");

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    protected void LookAtPlayer()
    {
        Debug.DrawRay(rigid.position, transform.right * 7f, Color.red, 0, false);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 7f, playerLayer);

        if (hit.collider == null && playerCheck == true)
        {
            playerCheck = false;
            playerPos = null;
            rigid.velocity = Vector2.zero;
        }
        else if (hit.collider != null && playerCheck == false)
        {
            playerCheck = true;
            playerPos = hit.transform;
        }

        if (playerCheck && !isHit && !isAttack)
        {
            float distance = playerPos.position.x - transform.position.x;
            if(Mathf.Abs(distance) > 3) rigid.velocity = new Vector2(Mathf.Sign(distance) * moveSpeed, rigid.velocity.y);
            transform.rotation = Quaternion.Euler(0, distance > 0 ? 0 : 180, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack") && !isAttack)
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
            Destroy(this.GetComponent<Enemy>());
        }
        else
        {
            StartCoroutine(Hit());
        }
    }

    protected private void Attack()
    {
        if (!isAttack && !attackAble)
        {
            isAttack = true;
            attackAble = true;
            anim.SetBool(hashAttack, true);
            rigid.velocity = Vector2.zero;
            Invoke("AttackAble", attackCool);
        }
    }

    protected void AttackAble()
    {
        attackAble = false;
    }

    public virtual void AttackExit()
    {
        isAttack = false;
        anim.SetBool(hashAttack, false);
    }
}
