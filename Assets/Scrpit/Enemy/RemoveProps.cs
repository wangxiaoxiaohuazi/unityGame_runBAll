using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RemoveProps : MonoBehaviour
{
    // Start is called before the first frame update
    public List<string> ignoredTags = new List<string> { "Player", "Wall" };

    void Start()
    {

    }
    void Update()
    {

        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
    }
    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        // 输出碰撞对象的名称
        if (!ignoredTags.Contains(collision.gameObject.tag))
        {
            // 销毁碰撞对象
            Destroy(collision.gameObject);
        }
    }
}
