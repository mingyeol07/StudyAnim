using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Skeleton : WalkMonster
{
    private void Update()
    {
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
