// # Systems
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class Bringer_Spell : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider.enabled = false;
    }

    public void SpellStart(Vector2 startPosition)
    {
        gameObject.SetActive(true);
        transform.position = startPosition;
    }

    private void AttackRangeActiveTure() // animator
    {
        boxCollider.enabled = true;
    }

    private void SpellExit() // animator
    {
        gameObject.SetActive(false);
        boxCollider.enabled = false;
    }
}
