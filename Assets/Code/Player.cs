using System.Collections;
using System.Collections.Generic;
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

    [Header("Attack")]
    [SerializeField] private float _attackCool;
    [SerializeField] private float _attackComboCool;

    private Rigidbody2D _rb;
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _moveInput = Input.GetAxisRaw("Horizontal");

        _rb.velocity = new Vector2(_moveSpeed * _moveInput, _rb.velocity.y);

        if (_moveInput > 0) { transform.eulerAngles = Vector3.zero; }
        else if (_moveInput < 0) transform.eulerAngles = new Vector3(0, 180, 0);

        _isGrounded = Physics2D.OverlapCircle(_feetPos.position, _checkRadius, _whatIsGround);

        if(_isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            // jump
        }

        if(Input.GetKeyDown(KeyCode.X))
        {
            _anim.SetTrigger("IsAttack");
        }

        AnimCon();
        Attack();

        if(_attackCool > 0) {
            _attackCool -= Time.deltaTime;
        }

        if(_attackComboCool > 0)
        {
            _attackComboCool -= Time.deltaTime;
        }
    }

    private void AnimCon()
    {
        _anim.SetBool("IsRun", Mathf.Approximately(_moveInput, 0) ? false : true);
    }

    private void Attack()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            if(_attackCool <=  0)
            {
                _anim.SetFloat("AttackTree", 0f);
                _anim.SetTrigger("AttackTrigger");
                _attackCool = 0.3f;
            }
            
        }
    }

    private void Move()
    {

    }
}
