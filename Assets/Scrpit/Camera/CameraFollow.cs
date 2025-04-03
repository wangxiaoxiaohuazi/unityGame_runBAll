using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;            // 将这个设为 public
    private Vector3 initialPosition;
    public float offsetZ = -10f;        // 与玩家的Z轴偏移量

    void Start()
    {
        // 添加空引用检查
        if (player == null)
        {
            // 尝试自动查找玩家
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (player == null)
            {
                Debug.LogError("Camera Follow: Could not find player with 'Player' tag!");
                enabled = false; // 禁用脚本
                return;
            }
        }

        // 存储摄像机的初始位置
        initialPosition = transform.position;


    }

    void LateUpdate()
    {
        if (player == null) return;  // 安全检查
        Vector3 newPosition = new Vector3(initialPosition.x, initialPosition.y, player.position.z + offsetZ);
        transform.position = newPosition;
    }
}