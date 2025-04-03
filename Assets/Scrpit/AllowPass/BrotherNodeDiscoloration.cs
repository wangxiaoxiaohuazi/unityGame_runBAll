
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BrotherNodeDiscoloration : MonoBehaviour
{
    public float colorChangeDuration = 1f; // 变色持续时间
    private Material _instanceMaterial;

    void Start()
    {
        //如果没有刚体，则添加刚体
        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
            //设置刚体为静态
            GetComponent<Rigidbody>().isKinematic = true;

        }
        //如果没有碰撞器，则添加碰撞器
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        Color targetColor = GameDataManager.Instance.BaseColor;

        if (collision.gameObject.tag == "Player")
        {
            //实现兄弟节点变色

            if (transform.parent == null)
            {
                if (transform.GetComponent<ColorChanger>() == null)
                {
                    transform.AddComponent<ColorChanger>();
                }
                transform.GetComponent<ColorChanger>().SetColor(colorChangeDuration);
                return;
            }
            Rigidbody[] arguments = transform.parent.gameObject.GetComponentsInChildren<Rigidbody>();
            if (arguments.Length > 0)
            {
                arguments.Where(rb => rb.gameObject != transform.parent.gameObject).ToArray();
                foreach (Rigidbody fragment in arguments)
                {
                    // 启动协程进行颜色渐变
                    if (fragment.GetComponent<ColorChanger>() == null)
                    {
                        fragment.AddComponent<ColorChanger>();
                    }
                    fragment.GetComponent<ColorChanger>().SetColor(colorChangeDuration);
                }
            }
        }

    }
}
