// # Systems
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class Bringer : WalkMonster
{
    [SerializeField] private Bringer_Spell spell;

    private float spellCoolTime;

    protected override void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rigid = GetComponentInParent<Rigidbody2D>();  
        //  playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        LookAtPlayer();

        if (Physics2D.OverlapBox(atkRange.transform.position, atkRange.transform.localScale, 0, playerLayer))
        {
            
        }
        else
        {
            //spell.SpellStart(new Vector2(playerPos.position.x, playerPos.position.y + 1));
        }
    }
}
