using UnityEngine;

public class BulletCondition : MonoBehaviour
{
    //子弹状态
    public enum BulletState
    {
        Normal,//正常
        Explode,//爆炸
    }
    //子弹伤害
    public int bulletDamage = 1;
    //子弹伤害范围
    public float bulletDamageRange = 10f;

    public float forceMagnitude = 20f;//子弹击退力
    //子弹拖尾效果
    public GameObject bulletTrail;
    //子弹爆炸效果
    public GameObject bulletExplodeEffect;

    // Start is called before the first frame update
    void Start()
    {
        //设置tag为Bullet
        gameObject.tag = "Bullet";
        //如果没有collider，则添加一个,并且将isTrigger设置为true
        if (!GetComponent<Collider>())
        {
            gameObject.AddComponent<BoxCollider>();
            GetComponent<Collider>().isTrigger = true;
        }
        else if (GetComponent<Collider>().isTrigger == false)
        {
            GetComponent<Collider>().isTrigger = true;
        }
        //隐藏拖尾效果
        if (bulletTrail != null)
        {
            // bulletTrail.SetActive(false);

        }
        //隐藏爆炸效果
        if (bulletExplodeEffect != null)
        {
            bulletExplodeEffect.SetActive(false);
        }
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            bulletDamage = player.GetComponent<player>().attack * bulletDamage;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void OnPlayBulletTrail()
    {
        if (bulletTrail != null)
        {
            bulletTrail.SetActive(true);
        }
    }
    public void OnPlayBulletExplodeEffect(Vector3 position)
    {
        if (bulletExplodeEffect != null)
        {
            // 实例化爆炸效果
            GameObject explosionEffect = Instantiate(bulletExplodeEffect, position, Quaternion.identity);
            explosionEffect.SetActive(true);
            // 可选：设置爆炸效果的生命周期
            Destroy(explosionEffect, 2f); // 2秒后销毁爆炸效果
        }
        else
        {
            Debug.LogWarning("爆炸效果预制体未设置！");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞的物体是否是 Enemy
        if (other.CompareTag("Enemy"))
        {

            // 播放爆炸效果
            OnPlayBulletExplodeEffect(other.transform.position);
            //创建一个范围让范围内的物体被炸飞
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, bulletDamageRange);
            foreach (var hitCollider in hitColliders)
            {
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                // rb.TryGetComponent<PublicItem>(out var publicItem)  防止PublicItem&SpeedChange 为空
                if (rb != null && rb.tag != "Wall" &&
                   ((rb.TryGetComponent<PublicItem>(out var publicItem) && publicItem.IsExplosion) ||
                    (rb.TryGetComponent<SpeedChange>(out var speedChange) && speedChange.IsExplosion)))
                {
                    // 启用重力并取消 Kinematic
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    // Debug.Log("Player:" + collision.gameObject.name);
                    // 计算击飞方向
                    Vector3 forceDirection = other.transform.position - transform.position;
                    forceDirection.y = 1; // 确保力有一个向上的分量
                    forceDirection.Normalize();

                    // 应用力
                    // 力的大小可以根据需要调整
                    rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
                }
            }
            Destroy(gameObject, 2f);
        }
    }
    // 在场景视图中绘制爆炸范围
    private void OnDrawGizmos()
    {
        // 设置 Gizmos 颜色
        Gizmos.color = Color.red; // 可以根据需要更改颜色

        // 绘制一个球体，表示爆炸范围
        Gizmos.DrawWireSphere(transform.position, bulletDamageRange);
    }

}
