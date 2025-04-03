using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/**
 *@brief 怪物状态
 */
public class EnemyStateCondition : MonoBehaviour
{
    //怪物血量
    public float hp = 1f;
    //怪物攻击力
    public float attack = 1f;
    //怪物防御力
    public float defense = 0f;
    public Scrollbar BloodScrollbar;//怪物血条
    private float BloodMax = 1f;//怪物最大血量
    // //怪物速度
    // public float speed;
    // //怪物攻击间隔
    // public float attackInterval;
    // //怪物攻击范围
    // public float attackRange;
    // //怪物攻击类型
    // public string attackType;
    // //怪物攻击效果
    // public string attackEffect;
    // //怪物攻击目标
    // public GameObject attackTarget;
    // //怪物攻击状态
    // public string attackState;
    // //怪物攻击动画
    // public string attackAnimation;
    // //怪物攻击声音
    // public string attackSound;
    // //怪物攻击伤害
    // public float attackDamage;
    //怪物攻击速度
    // public float attackSpeed;

    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<Collider>())
        {
            gameObject.AddComponent<BoxCollider>();
            GetComponent<Collider>().isTrigger = true;
        }
        else if (GetComponent<Collider>().isTrigger == false)
        {
            GetComponent<Collider>().isTrigger = true;
        }
        //如果物体没有刚体和碰撞器，则添加
        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
            GetComponent<Rigidbody>().useGravity = false;
        }
        BloodMax = hp;
    }

    // Update is called once per frame
    void Update()
    {
        changeBloodScrollbar(); // 更新血条
    }
    //被子弹碰到则扣血，血量小于等于0则销毁
    // 被子弹碰到时调用
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"sss:{other.name}");
        // 检查碰撞的物体是否是子弹
        if (other.CompareTag("Bullet"))
        {
            if (other.GetComponent<BulletCondition>() && other.GetComponent<BulletCondition>().bulletDamage > 0)
            {
                // 扣血
                TakeDamage(other.GetComponent<BulletCondition>().bulletDamage); // 假设每次被子弹碰到扣1点血
            }
            Destroy(other.gameObject); // 销毁子弹
        }
    }

    // 扣血方法
    private void TakeDamage(float damage)
    {
        if (defense > 0)
        {
            defense -= damage; //扣除防御
            Debug.Log($"当前防御: {defense}");
        }
        else
        {
            hp -= damage; // 扣除血量
            Debug.Log($"当前血量: {hp}");
        }
        // 检查血量是否小于等于0
        if (hp <= 0)
        {
            Die(); // 调用死亡方法
        }
    }

    // 死亡方法
    private void Die()
    {
        Debug.Log("敌人死亡");
        Destroy(gameObject); // 销毁敌人对象
    }
    private void changeBloodScrollbar()
    {
        BloodScrollbar.size = Mathf.Clamp(hp / BloodMax, 0f, 1f);
    }
}
