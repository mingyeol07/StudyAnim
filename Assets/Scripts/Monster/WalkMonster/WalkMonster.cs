using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkMonster : Monster
{
    [Header("WallCheck")]
    [SerializeField] private Transform wallCheckPos;
    private const float WallCheckRadius = 0.1f;
    private bool isWall;

    [Header("PlayerCheck")]
    [SerializeField] private float minPlayerDistance = 4;
    [SerializeField] private float maxPlayerDistance = 5;
    [SerializeField] private float playerRaycastDistance = 5;

    protected override void Start()
    {
        base.Start();

        // �̵��� ������ �������� �����ִ� �ڷ�ƾ ����
        StartCoroutine(RandomMove());
    }

    /// <summary>
    /// �������� ���̸� �� �÷��̾ �����ϰ� �ٶ󺸴� �Լ�
    /// </summary>
    protected void LookAtPlayer()
    {
        if (!attackStart && playerHit)
        {
            // ���̰� �÷��̾ �����ϸ� �������� �ڷ�ƾ�� ������ ���ݸ�� ����
            attackStart = true;
            playerPos = playerHit.transform;
            StopAllCoroutines();
        }
        else
        {
            // �÷��̾ �����ϴ� ���� ���
            Debug.DrawRay(rigid.position, transform.right * playerRaycastDistance, Color.red, 0, false);
            playerHit = Physics2D.Raycast(transform.position, transform.right, playerRaycastDistance, playerLayer);
        }

        if (attackStart && !isHit && !isAttack) // �÷��̾ ������ �� �̵�
        {
            MoveToPlayer();
        }
        else if(!attackStart) // �����̵�
        {
            // �տ� ���� ������ ����
            isWall = Physics2D.OverlapCircle(wallCheckPos.position, WallCheckRadius, wallLayer);
            rigid.velocity = new Vector2(isWall ? 0 : moveDir, rigid.velocity.y);
        }
    }

    protected virtual void MoveToPlayer()
    {
        // �÷��̾�� �Ÿ� ���
        playerDistance = playerPos.position.x - transform.position.x;

        // �÷��̾���� �Ÿ��� max���� ũ�ٸ� �÷��̾������� �̵�, min���� ������ �÷��̾� �ݴ������� �̵��Ͽ� �����Ÿ� ����
        // �÷��̾ ������ ���ʿ� �ִٸ� playerDistance�� �� ��ȣ�� -�̰� �����ʿ� �ִٸ� +�̴�.
        if (Mathf.Abs(playerDistance) > maxPlayerDistance) rigid.velocity = new Vector2(Mathf.Sign(playerDistance) * maxMoveSpeed, rigid.velocity.y);
        else if (Mathf.Abs(playerDistance) < minPlayerDistance) rigid.velocity = new Vector2(Mathf.Sign(playerDistance) * -minMoveSpeed, rigid.velocity.y);

        // �÷��̾��� ��ġ�� ���� �÷��̾ �ٶ󺸰� ��
        if (playerDistance < 0) transform.eulerAngles = turnLeftVec;
        else if (playerDistance > 0) transform.eulerAngles = turnRightVec;
    }

    /// <summary>
    /// �������� �̵��� ������ �����ִ� �ڷ�ƾ ����
    /// </summary>
    private IEnumerator RandomMove()
    {
        int randomDir = Random.Range(-1, 2);

        if (randomDir < 0) transform.eulerAngles = turnLeftVec;
        else if (randomDir > 0) transform.eulerAngles = turnRightVec;

        moveDir = randomDir * maxMoveSpeed;

        yield return new WaitForSeconds(1);

        StartCoroutine(RandomMove());
    }
}
