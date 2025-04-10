using UnityEngine;

public class ItemPickup : MonoBehaviour
{

    // [Header("攻击参数")]
    public float projectileSpeed = 70f; // 子弹速度
    private float projectileLifetime = 5f; // 投射物的生存时间
    public GameObject BulletPrefab;//子弹预制体
    [Header("子弹范围检测参数")]
    public float detectionDistance = 69f; // 检测范围的距离
    public float detectionWidth = 17f; // 检测范围的宽度
    public bool showDetectionRange = true; // 控制是否显示检测范围
    void Start()
    {
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
    }

    // 当与其他物体发生碰撞时
    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞的物体是否是 Player
        if (other.CompareTag("Player"))
        {
            // 获取 Player 对象
            GameObject player = other.gameObject;
            // 直接查找子物体
            Transform weaponTransform = player.transform.Find("Weapon");
            Debug.Log(weaponTransform + "Weapon");
            // GameObject weapon = weaponTransform.gameObject;
            if (weaponTransform != null)
            {

                //将当前物体复制一份添加到weaponTransform下，并且重置物体的transform
                GameObject weapon = Instantiate(gameObject, weaponTransform);
                // if (weapon == null) return; //如果武器为空，则直接返回
                if (weapon.GetComponent<ObjectController>() != null)
                {
                    weapon.GetComponent<ObjectController>().activeMode = ObjectController.MovementMode.None;
                }
                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localRotation = Quaternion.identity;
                weapon.transform.parent = weaponTransform;
                weapon.AddComponent<weaponCondition>();
                weapon.GetComponent<weaponCondition>().SetParameters(
                    detectionDistance,
                    detectionWidth,
                    projectileSpeed,
                    showDetectionRange,
                    BulletPrefab
                );

            }

            // 销毁当前道具
            Destroy(gameObject);
        }
    }
}