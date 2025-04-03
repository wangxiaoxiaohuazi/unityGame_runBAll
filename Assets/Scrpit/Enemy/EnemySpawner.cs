using UnityEngine;

/**
 *@brief 敌人生成器-生成敌人
 */
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // 敌人的预制体
    private GameObject spawnedEnemy; // 存储生成的敌人
    private bool isEnemyActive = false; // 敌人是否处于激活状态
    public Vector3 positionOffset; // 位置偏移
    public Vector3 rotationOffset; // 旋转偏移
    private void Start()
    {
        // 在开始时禁用敌人
        if (enemyPrefab != null)
        {
            // 计算生成位置和旋转
            Vector3 spawnPosition = transform.position + positionOffset;
            Quaternion spawnRotation = Quaternion.Euler(rotationOffset);

            // 在指定位置生成敌人
            spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);
            Debug.Log($"敌人生成在位置: {spawnPosition}，旋转: {spawnRotation}");
            spawnedEnemy.SetActive(false); // 隐藏敌人
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("玩家" + other.name + other.tag);
        // 检查碰撞的物体是否是玩家
        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家" + isEnemyActive);
            // 如果敌人未激活，则激活敌人
            if (!isEnemyActive)
            {
                spawnedEnemy.SetActive(true); // 显示敌人
                Debug.Log("生成敌人");
                isEnemyActive = true; // 更新状态
            }
        }
    }
    private void OnDrawGizmos()
    {
        // 设置 Gizmos 颜色
        Gizmos.color = Color.red;

        // 计算生成位置
        Vector3 spawnPosition = transform.position + positionOffset;

        // 绘制一个球体表示生成位置
        Gizmos.DrawSphere(spawnPosition, 3f); // 0.5f 是球体的半径

        // 绘制一条线表示生成方向
        Gizmos.DrawLine(spawnPosition, spawnPosition + Quaternion.Euler(rotationOffset) * Vector3.forward); // 生成方向
    }
}