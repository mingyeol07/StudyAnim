using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private Transform parent;
    [SerializeField] private float moveSpeed;
    private float moveInput;

    [Header("Jump")]
    private bool isGrounded;
    [SerializeField] private Transform feetPos;
    private float checkRadius = 0.1f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float jumpForce;

    [Header("Attack")]
    [SerializeField] private bool isAttack;
    [SerializeField] private GameObject atkRange;
    private bool comboAttack;

    [Header("Roll")]
    private bool isRolling;
    [SerializeField] float rollSpeed;

    [Header("Crouch")]
    private bool isCrouching;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private GameObject crouch_atkRange;

    [Header("Wall")]
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private Transform wallCheckPos;
    public  bool isWall;

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
            spriteRenderer.flipX = false;
        }
        else if (isWall && ((spriteRenderer.flipX && Input.GetKey(KeyCode.RightArrow))
            || (!spriteRenderer.flipX && Input.GetKey(KeyCode.RightArrow))))
        {
            anim.SetBool("IsClimbing", true);
            spriteRenderer.flipX = true;
            rigid.velocity = Vector3.down * 0.5f;
        }
        else
        {
            anim.SetBool("IsClimbing", false);
            spriteRenderer.flipX = false;
        }


        if (!isAttack && !isRolling)
        {
            Move();

            if (isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift)) Roll();
                if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool("IsJumping")) Jump();
                if (Input.GetAxisRaw("Vertical") < 0)
                {
                    isCrouching = true;
                    anim.SetBool("IsCrouching", isCrouching);
                }
                else
                {
                    isCrouching = false;
                    anim.SetBool("IsCrouching", isCrouching);
                }
            }

            if (anim.GetBool("IsJumping")){  // fall
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

    #region AnimEvent
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
