using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected int hp;

    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashHit = Animator.StringToHash("Hit");

    protected private void Attack()
    {

    }

    protected private void OnHit()
    {

    }
}
