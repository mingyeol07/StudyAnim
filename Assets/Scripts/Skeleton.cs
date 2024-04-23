using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Skeleton : Enemy
{
    private readonly int hashShield = Animator.StringToHash("IsShield");
    private readonly int hashAttack = Animator.StringToHash("Attack");
    private readonly int hashAttack2 = Animator.StringToHash("Attack2");
    private bool attackRangeCheck1;
    private bool attackRangeCheck2;

    [SerializeField] private bool attackAble;

    [SerializeField] private GameObject atkRange;
    [SerializeField] private GameObject atk2Range;

    private void Update()
    {
        attackRangeCheck1 = Physics2D.OverlapBox(atkRange.transform.position, atkRange.transform.localScale, 0, playerLayer);

        AnimationControl();
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

    protected private void Attack()
    {
        if (!isAttack && !attackAble)
        {
            isAttack = true;
            attackAble = true;
            rigid.velocity = Vector2.zero;
            anim.SetBool(hashAttack, true);
            Invoke("AttackAble", 3f);
        }
    }

    private void AttackAble()
    {
        attackAble = false;
    }

    private void AttackExit()
    {
        isAttack = false;
        anim.SetBool(hashAttack, false);
        atkRange.SetActive(false);
        atk2Range.SetActive(false);
    }
}
