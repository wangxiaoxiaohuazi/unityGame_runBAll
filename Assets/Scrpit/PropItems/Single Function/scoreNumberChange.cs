using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreNumberChange : MonoBehaviour
{
    public int num = 1;//数量

    // private Rigidbody rb;
    public bool IsDestroy = false;//是否销毁
    // 在游戏开始时调用
    void Start()
    {
        // 获取当前游戏对象上的刚体组件
        // rb = GetComponent<Rigidbody>();
    }

    // 当碰撞体进入触发器时调用
    void OnTriggerEnter(Collider other)
    {
        ChangeStart(other, num, 0, IsDestroy);
    }
    public void ChangeStart(Collider other, float _number = 0, float _time = 0, bool _isDestroy = false)
    {
        // 如果碰撞体的标签是"Player"
        if (other.tag == "Player")
        {
            // 调用GameDataManager脚本的ChangePlayerEnergy方法，参数为能量值
            other.GetComponent<player>().scoreNumber += num;
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
