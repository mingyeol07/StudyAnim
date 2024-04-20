using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected int hp;
    [SerializeField] protected Animator animator;

    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashHit = Animator.StringToHash("Hit");

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected private void Attack()
    {

    }

    protected private void OnHit()
    {

    }

    protected private void HpDown()
    {
        hp--;
        if(hp == 0)
        {
            animator.SetTrigger(hashDie);
        }
    }
}
