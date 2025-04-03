using UnityEngine;
using System.Collections;
/**
 *@brief 普通敌人攻击方式
 */
public class EnemAttackCondition : MonoBehaviour
{
    public GameObject bulletPrefab; // 子弹预制体
    public GameObject warningAreaPrefab; // 预警区域预制体
    private Transform player; // 玩家Transform
    public float attackInterval = 3f; // 攻击间隔
    public float warningDuration = 1.5f; // 预警持续时间
    public float attackDistance = 20f; // 在玩家前方的攻击距离
    public float bulletHeight = 10f; // 子弹生成高度

    public float bulletSpeed = 50f;//子弹速度
    private float nextAttackTime = 0f;
    private GameObject currentWarningArea; // 当前预警区域

    // Start is called before the first frame update
    void Start()
    {
        // 如果没有指定玩家，尝试查找
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        // 初始化下一次攻击时间
        nextAttackTime = Time.time + attackInterval;

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        // 检查是否到达攻击时间
        if (Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackSequence());
            nextAttackTime = Time.time + attackInterval;
        }
    }

    IEnumerator AttackSequence()
    {
        // 计算攻击位置（玩家前方特定距离）
        Vector3 playerForward = player.forward;
        Vector3 targetPosition = player.position + playerForward * attackDistance;

        // 创建预警区域
        if (warningAreaPrefab != null)
        {
            currentWarningArea = Instantiate(warningAreaPrefab, targetPosition, Quaternion.identity);

            if (currentWarningArea.GetComponent<WarningAreaCondition>() != null && gameObject.GetComponent<EnemyStateCondition>() != null)
            {
                currentWarningArea.GetComponent<WarningAreaCondition>().bulletDamage = gameObject.GetComponent<EnemyStateCondition>().attack;
            }
            // // 可以设置预警区域的大小或其他属性
            // currentWarningArea.transform.localScale = new Vector3(2f, 2f, 2f);

            // // 设置预警区域为红色
            // Renderer warningRenderer = currentWarningArea.GetComponent<Renderer>();
            // if (warningRenderer != null)
            // {
            //     warningRenderer.material.color = new Color(1f, 0f, 0f, 0.5f);
            // }
        }

        // 等待预警时间
        yield return new WaitForSeconds(warningDuration);

        // 发射子弹
        if (bulletPrefab != null)
        {
            // 在自身位置上方生成子弹
            Vector3 spawnPosition = transform.position + Vector3.up * bulletHeight;
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

            // 设置子弹朝向目标位置
            bullet.transform.LookAt(targetPosition);
            Vector3 directionToTarget = (targetPosition - spawnPosition).normalized;

            // 设置子弹朝向目标
            bullet.transform.rotation = Quaternion.LookRotation(directionToTarget);

            // 给子弹一个向目标移动的速度
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = directionToTarget * bulletSpeed;
                // 可选：关闭重力影响，使子弹直线运动
                bulletRb.useGravity = false;
            }
            else
            {
                bullet.AddComponent<Rigidbody>();
                Rigidbody bulletRb2 = bullet.GetComponent<Rigidbody>();
                bulletRb2.velocity = directionToTarget * bulletSpeed;
                // 可选：关闭重力影响，使子弹直线运动
                bulletRb2.useGravity = false;
            }
        }

        // 销毁预警区域
        if (currentWarningArea != null)
        {
            // Destroy(currentWarningArea);
        }
    }

    // 在场景视图中绘制攻击范围（用于调试）
    void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Vector3 targetPos = player.position + player.forward * attackDistance;
            Gizmos.DrawWireSphere(targetPos, 1f);

            // 绘制从敌人到目标位置的线
            Gizmos.DrawLine(transform.position, targetPos);
        }
    }
}
