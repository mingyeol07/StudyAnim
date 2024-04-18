using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private LayerMask playerLayer;
    private int hp;
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashHit = Animator.StringToHash("Hit");

    protected private void Attack()
    {

    }

    protected private void OnHit()
    {

    }
}
