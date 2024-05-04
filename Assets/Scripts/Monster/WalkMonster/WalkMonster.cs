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

        // 이동할 방향을 랜덤으로 정해주는 코루틴 루프
        StartCoroutine(RandomMove());
    }

    /// <summary>
    /// 눈앞으로 레이를 쏴 플레이어를 감지하고 바라보는 함수
    /// </summary>
    protected void LookAtPlayer()
    {
        if (!attackStart && playerHit)
        {
            // 레이가 플레이어를 감지하면 랜덤무브 코루틴을 끝내고 공격모드 돌입
            attackStart = true;
            playerPos = playerHit.transform;
            StopAllCoroutines();
        }
        else
        {
            // 플레이어를 감지하는 레이 쏘기
            Debug.DrawRay(rigid.position, transform.right * playerRaycastDistance, Color.red, 0, false);
            playerHit = Physics2D.Raycast(transform.position, transform.right, playerRaycastDistance, playerLayer);
        }

        if (attackStart && !isHit && !isAttack) // 플레이어를 감지할 시 이동
        {
            MoveToPlayer();
        }
        else if(!attackStart) // 랜덤이동
        {
            // 앞에 벽이 있으면 멈춤
            isWall = Physics2D.OverlapCircle(wallCheckPos.position, WallCheckRadius, wallLayer);
            rigid.velocity = new Vector2(isWall ? 0 : moveDir, rigid.velocity.y);
        }
    }

    protected virtual void MoveToPlayer()
    {
        // 플레이어와 거리 계산
        playerDistance = playerPos.position.x - transform.position.x;

        // 플레이어와의 거리가 max보다 크다면 플레이어쪽으로 이동, min보다 작으면 플레이어 반대쪽으로 이동하여 적정거리 유지
        // 플레이어가 나보다 왼쪽에 있다면 playerDistance는 의 부호는 -이고 오른쪽에 있다면 +이다.
        if (Mathf.Abs(playerDistance) > maxPlayerDistance) rigid.velocity = new Vector2(Mathf.Sign(playerDistance) * maxMoveSpeed, rigid.velocity.y);
        else if (Mathf.Abs(playerDistance) < minPlayerDistance) rigid.velocity = new Vector2(Mathf.Sign(playerDistance) * -minMoveSpeed, rigid.velocity.y);

        // 플레이어의 위치에 따라 플레이어를 바라보게 턴
        if (playerDistance < 0) transform.eulerAngles = turnLeftVec;
        else if (playerDistance > 0) transform.eulerAngles = turnRightVec;
    }

    /// <summary>
    /// 랜덤으로 이동할 방향을 정해주는 코루틴 루프
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
