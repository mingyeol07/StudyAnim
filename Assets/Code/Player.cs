using UnityEngine;

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
        ValueSet();

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
            anim.SetBool("IsClimbing", false);
            anim.SetBool("IsHanging", false);
    
            spriteRenderer.flipX = false;
        }
        else if (isWall && ((transform.rotation.y == 0 && Input.GetKey(KeyCode.RightArrow))
            || (transform.rotation.y != 0 && Input.GetKey(KeyCode.LeftArrow))))
        {
            if(isHanging)
            {
                anim.SetBool("IsClimbing", true);
                anim.SetBool("IsHanging", false);
                spriteRenderer.flipX = true;
                rigid.velocity = Vector3.down * climbSpeed;
            }
            else
            {
                anim.SetBool("IsHanging", true);
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
            anim.SetBool("IsClimbing", false);
            anim.SetBool("IsHanging", false);
            spriteRenderer.flipX = false;
        }


        if (!isAttack && !isRolling && !isClimbJumping)
        {
            Move();

            if (isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift)) Roll();
                if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool("IsJumping") && !isCrouching) Jump();
                if (Input.GetAxisRaw("Vertical") < 0)
                {
                    isCrouching = true;
                    anim.SetBool("IsCrouching", isCrouching);
                    p_collider2d.offset = new Vector2(p_collider2d.offset.x, -1.9f);
                    p_collider2d.size = new Vector2(p_collider2d.size.x, 1.2f);
                }
                else
                {
                    isCrouching = false;
                    anim.SetBool("IsCrouching", isCrouching);
                    p_collider2d.offset = new Vector2(p_collider2d.offset.x, -1.25f);
                    p_collider2d.size = new Vector2(p_collider2d.size.x, 2.5f);
                }
            }

            if (anim.GetBool("IsJumping")) // fall
            { 
                anim.SetFloat("Velocity", rigid.velocity.y);
                if (rigid.velocity.y < 0)
                {
                    anim.SetBool("IsJumping", !isGrounded);
                }
            }
        }
    }
    
    private void ValueSet() // 바라보는 방향, 땅에 닿았는지
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        anim.SetBool("IsRunning", Mathf.Approximately(moveInput, 0) ? false : true);
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
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
        anim.SetBool("IsJumping", true);
        rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Roll()
    {
        isRolling = true;
        anim.SetBool("IsRolling", true); 
    }

    private void Attack()
    {

        if (isAttack == false)
        {
            isAttack = true;
            anim.SetTrigger("AttackTrigger");
        }
        else if (isAttack == true && comboAttack == true)
        {
            anim.SetBool("AttackCombo", true);
        }
    }

    private void ClimbJump()
    {
        isClimbJumping = true;
        anim.SetTrigger("ClimbJump");
        anim.SetBool("IsJumping", false);
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
        anim.SetBool("IsRolling", false);
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
        anim.SetBool("AttackCombo", false);
        anim.SetBool("IsRunning", false);
    }
    #endregion
}
