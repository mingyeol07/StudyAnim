using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float _moveSpeed;
    private float _moveInput;

    [Header("Jump")]
    private bool _isGrounded;
    [SerializeField] private Transform _feetPos;
    private float _checkRadius = 0.1f;
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private float _jumpForce;

    [Header("Attack")]
    private bool _isAttack;
    private bool _comboAttack;

    [Header("Roll")]
    private bool _isRolling;
    [SerializeField] float _rollSpeed;

    private Rigidbody2D _rb;
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        ValueSet();

        if (Input.GetKeyDown(KeyCode.X) && !_isRolling)
        {
            Attack();
        }
        else if (_isRolling && !_isAttack)
        {
            _rb.velocity = transform.right * _rollSpeed;
        }

        if (!_isAttack && !_isRolling)
        {
            Move();
            if(_isGrounded )
            {
                if (Input.GetKeyDown(KeyCode.LeftShift)) Roll();
                if (Input.GetKeyDown(KeyCode.Space)) Jump();
            }
        }
    }
    
    private void ValueSet() // 바라보는 방향, 땅에 닿았는지
    {
        _moveInput = Input.GetAxisRaw("Horizontal");
        _anim.SetBool("IsRun", Mathf.Approximately(_moveInput, 0) ? false : true);
        _isGrounded = Physics2D.OverlapCircle(_feetPos.position, _checkRadius, _whatIsGround);

        if(_isAttack) _rb.velocity = Vector3.zero;
    }

    private void Move()
    {
        _rb.velocity = new Vector2(_moveSpeed * _moveInput, _rb.velocity.y);

        if (_moveInput > 0) { transform.eulerAngles = Vector3.zero; }
        else if (_moveInput < 0) transform.eulerAngles = new Vector3(0, 180, 0);
    }

    private void Jump()
    {
        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }

    private void Roll()
    {
        _isRolling = true;
        _anim.SetBool("IsRolling", true); 
    }

    private void RollingExit() // AnimEvent
    {
        _isRolling = false;
        _anim.SetBool("IsRolling", false);
        _rb.velocity = Vector2.zero;
    }

    private void Attack()
    {
        if (_isAttack == false)
        {
            _isAttack = true;
            _anim.SetTrigger("AttackTrigger");
        }
        else if (_isAttack == true && _comboAttack == true)
        {
            _anim.SetBool("AttackCombo", true);
        }
    }

    private void OnComboAttack()
    {
        _comboAttack = true;
    }

    private void AttackExit() // AnimEvent
    {
        _isAttack = false;
        _comboAttack = false;
        _anim.SetBool("AttackCombo", false);
        _anim.SetBool("IsRun", false);
    }
}
