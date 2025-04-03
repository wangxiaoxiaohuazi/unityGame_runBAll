using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedChange : MonoBehaviour
{
    // Start is called before the first frame update
    public float Speed = 20;//增加速度
    public float Time = 2;//持续时间

    public bool IsDestroy = true;//是否销毁

    public bool IsInvincible = true;//是否无敌
    public bool IsExplosion = false;//是否爆炸(能被子弹炸飞)

    public GameObject TrailingEffect;//拖尾特效

    public GameObject ProtectiveEffect;//护体特效

    // 当碰撞体进入触发器时调用
    void OnTriggerEnter(Collider other)
    {
        ChangeStart(other);
    }
    public void ChangeStart(Collider other)
    {
        // 如果碰撞体的标签是"Player"
        if (other.tag == "Player")
        {
            Debug.Log("触发加速");
            // 如果碰撞体上挂载了player脚本
            if (other.GetComponent<player>())
            {
                // 将isInvincible设置为player脚本的isInvincible属性
                other.GetComponent<player>().isInvincible = IsInvincible;
            }
            if (other.GetComponent<SphereController>())
            {
                Debug.Log("调用加速");
                other.GetComponent<SphereController>().Dash(Speed, Time, TrailingEffect, ProtectiveEffect);//调用Dash函数
            }
            //销毁游戏对象
            if (IsDestroy && gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
