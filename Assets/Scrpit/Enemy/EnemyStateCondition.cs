
using System.Collections;
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
    public GameObject BloodScroll;//怪物血条
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
        bool flag = gameObject.tag == "Enemy";//判断是否为敌人

        //敌人不发生直接碰撞，仅做触发判断
        if (!GetComponent<Collider>())
        {
            gameObject.AddComponent<BoxCollider>();
            GetComponent<Collider>().isTrigger = flag;
        }
        else if (GetComponent<Collider>().isTrigger == false)
        {
            GetComponent<Collider>().isTrigger = flag;
        }
        //如果物体没有刚体和碰撞器，则添加
        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
            GetComponent<Rigidbody>().useGravity = flag;
        }


        BloodMax = hp;
    }
    void Update()
    {
        if (BloodScroll != null)
        {
            changeBloodScroll(); // 更新血条
        }
        if (hp <= 0)
        {
            Die(); // 调用死亡方法
        }
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
            // Destroy(other.gameObject); // 销毁子弹
        }
        if (other.CompareTag("Player"))
        {
            TakeDamage(other.GetComponent<player>().attack);
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
            if (attack > 0 && !isInvincible)
            {
                // 调用GameDataManager脚本的ChangePlayerLives方法，参数为伤害值
                GameDataManager.Instance.ChangePlayerLives(-attack);
            }
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
        // 先禁用自身组件
        if (TryGetComponent<Renderer>(out var selfRenderer))
            selfRenderer.enabled = false;
        if (TryGetComponent<Collider>(out var selfCollider))
            selfCollider.enabled = false;
        // 禁用所有渲染器和碰撞器
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = false;
        foreach (var collider in GetComponentsInChildren<Collider>())
            collider.enabled = false;
        Debug.Log("敌人死亡");
        // 延迟销毁
        StartCoroutine(DelayedDestroy());
    }
    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(2f);

        Debug.Log("敌人销毁");
        Destroy(gameObject); // 销毁敌人对象

        // 可选：播放消失特效
        // Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
    }
    private void changeBloodScroll()
    {
        BloodScroll.GetComponent<BloodUICondition>().ChangeBloodUI();
    }
}
