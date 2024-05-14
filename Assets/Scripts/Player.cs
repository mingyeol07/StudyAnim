using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Lumin;
// 300줄 코드 수술 드가자^^lqkf
// 정리 필요
public class Player : MonoBehaviour
{
    // Collider 
    [SerializeField] private BoxCollider2D playerCollider;
    private Vector2 standColliderOffset = new Vector2(-0.35f, -1.25f);
    private Vector2 standColliderSize = new Vector2(1.5f, 2.5f);
    private Vector2 crouchColliderOffset = new Vector2(-0.35f, -1.9f);
    private Vector2 crouchColliderSize =  new Vector2(1.5f, 1.2f);

    // Layer
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;

    [Header("Stat")]
    [SerializeField] private int hp;

    [Header("Move")]
    [SerializeField] private Transform parent;
    [SerializeField] private float moveSpeed;
    private float moveInput;

    [Header("Jump")]
    [SerializeField] private Transform feetPos;
    [SerializeField] private float jumpForce;
    private const float checkRadius = 0.1f;

    [Header("Attack")]
    [SerializeField] private GameObject atkRange;
    private bool comboAttack;

    [Header("Roll")]
    [SerializeField] float rollSpeed;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private GameObject crouch_atkRange;
    
    [Header("Wall")]
    [SerializeField] private Transform wallCheckPos;

    // climb = 벽대고 스르륵
    [Header("Climb")]
    [SerializeField] private Transform climbJumpExitPos;
    private const float climbSpeed = 0.5f;

    [Header("Hang")]
    [SerializeField] private Transform hangCheckPos;
    private const float hangSpeed = 0.1f;
    
    [Header("Hit")]
    [SerializeField] private float nuckBackRange;

    private bool actionAble;

    private bool isAttack;
    private bool isHit;
    private bool isWall;
    private bool isCrouching;
    private bool isRolling;
    private bool isHanging;
    private bool isClimbJumping;
    private bool isGrounded;

    // hashs
    private readonly int hashIsClimbing = Animator.StringToHash("IsClimbing");
    private readonly int hashIsHanging = Animator.StringToHash("IsHanging");
    private readonly int hashIsJumping = Animator.StringToHash("IsJumping");
    private readonly int hashIsCrouching = Animator.StringToHash("IsCrouching");
    private readonly int hashIsRunning = Animator.StringToHash("IsRunning");
    private readonly int hashIsRolling = Animator.StringToHash("IsRolling");
    private readonly int hashAttackCombo = Animator.StringToHash("AttackCombo");
    private readonly int hashClimbJump = Animator.StringToHash("ClimbJump");
    private readonly int hashAttackTrigger = Animator.StringToHash("AttackTrigger");
    private readonly int hashIsHit = Animator.StringToHash("IsHit");
    private readonly int hashDieTrigger = Animator.StringToHash("Die");

    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponentInParent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        actionAble = !isAttack && !isRolling && !isHit && !isClimbJumping;
        if (actionAble) Move();

        if (!isClimbJumping) ValueSet();
        else rigid.velocity = Vector2.up * hangSpeed;

        GroundHandle();
        WallHandle();
    }

    #region Handle
    private void GroundHandle()
    {
        if (isGrounded)
        {
            if (!isRolling && !anim.GetBool(hashIsJumping) && !isHit && !isClimbJumping)
            {
                if (Input.GetKeyDown(KeyCode.X)) Attack();

                if (!isAttack)
                {
                    if (Input.GetKeyDown(KeyCode.LeftShift)) Roll();

                    if (Input.GetKeyDown(KeyCode.Space)) Jump();

                    Crouch();
                }
            }
            if (isRolling && !isAttack) rigid.velocity = transform.right * rollSpeed; // rolling
        }
    }

    private void WallHandle()
    {
        if (!isGrounded && isWall && (transform.rotation.y == 0 ? Input.GetKey(KeyCode.RightArrow) : Input.GetKey(KeyCode.LeftArrow)))
        {
            if (isHanging) // hangCheckPos에 ground가 있을 때
            {
                Hanging();
            }
            else // 매달리기
            {
                ClimbDown();
            }
        }
        else
        {
            anim.SetBool(hashIsClimbing, false);
            anim.SetBool(hashIsHanging, false);
            spriteRenderer.flipX = false;
        }
    }
    #endregion

    private void ValueSet()
    {
        AnimatorSetting();
        LayerCheck();
        Fall();
    }

    private void AnimatorSetting()
    {
        anim.SetBool(hashIsRunning, Mathf.Approximately(moveInput, 0) ? false : true);
        anim.SetBool(hashIsHit, isHit);
        anim.SetBool(hashIsHanging, !isHanging);
        anim.SetBool(hashIsCrouching, isCrouching);
        anim.SetBool(hashIsRolling, isRolling);
        anim.ResetTrigger(hashClimbJump);
    }

    private void LayerCheck()
    {
        isGrounded = Physics2D.OverlapBox(feetPos.position, new Vector2(1.5f, 0.1f), 0, whatIsGround);
        isWall = Physics2D.OverlapCircle(wallCheckPos.position, checkRadius, whatIsWall);
        isHanging = Physics2D.OverlapCircle(hangCheckPos.position, checkRadius, whatIsWall);
    }

    private void Move()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        rigid.velocity = new Vector2((isCrouching? crouchSpeed : moveSpeed) * moveInput, rigid.velocity.y);

        if (moveInput > 0)
        {
            parent.eulerAngles = Vector3.zero;
        }
        else if (moveInput < 0)
        {
            parent.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    private void Crouch()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            isCrouching = true;
            playerCollider.offset = crouchColliderOffset;
            playerCollider.size = crouchColliderSize;
        }
        else
        {
            isCrouching = false;
            playerCollider.offset = standColliderOffset;
            playerCollider.size = standColliderSize;
        }
    }

    private void Jump()
    {
        anim.SetFloat("Velocity", 0);
        anim.SetBool(hashIsJumping, true);
       
        rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Fall()
    {
        if (anim.GetBool(hashIsJumping) || rigid.velocity.y < 0) // fall
        {
            anim.SetFloat("Velocity", rigid.velocity.y);
            anim.SetBool(hashIsJumping, !isGrounded);
        }
    }

    private void Roll()
    {
        isRolling = true;
    }

    private void Attack()
    {
        if (isAttack == false)
        {
            isAttack = true;
            rigid.velocity = Vector2.zero;
            anim.SetTrigger(hashAttackTrigger);
        }
        else if (isAttack == true && comboAttack == true)
        {
            anim.SetBool(hashAttackCombo, true);
        }
    }

    private void Hanging()
    {
        anim.SetBool(hashIsClimbing, true);
        spriteRenderer.flipX = true;
        rigid.velocity = Vector3.down * climbSpeed;
    }

    private void ClimbDown()
    {
        rigid.velocity = Vector2.up * hangSpeed;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClimbJump();
        }
    }

    private void ClimbJump()
    {
        isClimbJumping = true;
        anim.SetTrigger(hashClimbJump);
        anim.SetBool(hashIsJumping, false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyAttack") && !isRolling && !isHit)
        {
            hp--;
            if (hp <= 0)
            {
                anim.SetTrigger(hashDieTrigger);
                rigid.velocity = Vector2.zero;
                Destroy(this);
            }
            else
            {
                StartCoroutine(Hit(collision.transform));
            }
        }
    }

    private IEnumerator Hit(Transform enemyPos)
    {
        isHit = true;
        isAttack = false;
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        anim.SetTrigger(hashIsHit);

        if (enemyPos.position.x > transform.position.x) { parent.eulerAngles = Vector3.zero; }
        else if (enemyPos.position.x < transform.position.x) parent.eulerAngles = new Vector3(0, 180, 0);

        rigid.velocity = new Vector2(parent.rotation.y == 0 ? -nuckBackRange : nuckBackRange, rigid.velocity.y + nuckBackRange);

        yield return new WaitForSeconds(0.5f);

        isHit = false;
        spriteRenderer.color = new Color(1, 1, 1, 1f);
    }

    #region AnimEvent
    private void ClimbExit() // animEvent
    {
        parent.position = climbJumpExitPos.position;
        isClimbJumping = false;
    }

    private void RollingExit() // AnimEvent
    {
        isRolling = false;
        Physics2D.IgnoreLayerCollision(6, 9, false);
        rigid.velocity = Vector2.zero;
    }

    private void OnComboAttack() // AnimEvent
    {
        comboAttack = true;
    }

    private void AttackExit() // AnimEvent idle이 시작될 때 호출
    {
        isAttack = false;
        comboAttack = false;
        anim.SetBool(hashAttackCombo, false);
        anim.SetBool(hashIsRunning, false);
    }

    private void AttackColl()
    {
        if (atkRange.activeSelf) atkRange.SetActive(false);
        else atkRange.SetActive(true);
    }

    private void CrouchAttackColl()
    {
        if (crouch_atkRange.activeSelf) crouch_atkRange.SetActive(false);
        else crouch_atkRange.SetActive(true);
    }
    #endregion
}
