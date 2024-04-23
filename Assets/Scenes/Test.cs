using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

// player�� enemy ������ �Ÿ��� ���ϴ� �Լ����� �ִ� ���̺귯��
public class Test : MonoBehaviour
{
    public float PlayerToEnemyDistanceX(Transform player, Transform enemy)
    {
        float distance = Mathf.Abs(player.position.x - enemy.position.x);
        return distance;
    }
    public float PlayerToEnemyDistanceY(Transform player, Transform enemy)
    {
        float distance = Mathf.Abs(player.position.y - enemy.position.y);
        return distance;
    }
    public float PlayerToEnemyDistanceZ(Transform player, Transform enemy)
    {
        float distance = Mathf.Abs(player.position.z - enemy.position.z);
        return distance;
    }
    public Vector3 PositionOfScreen(Transform transform)
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(transform.position);
        return position;
    }
}
