using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using System.Collections;
/**
 *@brief Boss攻击1-弹道导弹
 */
public class BossAttackOne : MonoBehaviour
{
    [Header("弹道设置")]
    public int BallisticMIssileNumber = 5;//弹道数量
    private List<GameObject> BallisticMissileList = new List<GameObject>();//弹道列表
    public GameObject BallisticMissilePrefab;//弹道预制体
    public float BallisticMissileZ = 5f;//弹道长度
    public float BallisticMissileX = 0.3f;//弹道宽度
    public float BallisticMissileY = 0.1f;//弹道高度
    public float Spacing = 1f; // 弹道之间的间距
    public bool IsShowBallisticMissile = false;//是否显示弹道
    [Header("导弹攻击设置")]
    public GameObject MissilePrefab;//导弹预制体
    public float MissileSpeed = 10f;//导弹速度
    [Header("预警设置")]
    public GameObject WarningPrefab;//预警预制体
    public float WarningTime = 2f;//预警时间

    [Header("攻击设置")]
    public float AttackRange = 10f;//攻击范围
    public float AttackDamage = 1f;//攻击伤害
    public float AttackCD = 5f;//攻击CD
    private float LastAttackCDTime = 0f;//攻击CD时间
    private GameObject Target;//目标
    void Start()
    {
        //生成弹道
        initBallisticMissile();
        if (Target == null)
        {
            Target = GameObject.FindWithTag("Player");//获取玩家
        }
    }
    public void initBallisticMissile()
    {
        for (int i = 0; i < BallisticMIssileNumber; i++)
        {
            BallisticMissileList.Add(Instantiate(BallisticMissilePrefab));//初始化弹道
        }
    }
    void Update()
    {
        //处理弹道
        changeBallisticMissilePosition();
        //处理攻击
        ReleaseAttack();
    }
    public void changeBallisticMissilePosition()
    {
        // 计算总宽度
        float totalWidth = (BallisticMIssileNumber * BallisticMissileX) + ((BallisticMIssileNumber - 1) * Spacing);
        // 计算起始位置（最左边的弹道位置）
        float startX = transform.position.x - (totalWidth / 2);

        for (int i = 0; i < BallisticMissileList.Count; i++)
        {
            // 计算每个弹道的位置，确保左右对称
            float xPosition = startX + (i * (BallisticMissileX + Spacing)) + (BallisticMissileX / 2);

            // 更新弹道位置
            BallisticMissileList[i].transform.position = new Vector3(
                xPosition,
                BallisticMissileY,
                transform.position.z
            );
            // 设置弹道大小
            BallisticMissileList[i].transform.localScale = new Vector3(
                BallisticMissileX,
               0.1f,
                BallisticMissileZ
            );
            // 在游戏运行时完全透明
            Renderer renderer = BallisticMissileList[i].GetComponent<Renderer>();
            if (renderer == null)
            {
                BallisticMissileList[i].AddComponent<Renderer>();
            }
            renderer = BallisticMissileList[i].GetComponent<Renderer>();
            if (renderer != null && IsShowBallisticMissile)
            {
                renderer.enabled = true;
                // 设置为半透明的调试颜色
                renderer.material.color = new Color(1f, 0f, 0f, 0.5f); // 半透明红色
            }
            else
            {
                renderer.enabled = false;
            }

        }
    }

    public void ReleaseAttack()
    {
        //cd检查
        if (Time.time - LastAttackCDTime < AttackCD)
        {
            Debug.Log("CD中");
            return;
        }
        LastAttackCDTime = Time.time;
        //从1到BallisticMIssileNumber随机选出2-3个数
        int[] randomNumbers = Enumerable.Range(0, BallisticMIssileNumber - 1)
                                    .OrderBy(x => Guid.NewGuid())
                                    .Take(Random.Range(2, 4))
                                    .ToArray();
        foreach (int i in randomNumbers)
        {
            Debug.Log("随机数" + i);
            Debug.Log("弹道" + BallisticMissileList[i]);
            if (BallisticMissileList[i] != null)
            {
                //创建警告预制体
                StartCoroutine(CreateWarningPrf(BallisticMissileList[i]));

            }
        }

    }
    private IEnumerator CreateWarningPrf(GameObject BallisticMissileItem)
    {
        //创建警告预制体
        GameObject WarringPrf = Instantiate(WarningPrefab, BallisticMissileItem.transform.position, BallisticMissileItem.transform.rotation);
        Renderer renderer = WarringPrf.GetComponent<Renderer>();
        // 设置为半透明的调试颜色
        renderer.material.color = new Color(0.8f, 0f, 0f, 0.3f); // 半透明红色
        //设置警告预制体消失时间
        Destroy(WarringPrf, WarningTime);
        // 等待预警时间
        yield return new WaitForSeconds(WarningTime);
        //创建导弹预制体
        CreateMissilePrf(BallisticMissileItem);
    }
    private void CreateMissilePrf(GameObject BallisticMissileItem)
    {
        // 创建导弹预制体
        GameObject MissilePrf = Instantiate(MissilePrefab, BallisticMissileItem.transform.position, Quaternion.identity);

        // 获取弹道的宽度和位置
        float ballisticWidth = BallisticMissileItem.transform.localScale.x;
        float ballisticXPosition = BallisticMissileItem.transform.position.x;

        // 添加导弹控制脚本
        MissileController2 MissileController2 = MissilePrf.AddComponent<MissileController2>();
        MissileController2.Initialize(Target, ballisticXPosition, ballisticWidth, MissileSpeed, AttackDamage);
        // 10秒后销毁导弹（如果还没有被销毁）
        Destroy(MissilePrf, 10f);
    }
}

// 导弹控制器脚本
public class MissileController2 : MonoBehaviour
{
    private Transform target;
    private float ballisticXPosition; // 弹道的X坐标
    private float ballisticWidth; // 弹道宽度
    private float missileSpeed;
    public float missileHeight = 2f; // 导弹飞行高度
    private float damage = 1f; // 导弹伤害

    public void Initialize(GameObject targetObj, float xPos, float width, float speed, float attackDamage)
    {
        if (targetObj != null)
        {
            target = targetObj.transform;
        }
        ballisticXPosition = xPos;
        ballisticWidth = width;
        missileSpeed = speed;

        // 初始化时设置导弹高度
        Vector3 startPos = transform.position;
        startPos.y += missileHeight;
        transform.position = startPos;
        damage = attackDamage;
    }

    void Update()
    {
        if (target != null)
        {
            // 固定向前移动方向
            Vector3 direction = Vector3.back;
            // 计算移动方向（朝向目标）
            // 计算新位置
            Vector3 newPosition = transform.position + direction * missileSpeed * Time.deltaTime;

            // 限制X轴移动范围，确保导弹在弹道内
            float halfWidth = ballisticWidth / 2;
            newPosition.x = Mathf.Clamp(newPosition.x,
                ballisticXPosition - halfWidth,
                ballisticXPosition + halfWidth);

            // 保持Y轴高度
            newPosition.y = transform.position.y;

            // 更新位置
            transform.position = newPosition;

            // 固定朝向前方
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 触发爆炸效果
            Explode();
        }
    }

    void Explode()
    {
        // TODO: 添加爆炸效果（粒子系统等）
        Debug.Log("导弹爆炸！");

        // TODO: 对玩家造成伤害
        GameDataManager.Instance.ChangePlayerLives(-damage);
        // 销毁导弹
        Destroy(gameObject);
    }
}