using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Goblin : WalkMonster
{
    private bool attacked;

    private void Update()
    {
        anim.SetInteger(hashMove, attacked ? 0 : (int)rigid.velocity.x);

        LookAtPlayer();
    }

    protected override void MoveToPlayer()
    {
        playerDistance = playerPos.position.x - transform.position.x;

        if (!attacked)
        {
            rigid.velocity = new Vector2(Mathf.Sign(playerDistance) * maxMoveSpeed, rigid.velocity.y);
        }

        if(Mathf.Abs(playerDistance) < 1 && !attacked)
        {
            rigid.velocity = Vector2.zero;
            attacked = true;
            StartCoroutine(AttackWait());
        }

        // 플레이어의 위치에 따라 플레이어를 바라보게 턴
        if (playerDistance < 0) transform.eulerAngles = turnLeftVec;
        else if (playerDistance > 0) transform.eulerAngles = turnRightVec;
    }

    private IEnumerator AttackWait()
    {
        Attack();
        yield return new WaitForSeconds(attackCoolTime);
        attacked = false;
    }
}
