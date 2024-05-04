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
        // �÷��̾�� �Ÿ� ���
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
        // �÷��̾��� ��ġ�� ���� �÷��̾ �ٶ󺸰� ��
        if (playerDistance < 0) transform.eulerAngles = turnLeftVec;
        else if (playerDistance > 0) transform.eulerAngles = turnRightVec;
    }
}
