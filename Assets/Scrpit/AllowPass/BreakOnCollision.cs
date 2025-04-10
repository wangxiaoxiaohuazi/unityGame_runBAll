using System.Linq;
using UnityEngine;

/**
 * @brief 碰撞后生成碎块脚本
 */
public class BreakOnCollision : MonoBehaviour
{
    [Header("碎块参数")]
    public GameObject BreakPrefab;     // 碎块预制体
    //Scale继承谁的参数
    public enum BreakSizeOption
    {
        Parent,//父对象
        Self,//自身
        Auto//自定义
    }

    public BreakSizeOption breakPrefabSize = BreakSizeOption.Parent; // 默认值为 Parent
    public Vector3 Scale;       // 碎块最小尺寸
    [Header("爆炸参数")]
    public float breakForce = 0.1f;       // 爆炸力度
    public bool breakOnTrigger = true; // 是否在触发器碰撞时破碎
    public float DistoryTime = 5f; // 碎块销毁时间
    private bool IsRuning = false;
    private float lastActiveTime = 0f; // 上次激活时间
    private float debounceDelay = 1f; // 防抖延迟时间
    void Start()
    {
    }


    void FixedUpdate()
    {
        if (gameObject.GetComponent<EnemyStateCondition>() != null && gameObject.GetComponent<EnemyStateCondition>().hp < 1)
        {
            ActiveBrothers();
        }
    }

    public void ActiveBrothers()
    {
        Debug.Log("ActiveBrothers");
        if (IsRuning || Time.time - lastActiveTime < debounceDelay) return; // 添加防抖处理
        lastActiveTime = Time.time; // 更新上次激活时间
        if (!breakOnTrigger) return; // 如果不是在触发器碰撞时破碎，则不执行以下代码
        if (BreakPrefab == null)
        {
            Debug.LogWarning("对象未绑定碎块预制体");
            return;
        }
        if (gameObject.transform.parent == null)
        {
            Debug.LogWarning("当前对象没有父对象");
            return;
        }
        IsRuning = true;
        Vector3 brokenScale = Vector3.zero;
        Quaternion brokenRotation = Quaternion.identity;
        switch (breakPrefabSize)
        {
            case BreakSizeOption.Parent:
                brokenScale = transform.parent.localScale;
                brokenRotation = transform.parent.rotation;
                break;
            case BreakSizeOption.Self:
                brokenScale = BreakPrefab.transform.localScale;
                brokenRotation = BreakPrefab.transform.rotation;
                break;
            case BreakSizeOption.Auto:
                brokenScale = Scale;
                brokenRotation = BreakPrefab.transform.rotation;
                break;
        }
        // 实例化碎块预制体
        GameObject brokenObject = Instantiate(BreakPrefab, transform.position, brokenRotation);
        brokenObject.transform.localScale = brokenScale;

        Rigidbody[] fragments = brokenObject.gameObject.GetComponentsInChildren<Rigidbody>()
                       .Where(rb => rb.gameObject != gameObject).ToArray();

        if (fragments.Length == 0)
        {
            if (brokenObject.GetComponent<Rigidbody>() == null)
            {
                brokenObject.AddComponent<Rigidbody>();
            }
            Rigidbody rb = brokenObject.GetComponent<Rigidbody>();
            ResetRigidbody(rb);
        }
        else
        {
            foreach (Rigidbody rb in fragments)
            {
                ResetRigidbody(rb);
            }

        }

        // 设置自动销毁（DistoryTime秒后清理碎块）
        Destroy(brokenObject, DistoryTime);
        // 销毁当前物体
    }
    private void ResetRigidbody(Rigidbody rb)
    {
        if (rb.tag == "Wall") return;
        rb.useGravity = true;
        rb.isKinematic = false;
        // 修改后的爆炸参数
        rb.AddExplosionForce(
            breakForce,          // 增大爆炸强度
            explosionPosition: transform.position + Random.insideUnitSphere * 0.5f, // 添加随机中心偏移
            explosionRadius: 5f,          // 增大影响半径
            upwardsModifier: 0.3f,         // 降低垂直修正
            mode: ForceMode.Impulse
        );

        // 添加水平随机力
        Vector3 horizontalForce = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ) * 200f;
        rb.AddForce(horizontalForce, ForceMode.Impulse);

        // 修改旋转力参数
        rb.AddTorque(Random.insideUnitSphere * 50, ForceMode.Impulse); // 增大扭矩值
        //变色
        // 创建 ColorChanger 实例并调用 SetColor 方法
        if (rb.gameObject.GetComponent<ColorChanger>() == null)
        {
            ColorChanger colorChanger = rb.gameObject.AddComponent<ColorChanger>();
            colorChanger.SetColor();
        }
        else
        {
            rb.gameObject.GetComponent<ColorChanger>().SetColor();
        }
    }
}


