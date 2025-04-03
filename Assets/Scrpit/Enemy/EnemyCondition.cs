using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/***
 * @todo 敌人状态与攻击调度
 * */
public class EnemyCondition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //如果物体没有刚体和碰撞器，则添加
        if (GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

}
