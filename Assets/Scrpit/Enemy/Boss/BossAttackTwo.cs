using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Boss攻击二-障碍物生成
 */
public class BossAttackTwo : MonoBehaviour
{
    //障碍物预制体列表
    public List<GameObject> obstacleList = new List<GameObject>();

    //障碍物生成间隔
    public float obstacleSpawnInterval = 10f;
    //障碍物生成数量
    public int obstacleSpawnCount = 5;
    private float lasetobstacleSpawnInterval = 0f;//上一次障碍物生成时间
    //添加这些新变量
    private Transform playerTransform; //玩家位置引用
    public float spawnRangeWidth = 10f; //生成范围宽度
    public float spawnDistanceFromPlayer = 15f; //与玩家的前方距离

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lasetobstacleSpawnInterval >= obstacleSpawnInterval - 1)
        {
            playerTransform = GameObject.FindWithTag("Player").transform; //获取玩家位置
            obstacleListAttack();
        }
    }
    public void obstacleListAttack()
    {
        if (Time.time - lasetobstacleSpawnInterval >= obstacleSpawnInterval)
        {
            for (int i = 0; i < obstacleSpawnCount; i++)
            {
                //计算随机生成位置
                float randomX = Random.Range(-spawnRangeWidth / 2, spawnRangeWidth / 2);
                Vector3 spawnPosition = playerTransform.position +
                    playerTransform.forward * spawnDistanceFromPlayer +
                    Vector3.right * randomX;
                //保持Y轴位置与地面齐平
                spawnPosition.y = 0.1f;
                Debug.Log("PlayerName:====" + playerTransform.name);
                Debug.Log(playerTransform.position.z + "player:====" + spawnDistanceFromPlayer);
                spawnPosition.z = playerTransform.position.z + spawnDistanceFromPlayer;
                //在计算出的位置生成障碍物
                GameObject obstacle = Instantiate(obstacleList[Random.Range(0, obstacleList.Count)],
                    spawnPosition,
                    Quaternion.identity);
                //保持Y轴位置与地面齐平
                obstacle.transform.position = new Vector3(obstacle.transform.position.x, 1f, playerTransform.position.z + spawnDistanceFromPlayer);
                //销毁
                Destroy(obstacle, 3f);
            }
            lasetobstacleSpawnInterval = Time.time;
        }
    }
}
