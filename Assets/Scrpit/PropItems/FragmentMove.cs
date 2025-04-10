using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentMove : MonoBehaviour
{
    [Tooltip("不填默认为player")]
    public GameObject Target;//移动目标
    public List<GameObject> rewardList;//奖励列表
    public List<int> rewardNumList; //奖励数量列表
    // public float speed = 50f;//移动速度
    public float delayTime = 2.5f;//延迟时间
    public float explosionForce = 1f;//爆炸力
    public float Rbmass = 1f;//刚体质量
    public bool IsExplosion = false;//是否爆炸(能被子弹炸飞)

    private bool IsRuning = false;
    // Start is called before the first frame update
    void Start()
    {
        if (Target == null)
        {
            Target = GameObject.Find("Player");
        }
    }

    void FixedUpdate()
    {
        if (gameObject.GetComponent<EnemyStateCondition>() != null && gameObject.GetComponent<EnemyStateCondition>().hp < 1)
        {
            ActiveReward();
        }
    }
    void ActiveReward()
    {
        if (IsRuning) return;
        IsRuning = true;
        Debug.Log("奖励碰撞");
        if (rewardList.Count > 0)
        {
            for (int i = 0; i < rewardList.Count; i++)
            {
                for (int j = 0; j < rewardNumList[i]; j++)
                {
                    // Debug.Log("第num：" + i + '的' + j + "rewardNumList" + rewardNumList[i]);
                    Debug.Log("渲染：" + rewardList[i].gameObject.name);
                    Quaternion brokenRotation = rewardList[i].transform.rotation;
                    GameObject reward = Instantiate(rewardList[i], transform.position, brokenRotation);
                    reward.transform.localScale = rewardList[i].transform.localScale;
                    if (reward.GetComponent<Rigidbody>() == null)
                    {
                        reward.AddComponent<Rigidbody>();
                    }
                    Rigidbody rb = reward.GetComponent<Rigidbody>();
                    rb.mass = Rbmass;
                    ResetRigidbody(rb);
                    SetTargetFollow(reward);
                }
            }
        }
    }
    private void ResetRigidbody(Rigidbody rb)
    {
        rb.useGravity = true;
        rb.isKinematic = false;

        // 使用当前物体的位置作为爆炸中心
        Vector3 randomOffset = new Vector3(
        Random.Range(-0.5f, 0.5f),
        Random.Range(-0.2f, 0.5f),  // Y轴偏移较小避免过度下沉
        Random.Range(-0.5f, 0.5f)
    );
        Vector3 explosionPoint = rb.transform.position+randomOffset;
        float explosionRadius = 5f; // 爆炸半径
        // 添加爆炸力
        rb.AddExplosionForce(
            explosionForce,          // 爆炸力的大小
            explosionPoint,          // 爆炸中心点
            explosionRadius,         // 爆炸半径
            0.5f,                    // 影响的衰减
            ForceMode.Force        // 力的施加模式
        );

        // 可选：添加随机旋转力
        rb.AddTorque(new Vector3(
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f)
        ));
    }
    //挂载追踪脚本
    // 设置追踪目标
    public void SetTargetFollow(GameObject reward)
    {
        // 初始化追踪
        var follower = reward.AddComponent<TargetFollower>();
        follower.delayTime = delayTime; // 设置延迟时间
        follower.moveSpeed = Target.GetComponent<SphereController>().forwardSpeed * 1.7f;
        follower.rotationSpeed = 8f;
        follower.SetTarget(Target.transform);
    }
}
