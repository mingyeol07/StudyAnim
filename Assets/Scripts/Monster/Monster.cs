using System.Collections;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

// 적의 최상위 클래스, 여러가지 변수들과 Hit, Attack 함수
public class Monster : MonoBehaviour
{
    [Header("Layer")]
    [SerializeField] protected LayerMask playerLayer = 6;
    [SerializeField] protected  LayerMask wallLayer = 8;

    protected bool playerChecking = true;
    protected bool attackStart = false;

    [Header("Stat")]
    [SerializeField] private int hp;

    [Header("Move")]
    [SerializeField] protected float maxMoveSpeed;
    [SerializeField] protected float minMoveSpeed;
    protected Vector3 turnRightVec = new Vector3(0, 0, 0);
    protected Vector3 turnLeftVec = new Vector3(0, 180, 0);
    protected float moveDir;
    protected float playerDistance;

    [Header("PlayerCheck")]
    [SerializeField] protected Transform playerPos;
    protected RaycastHit2D playerHit;

    [Header("Attack")]
    [SerializeField] protected float attackCoolTime;
    [SerializeField] private GameObject atkRange;
    protected float attackReadyTime;
    protected bool isAttack;

    protected bool isHit;

    protected Animator anim;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigid;

    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashHit = Animator.StringToHash("Hit");
    protected readonly int hashMove = Animator.StringToHash("IsMove");
    protected readonly int hashAttack = Animator.StringToHash("Attack");

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    #region Hit
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack") && !isAttack)
        {
            HpDown();
        }
    }

    private void HpDown()
    {
        hp--;
        if (hp <= 0)
        {
            anim.SetTrigger(hashDie);
            rigid.velocity = Vector2.zero;
            gameObject.layer = 10;
            Destroy(this);
        }
        else
        {
            if(!attackStart)
            {
                attackStart = true;
            }
            StartCoroutine(Hit());
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
    #endregion

    #region AttackStart & Exit
    protected virtual void Attack()
    {
        if (!isAttack)
        {
            isAttack = true;
            anim.SetTrigger(hashAttack);
            rigid.velocity = Vector2.zero;
            attackReadyTime = 0;
        }
    }

    public virtual void AttackExit()
    {
        isAttack = false;
        atkRange.SetActive(false);
    }

    private void AttackRangeActive()
    {
        atkRange.SetActive(!atkRange.activeSelf);
    }
    #endregion
}
