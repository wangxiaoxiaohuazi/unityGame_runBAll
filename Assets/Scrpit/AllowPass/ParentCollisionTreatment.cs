using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // 添加这一行
public class ParentCollisionTreatment : MonoBehaviour
{

    private Rigidbody rb;

    // public float forceMagnitude = 35f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnter(Collision collision)
    {
        // 检查碰撞的对象是否有刚体组件
        Rigidbody otherRb = collision.collider.GetComponent<Rigidbody>();

        if (otherRb && otherRb.useGravity && rb != null && !rb.useGravity)
        {
            // Debug.Log("碰撞发生，将父对象的刚体设置为非静态");
            // Debug.Log("被" + collision.collider.name + "碰撞");
            rb.useGravity = true;
            rb.isKinematic = false; // 将刚体设置为非静态
                                    // Rigidbody[] fragments = transform.gameObject.GetComponentsInChildren<Rigidbody>()
                                    //    .Where(rb => rb.gameObject != gameObject).ToArray();
                                    // // 应用力使碎片分散
                                    // foreach (Rigidbody rb in fragments)
                                    // {
                                    //     rb.useGravity = true;
                                    //     rb.isKinematic = false;
                                    // 添加一个随机方向的力
           // rb.AddExplosionForce(forceMagnitude, collision.contacts[0].point, 100f);
            // }

            // // 销毁当前物体
            // Destroy(gameObject);

        }
    }
}
