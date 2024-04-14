using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed;
    private float moveInput;

    [Header("Jump")]
    private bool isGrounded;
    [SerializeField] private Transform feetPos;
    private float checkRadius = 0.1f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float jumpForce;

    [Header("Attack")]
    private bool isAttack;
    private bool comboAttack;

    [Header("Roll")]
    private bool isRolling;
    [SerializeField] float rollSpeed;

    private Rigidbody2D rigid;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        ValueSet();

        if (Input.GetKeyDown(KeyCode.X) && !isRolling && isGrounded)
        {
            Attack();
        }
        else if (isRolling && !isAttack)
        {
            rigid.velocity = transform.right * rollSpeed;
        }

        if (!isAttack && !isRolling)
        {
            Move();
            if(isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift)) Roll();
                if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool("IsJumping")) Jump();
            }

            if (anim.GetBool("IsJumping")){
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

        if(isAttack) rigid.velocity = Vector3.zero;
    }

    private void Move()
    {
        rigid.velocity = new Vector2(moveSpeed * moveInput, rigid.velocity.y);

        if (moveInput > 0) { transform.eulerAngles = Vector3.zero; }
        else if (moveInput < 0) transform.eulerAngles = new Vector3(0, 180, 0);
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

    private void AttackExit() // AnimEvent
    {
        isAttack = false;
        comboAttack = false;
        anim.SetBool("AttackCombo", false);
        anim.SetBool("IsRunning", false);
    }
    #endregion
}
