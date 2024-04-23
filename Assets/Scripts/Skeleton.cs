using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Skeleton : Enemy
{
    private readonly int hashShield = Animator.StringToHash("IsShield");
    private readonly int hashAttack2 = Animator.StringToHash("Attack2");
    private bool attackRangeCheck1;
    private bool attackRangeCheck2;

    [SerializeField] private GameObject atkRange;
    [SerializeField] private GameObject atk2Range;

    private void Update()
    {
        attackRangeCheck1 = Physics2D.OverlapBox(atkRange.transform.position, atkRange.transform.localScale, 0, playerLayer);

        anim.SetBool(hashWalk, playerCheck);

        LookAtPlayer();

        if (attackRangeCheck1)
        {
            Attack();
        }
    }

    private void AttackRangeActive()
    {
        if(atkRange.activeSelf)
        {
            atkRange.SetActive(false);
        }
        else
        {
            atkRange.SetActive(true);
        }
    }

    private void Attack2RangeActive()
    {
        if (!atk2Range.activeSelf)
        {
            atk2Range.SetActive(false);
        }
        else
        {
            atk2Range.SetActive(true);
        }
    }

    public override void AttackExit()
    {
        base.AttackExit();
        atkRange.SetActive(false);
        atk2Range.SetActive(false);
    }
}
