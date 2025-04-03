using UnityEngine;
/**
 * @brief 碰撞反馈-伤害/能量吸收
 */
public class Barrier : MonoBehaviour
{
    // Start is called before the first frame update
    public int damage = 1;
    public float energy = 0.01f;

    public float collisionProbability = 0.8f;

    public float forceMagnitude = 35f;

    private Rigidbody rb;
    // 在游戏开始时调用
    void Start()
    {
        // 获取当前游戏对象上的刚体组件
        rb = GetComponent<Rigidbody>();
    }

    // 当碰撞体进入触发器时调用
    void OnTriggerEnter(Collider other)
    {
        // 如果碰撞体的标签是"Wall"，则返回
        if (other.tag == "Wall") return;
        // 如果碰撞体的标签是"Player"
        if (other.tag == "Player")
        {
            // 定义一个布尔变量isInvincible，初始值为false
            bool isInvincible = false;
            // 如果碰撞体上挂载了player脚本
            if (other.GetComponent<player>())
            {
                // 将isInvincible设置为player脚本的isInvincible属性
                isInvincible = other.GetComponent<player>().isInvincible;
            }
            // 如果没有找到GameDataManager脚本
            if (!FindObjectOfType<GameDataManager>())
            {
                // 输出警告信息
                Debug.LogWarning("GameDataManager 未找到！");
                return;
            }
            // 如果伤害值大于0且玩家不是无敌状态
            if (damage > 0 && !isInvincible)
            {
                // 调用GameDataManager脚本的ChangePlayerLives方法，参数为伤害值
                GameDataManager.Instance.ChangePlayerLives(-damage);
            }
            // 如果能量值大于0
            if (energy > 0)
            {
                // 销毁当前游戏对象
                Destroy(gameObject);
                // 调用GameDataManager脚本的ChangePlayerEnergy方法，参数为能量值
                GameDataManager.Instance.ChangePlayerEnergy(energy);
            }
        }
    }

}

