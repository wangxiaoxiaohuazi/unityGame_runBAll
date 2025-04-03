using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyChange : MonoBehaviour
{
    public float Size = 1.5f;//增加的大小
    public float Time = 2;//持续时间

    public bool IsDestroy = false;//是否销毁



    // 当碰撞体进入触发器时调用
    void OnTriggerEnter(Collider other)
    {
        ChangeStart(other, Size, Time, IsDestroy);
    }
    public void ChangeStart(Collider other, float _number = 0, float _time = 0, bool _isDestroy = false)
    {
        // 如果碰撞体的标签是"Player"
        if (other.tag == "Player")
        {
            if (other.GetComponent<SphereController>())
            {
                Debug.Log("调用加速");
                other.GetComponent<SphereController>().ChangeVolume(_number, _time);//调用Dash函数
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
