using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningAreaCondition : MonoBehaviour
{
    public GameObject bulletExplodeEffect;
    public float forceMagnitude = 30f;
    public float bulletDamageRange = 6f;
    public float bulletDamage = 1f;
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
        if (!GetComponent<Rigidbody>())
        {
            gameObject.AddComponent<Rigidbody>();
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {

            // 播放爆炸效果
            OnPlayBulletExplodeEffect(other.transform.position);
            //创建一个范围让范围内的物体被炸飞
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, bulletDamageRange);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.GetComponent<player>())
                {
                    // other.GetComponent<>();
                    GameDataManager.Instance.ChangePlayerLives(-bulletDamage);
                }
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                if (rb != null && rb.tag != "Wall")
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
            Destroy(other.gameObject); // 销毁
            Destroy(gameObject);
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
}
