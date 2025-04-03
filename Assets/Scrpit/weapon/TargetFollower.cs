using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetFollower : MonoBehaviour
{
    [Header("基础设置")]
    [Tooltip("要追踪的目标对象")]
    public Transform target;
    [Tooltip("移动速度")]
    public float moveSpeed = 10f;
    [Tooltip("转向速度")]
    public float rotationSpeed = 5f;
    [Tooltip("启动时间")]
    public float delayTime = 0.5f; // 延迟时间
    private Rigidbody rb;
    private bool isTracking = true;
    private Vector3 lastDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ConfigureRigidbody();
    }

    void FixedUpdate()
    {
        if (isTracking)
        {
            if (TargetIsValid())
            {
                UpdateMovement();
            }
            else
            {
                // 目标失效后保持最后方向直线运动
                isTracking = false;
                rb.velocity = lastDirection.normalized * moveSpeed;
            }
        }
    }

    void UpdateMovement()
    {
        // 检查物体是否有触发器
        Collider[] colliders = GetComponents<Collider>();
        bool hasTrigger = false;

        foreach (var collider in colliders)
        {
            if (collider.isTrigger)
            {
                hasTrigger = true; // 如果找到触发器，设置标志
                break;
            }
        }

        // 如果没有触发器，添加一个新的 Collider 并设置为触发器
        if (!hasTrigger)
        {
            BoxCollider newCollider = gameObject.AddComponent<BoxCollider>(); // 添加一个 BoxCollider
            newCollider.isTrigger = true; // 设置为触发器
        }
        // 直接获取目标当前位置
        Vector3 targetPosition = target.position;
        Vector3 direction = (targetPosition - rb.position).normalized;
        lastDirection = direction;
        // 平滑转向
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        // 根据速度移动到目标位置
        float step = moveSpeed * Time.deltaTime; // 计算每帧移动的距离
        rb.position = Vector3.MoveTowards(rb.position, targetPosition, step); // 移动到目标位置
    }

    bool TargetIsValid()
    {
        return target != null &&
               target.gameObject.activeInHierarchy;
    }

    void ConfigureRigidbody()
    {
        rb.useGravity = false;
        rb.drag = 0;
        rb.angularDrag = 0.5f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void SetTarget(Transform newTarget)
    {

        if (delayTime == 0)
        {
            target = newTarget;
            isTracking = true;
        }
        else
        {
            // delayTime后执行赋值
            StartCoroutine(SetTargetAfterDelay(newTarget));

        }
    }
    private IEnumerator SetTargetAfterDelay(Transform newTarget)
    {
        yield return new WaitForSeconds(delayTime); // 等待delayTime秒
        target = newTarget; // 执行赋值
        isTracking = true; // 开始追踪
        rb.velocity = Vector3.zero;
    }

}