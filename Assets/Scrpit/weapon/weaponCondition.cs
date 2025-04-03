using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * 武器检测条件
 */
public class weaponCondition : MonoBehaviour
{
    [Header("移动参数")]
    public float detectionDistance = 5f; // 检测范围的距离
    public float detectionWidth = 1f; // 检测范围的宽度
    public float projectileSpeed = 50f; // 子弹速度
    public float explosionForce = 500f; // 爆炸作用力
    public float explosionRadius = 5f; // 爆炸半径
    public float projectileLifetime = 5f; // 投射物的生存时间
    public bool showDetectionRange = true; // 控制是否显示检测范围

    public bool isActive = false; // 武器是否处于激活状态

    public bool isRunning = false; // 武器是否正在运行

    public GameObject bulletPrefab; // 子弹预制体
    private GameObject currentTarget; // 当前锁定的目标


    void Start()
    {
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }
    void Update()
    {
        if (isActive)
        {
            DetectEnemies();
        }
    }

    // 停用武器
    public void Deactivate()
    {
        isActive = false;
    }
    //修改参数
    public void SetParameters(float detectionDistance, float detectionWidth, float projectileSpeed, float explosionForce, float explosionRadius, float projectileLifetime, bool showDetectionRange, GameObject bulletPrefab)
    {

        this.detectionDistance = detectionDistance;
        this.detectionWidth = detectionWidth;
        this.projectileSpeed = projectileSpeed;
        this.explosionForce = explosionForce;
        this.explosionRadius = explosionRadius;
        this.projectileLifetime = projectileLifetime;
        this.showDetectionRange = showDetectionRange;
        this.bulletPrefab = bulletPrefab;
        isActive = true;
    }
    // 检测范围内的敌人
    private void DetectEnemies()
    {
        Vector3 startPoint = transform.position; // 武器的起始位置
        Vector3 direction = transform.forward; // 武器的前方方向
        if (!isRunning)
        {
            // 创建一个长方形的检测范围
            Collider[] hitColliders = Physics.OverlapBox(startPoint + direction * (detectionDistance / 2),
                       new Vector3(detectionWidth / 2, detectionWidth / 2, detectionDistance / 2),
                       Quaternion.identity, LayerMask.GetMask("Enemy")); // 确保敌人有正确的Layer
            Debug.Log("检测到敌人数量：" + hitColliders.Length);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    currentTarget = hitCollider.gameObject;
                    isRunning = true;
                    Debug.Log("敌人被检测到");
                    // 找到敌人后，发射武器作为子弹
                    LaunchProjectile();
                    break; // 找到一个敌人后退出循环
                }
            }
        }
    }

    // 发射武器作为子弹
    private void LaunchProjectile()
    {
        GameObject bullet;
        Rigidbody rb;

        // 创建子弹时强制添加刚体组件
        if (bulletPrefab != null)
        {
            bullet = Instantiate(bulletPrefab,transform.position, transform.rotation);
            // 确保预制体包含刚体组件
            if (bullet.GetComponent<Rigidbody>() == null)
            {
                bullet.AddComponent<Rigidbody>();
            }
        }
        else
        {
            bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // 自动添加刚体组件
            bullet.AddComponent<Rigidbody>();
        }
        // 设置子弹的起始位置和方向
        bullet.transform.position = transform.position; // 设置子弹的起始位置
        bullet.transform.forward = transform.forward; // 设置子弹的方向
        //播放子弹发射动画
        // bullet.GetComponent<BulletCondition>().OnPlayBulletTrail();
        // 检查是否已有刚体组件
        if (bullet.GetComponent<Rigidbody>() == null)
        {
            rb = bullet.AddComponent<Rigidbody>(); // 添加刚体组件
        }
        else
        {
            rb = bullet.GetComponent<Rigidbody>(); // 获取已有的刚体组件
        }

        rb.useGravity = false; // 禁用重力
        // 忽略子弹与检测范围的碰撞
        Collider detectionCollider = GetComponent<Collider>(); // 获取检测范围的 Collider
        Collider bulletCollider = bullet.GetComponent<Collider>(); // 获取子弹的 Collider
        if (detectionCollider == null)
        {
            Debug.LogWarning("检测范围没有 Collider，正在添加一个默认的 Collider。");
            detectionCollider = gameObject.AddComponent<SphereCollider>(); // 添加默认的 Collider
        }
        if (bulletCollider == null)
        {
            Debug.LogWarning("子弹没有 Collider，正在添加一个默认的 Collider。");
            bulletCollider = bullet.AddComponent<SphereCollider>(); // 添加默认的 Collider
        }
        Physics.IgnoreCollision(detectionCollider, bulletCollider);
        SetTargetFollow(bullet);
        isRunning = false;
        Destroy(gameObject); // 销毁检测范围
    }

    // 在场景视图中绘制检测范围
    private void OnDrawGizmos()
    {
        if (showDetectionRange)
        {
            Gizmos.color = Color.red; // 设置颜色
            Vector3 startPoint = transform.position + transform.forward * (detectionDistance / 2);
            Vector3 size = new Vector3(detectionWidth, detectionWidth, detectionDistance);
            Gizmos.DrawWireCube(startPoint, size); // 绘制检测范围的长方体
        }
    }
    //挂载追踪脚本
    // 设置追踪目标
    public void SetTargetFollow(GameObject bullet)
    {
        // 初始化追踪
        var follower = bullet.AddComponent<TargetFollower>();
        follower.SetTarget(currentTarget.transform);
        follower.moveSpeed = projectileSpeed;
        follower.rotationSpeed = 8f;
    }
}




