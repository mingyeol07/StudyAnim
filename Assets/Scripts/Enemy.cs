using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected int hp;
    protected Animator anim;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigid;

    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashAttack = Animator.StringToHash("Attack");
    private readonly int hashShieldt = Animator.StringToHash("IsShield");

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    protected private void Attack()
    {
        
    }

    protected private void OnHit()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            HpDown(collision.GetComponentInParent<Transform>());
        }
    }

    private IEnumerator Hit(Transform player)
    {
        anim.SetTrigger(hashHit);
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);

        yield return new WaitForSeconds(0.5f);

        spriteRenderer.color = new Color(1, 1, 1, 1f);
        rigid.velocity = Vector2.zero;
    }

    private void HpDown(Transform player)
    {
        hp--;
        if (hp <= 0)
        {
            anim.SetTrigger(hashDie);
            this.gameObject.layer = 10;
            this.enabled = false;
        }
        else
        {
            StartCoroutine(Hit(player));
        }
    }
}
