using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Skeleton : WalkMonster
{
    private void Update()
    {
        anim.SetInteger(hashMove, attackStart ? 1 : (int)rigid.velocity.x);

        LookAtPlayer();

        if (attackStart)
        {
            attackReadyTime += Time.deltaTime;
            if(attackReadyTime > attackCoolTime)
            {
                Attack();
            }
        }
    }
}
