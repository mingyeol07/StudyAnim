using System.Collections;
using UnityEngine;
using UnityEngine.Lumin;

// 정리 필요
public class Player : MonoBehaviour
{
    [SerializeField] private BoxCollider2D p_collider2d;

    [Header("Move")]
    [SerializeField] private Transform parent;
    [SerializeField] private float moveSpeed;
    private float moveInput;

    [Header("Jump")]
    [SerializeField] private Transform feetPos;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float jumpForce;
    private bool isGrounded;
    private float checkRadius = 0.1f;

    [Header("Attack")]
    [SerializeField] private bool isAttack;
    [SerializeField] private GameObject atkRange;
    private bool comboAttack;

    [Header("Roll")]
    [SerializeField] float rollSpeed;
    private bool isRolling;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private GameObject crouch_atkRange;
    private bool isCrouching;

    [Header("Wall")]
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private Transform wallCheckPos;
    [SerializeField] private Transform hangCheckPos;
    [SerializeField] private Transform climbJumpExitPos;
    [SerializeField] private float climbSpeed;
    [SerializeField] private float hangSpeed;
    private bool isClimbJumping;
    private bool isHanging;
    private bool isWall;

    [Header("Hit")]
    private bool isHit;

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
    private readonly int hashHitTrigger = Animator.StringToHash("IsHit");

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
        if (!isClimbJumping)
        {
            ValueSet();
        }

        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.X) && !isRolling)
            {
                Attack();
            }
            else if (isRolling && !isAttack) // rolling
            {
                rigid.velocity = transform.right * rollSpeed;
            }
            anim.SetBool(hashIsClimbing, false);
            anim.SetBool(hashIsHanging, false);
    
            spriteRenderer.flipX = false;
        }
        else if (isWall && ((transform.rotation.y == 0 && Input.GetKey(KeyCode.RightArrow))
            || (transform.rotation.y != 0 && Input.GetKey(KeyCode.LeftArrow))))
        {
            if(isHanging)
            {
                anim.SetBool(hashIsClimbing, true);
                anim.SetBool(hashIsHanging, false);
                spriteRenderer.flipX = true;
                rigid.velocity = Vector3.down * climbSpeed;
            }
            else
            {
                anim.SetBool(hashIsHanging, true);
                rigid.velocity = Vector2.up * hangSpeed;
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    ClimbJump();
                }
            }
        }
        else if(isClimbJumping)
        {
            rigid.velocity = Vector2.up * hangSpeed;
        }
        else
        {
            anim.SetBool(hashIsClimbing, false);
            anim.SetBool(hashIsHanging, false);
            spriteRenderer.flipX = false;
        }


        if (!isAttack && !isRolling && !isClimbJumping && !isHit)
        {
            Move();

            if (isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift)) Roll();
                if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool(hashIsJumping) && !isCrouching) Jump();
                if (Input.GetAxisRaw("Vertical") < 0)
                {
                    isCrouching = true;
                    anim.SetBool(hashIsCrouching, isCrouching);
                    p_collider2d.offset = new Vector2(p_collider2d.offset.x, -1.9f);
                    p_collider2d.size = new Vector2(p_collider2d.size.x, 1.2f);
                }
                else
                {
                    isCrouching = false;
                    anim.SetBool(hashIsCrouching, isCrouching);
                    p_collider2d.offset = new Vector2(p_collider2d.offset.x, -1.25f);
                    p_collider2d.size = new Vector2(p_collider2d.size.x, 2.5f);
                }
            }

            // bug fix
            if (anim.GetBool(hashIsJumping) || rigid.velocity.y < 0) // fall
            {
                anim.SetFloat("Velocity", rigid.velocity.y);
                if (anim.GetFloat("Velocity") < 0)
                {
                    anim.SetBool(hashIsJumping, !isGrounded);
                }
            }
            
        }
    }
    
    private void ValueSet() // 바라보는 방향, 땅에 닿았는지
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        anim.SetBool(hashIsRunning, Mathf.Approximately(moveInput, 0) ? false : true);
        isGrounded = Physics2D.OverlapBox(feetPos.position, new Vector2(1.5f, 0.1f), 0, whatIsGround);
        isWall = Physics2D.OverlapCircle(wallCheckPos.position, checkRadius, whatIsWall);
        isHanging = Physics2D.OverlapCircle(hangCheckPos.position, checkRadius, whatIsWall);

        if (isAttack) rigid.velocity = Vector3.zero;
    }

    private void Move()
    {
        rigid.velocity = new Vector2((isCrouching? crouchSpeed : moveSpeed) * moveInput, rigid.velocity.y);
        
        if (moveInput > 0) { parent.eulerAngles = Vector3.zero; }
        else if (moveInput < 0) parent.eulerAngles = new Vector3(0, 180, 0);
    }

    private void Jump()
    {
        anim.SetBool(hashIsJumping, true);
        rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Roll()
    {
        isRolling = true;
        anim.SetBool(hashIsRolling, true); 
    }

    private void Attack()
    {

        if (isAttack == false)
        {
            isAttack = true;
            anim.SetTrigger(hashAttackTrigger);
        }
        else if (isAttack == true && comboAttack == true)
        {
            anim.SetBool(hashAttackCombo, true);
        }
    }

    private void ClimbJump()
    {
        isClimbJumping = true;
        anim.SetTrigger(hashClimbJump);
        anim.SetBool(hashIsJumping, false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            
            StartCoroutine(Hit());
        }
    }
    private IEnumerator Hit()
    {
        Physics2D.IgnoreLayerCollision(6, 9, true);
        anim.SetTrigger(hashHitTrigger);
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        rigid.velocity = new Vector2(transform.rotation.y == 0 ? -jumpForce : jumpForce , rigid.velocity.y + jumpForce);
        isHit = true;

        yield return new WaitForSeconds(0.5f);

        Physics2D.IgnoreLayerCollision(6, 9, false);
        spriteRenderer.color = new Color(1, 1, 1, 1f);
        isHit = false;
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
        anim.SetBool(hashIsRolling, false);
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
    #endregion
}
