using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected LayerMask playerLayer = 6;
    [SerializeField] protected  LayerMask wallLayer = 8;
    [SerializeField] private int hp;
   
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform playerPos;
    [SerializeField] private float attackCool;
    [SerializeField] private Transform wallCheckPos;
    private const float wallCheckRadius = 0.5f;
    private bool wallCheck;
    private float moveDir;

    private Collider2D playerHit;
    private const float playerCheckRadius = 8f;

    protected bool playerChecking = true;
    protected bool isHit;
    protected bool isAttack;
    protected bool attackAble;

    protected Animator anim;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigid;

    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashHit = Animator.StringToHash("Hit");
    protected readonly int hashMove = Animator.StringToHash("IsMove");
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
        playerHit = Physics2D.OverlapCircle(transform.position, playerCheckRadius, playerLayer);
        wallCheck = Physics2D.OverlapCircle(wallCheckPos.position, wallCheckRadius, wallLayer);

        anim.SetInteger(hashMove, (int)rigid.velocity.x);

        if (wallCheck)
        {
            moveDir = 0;
            rigid.velocity = Vector2.zero;
        }

        if (playerHit == null && playerChecking == true)
        {
            playerChecking = false;
            playerPos = null;
            StartCoroutine(Move());
        }
        else if (playerHit != null && playerChecking == false)
        {
            playerChecking = true;
            playerPos = playerHit.transform;
            StopCoroutine(Move());
        }

        if (playerChecking && !isHit && !isAttack)
        {
            float distance = playerPos.position.x - transform.position.x;
            if (Mathf.Abs(distance) > 5) rigid.velocity = new Vector2(Mathf.Sign(distance) * moveSpeed, rigid.velocity.y);
            else if (Mathf.Abs(distance) < 4) rigid.velocity = new Vector2(Mathf.Sign(distance) * -moveSpeed, rigid.velocity.y);
            transform.rotation = Quaternion.Euler(0, distance > 0 ? 0 : 180, 0);
        }
        else
        {
            rigid.velocity = new Vector2(moveDir, rigid.velocity.y);
        }
    }

    private IEnumerator Move()
    {
        int randomDir = Random.Range(-1, 2);
        moveDir = randomDir * moveSpeed;
        if (randomDir == 0) moveDir = 0; anim.SetInteger(hashMove, (int)moveDir);

        if (randomDir < 0) transform.eulerAngles = new Vector3(0, 180, 0);
        else if (randomDir > 0) transform.eulerAngles = Vector3.zero;

        yield return new WaitForSeconds(1);

        StartCoroutine(Move());
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
