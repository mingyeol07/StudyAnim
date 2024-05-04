using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : WalkMonster
{
    private void Update()
    {
        LookAtPlayer();
    }

    protected override void MoveToPlayer()
    {
        // 플레이어와 거리 계산
        playerDistance = playerPos.position.x - transform.position.x;

        if (playerDistance < 1)
        {
            attackReadyTime += Time.deltaTime;
            Attack();
        }
        else if (attackReadyTime >= attackCoolTime)
        {
            rigid.velocity = new Vector2(Mathf.Sign(playerDistance) * maxMoveSpeed, rigid.velocity.y);
        }
        // 플레이어의 위치에 따라 플레이어를 바라보게 턴
        if (playerDistance < 0) transform.eulerAngles = turnLeftVec;
        else if (playerDistance > 0) transform.eulerAngles = turnRightVec;
    }
}
