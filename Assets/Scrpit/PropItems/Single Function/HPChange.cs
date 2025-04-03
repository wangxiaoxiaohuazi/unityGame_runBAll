using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPChange : MonoBehaviour
{
    // Start is called before the first frame update
    public int HP = 1;//血量
    private Rigidbody rb;
    public bool IsDestroy = false;//是否销毁
    // 在游戏开始时调用
    void Start()
    {
        // 获取当前游戏对象上的刚体组件
        rb = GetComponent<Rigidbody>();
    }

    // 当碰撞体进入触发器时调用
    void OnTriggerEnter(Collider other)
    {
        ChangeStart(other, HP, 0, IsDestroy);
    }
    public void ChangeStart(Collider other, float _number = 0, float _time = 0, bool _isDestroy = false)
    {// 如果碰撞体的标签是"Player"
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
            if ((HP < 0 && !isInvincible) || HP > 0)
            {
                // 调用GameDataManager脚本的ChangePlayerLives方法，参数为伤害值
                GameDataManager.Instance.ChangePlayerLives(HP);
            }
            //销毁游戏对象
            if (this != null)
            {
                if (_isDestroy && gameObject != null)
                {

                    Destroy(gameObject);
                }
            }
        }
    }
}
