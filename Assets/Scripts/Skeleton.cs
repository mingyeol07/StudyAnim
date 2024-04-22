using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Skeleton : Enemy
{
    private readonly int hashShieldt = Animator.StringToHash("IsShield");
    private bool attack1;
    private bool attack2;
    
    [SerializeField] private GameObject atkRange;
    [SerializeField] private GameObject atk2Range;

    private void Update()
    {
        attack1 = Physics2D.OverlapBox(atkRange.transform.position, atkRange.transform.localScale, 0, playerLayer);
        attack2 = Physics2D.OverlapBox(atk2Range.transform.position, atk2Range.transform.localScale, 0, playerLayer);
        AnimationControl();
        LookAtPlayer();

        if(attack1 || attack2)
        {
            Attack();
        }
    }
}
