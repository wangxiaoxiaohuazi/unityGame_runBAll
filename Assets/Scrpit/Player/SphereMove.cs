using System.Collections;
using UnityEngine;

// [RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class SphereController : MonoBehaviour
{
    // private Animator animator;
    private Rigidbody rb;

    [Header("移动设置")]
    public float forwardSpeed = 20f; // 前进速度
    public float sideSpeed = 20f; // 左右移动速度
    public bool autoRun = true; // 是否自动行走

    private float detectionDistance = 2f; // 设置一个固定的禁止通过检测距离，例如 1.0f

    // [Header("冲刺设置")]
    private float defaultForwardSpeed = 10f; // 默认前进速度
    private Coroutine dashCoroutine; // 冲刺协程引用
    private float defaultPositionY = 0f; // 默认位置Y值

    [Header("撞击设置")]
    public float impactForce = 10f; // 撞击力度
    public float upwardForce = 2f; // 向上的力（让物体被撞飞时有一定高度）

    // [Header("撞击影响设置")]
    private float minImpactSpeed = 5f; // 最小撞击速度才会产生效果
    private float maxForceDistance = 2f; // 最大力的影响距离
    private bool addRandomRotation = true; // 是否添加随机旋转
    private float startVolume; // 获取当前体积
    private Coroutine volumeCoroutine; // 体积缩放协程引用

    void Start()
    {
        _initState();
        defaultForwardSpeed = forwardSpeed;
        startVolume = transform.localScale.x;
        // 启动协程以在一秒后赋值
        StartCoroutine(AssignDefaultPositionY());
    }

    private IEnumerator AssignDefaultPositionY()
    {
        // 等待一秒
        yield return new WaitForSeconds(1f);

        // 赋值
        defaultPositionY = transform.position.y;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void _initState()
    {
        // animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // 添加空引用检查
        if (rb == null)
        {
            enabled = false; // 禁用脚本
            return;
        }
        if (!GetComponent<Collider>())
        {
            // 添加 BoxCollider 组件
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            // 设置 BoxCollider 的中心
            boxCollider.center = new Vector3(boxCollider.center.x, 0.5f, boxCollider.center.z);
        }
        // 配置Rigidbody
        rb.freezeRotation = true; // 防止球体滚动
        rb.useGravity = false; // 禁用重力
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 改善碰撞检测
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.velocity = Vector3.zero; // 确保初始速度为零
    }

    //冲刺
    public void Dash(
        float speed,
        float durationTime,
        GameObject TrailingEffect,
        GameObject ProtectiveEffect
    )
    {
        // 如果已有冲刺在进行，先停止
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }
        // 启动新的冲刺协程
        dashCoroutine = StartCoroutine(
            DashRoutine(speed + forwardSpeed, durationTime, TrailingEffect, ProtectiveEffect)
        );
    }

    private IEnumerator DashRoutine(
        float targetSpeed,
        float duration,
        GameObject TrailingEffectPrefab,
        GameObject ProtectiveEffectPrefab
    )
    {
        // 查找或创建挂载点
        Transform Trailing = CreateMountPoint("Trailing");
        Transform Protective = CreateMountPoint("Protective");

        // 实例化特效并挂载
        InstantiateEffect(TrailingEffectPrefab, Trailing);
        InstantiateEffect(ProtectiveEffectPrefab, Protective);

        float originalSpeed = forwardSpeed;
        float elapsedTime = 0f;

        // 冲刺阶段：加速到目标速度
        while (elapsedTime < duration * 0.3f) // 前30%时间加速
        {
            forwardSpeed = Mathf.Lerp(originalSpeed, targetSpeed, elapsedTime / (duration * 0.3f));
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        // 保持阶段：维持最高速度
        forwardSpeed = targetSpeed;
        yield return new WaitForSeconds(duration * 0.4f); // 中间40%时间保持

        // 减速阶段：回到默认速度
        float remainingTime = duration * 0.3f; // 后30%时间减速
        elapsedTime = 0f;
        while (elapsedTime < remainingTime)
        {
            forwardSpeed = Mathf.Lerp(
                targetSpeed,
                defaultForwardSpeed,
                elapsedTime / remainingTime
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终速度正确
        forwardSpeed = defaultForwardSpeed;
        gameObject.GetComponent<player>().isInvincible = false; // 恢复普通状态

        // 冲刺结束后清理
        ClearEffects(Trailing);
        ClearEffects(Protective);
        //恢复默认配置
        gameObject.GetComponent<player>().InitDefaultTraling();
        dashCoroutine = null;
    }

    Transform CreateMountPoint(string name)
    {
        Transform mount = transform.Find(name);
        if (mount == null)
        {
            mount = new GameObject(name).transform;
            mount.SetParent(transform);
            mount.localPosition = Vector3.zero;
        }
        return mount;
    }

    GameObject InstantiateEffect(GameObject prefab, Transform parent)
    {
        if (prefab == null)
            return null;
        ClearEffects(parent);
        GameObject instance = Instantiate(prefab);
        instance.transform.SetParent(parent);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = prefab.transform.localRotation;
        return instance;
    }

    void ClearEffects(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    //移动
    private void HandleMovement()
    {
        Vector3 movement = Vector3.zero; // 初始化 movement 变量
        // 在编辑器中使用鼠标输入
        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            float moveX;
            if (Input.GetMouseButton(0))
            { //鼠标移动
                moveX = Input.GetAxis("Mouse X") * 0.3f;
            }
            else
            { //屏幕移动
                Debug.Log(Input.touchCount + "触摸输入");
                Touch touch = Input.GetTouch(0);
                moveX = touch.deltaPosition.x * 0.3f;
            }
            movement = transform.forward * forwardSpeed;
            movement += new Vector3(moveX * sideSpeed, 0.0f);
        }
        else
        {
            // PC 上的键盘输入
            float moveHorizontal = Input.GetAxis("Horizontal");
            float _ForwardSpeed = Input.GetAxis("Vertical");
            if (autoRun)
            {
                // 计算移动向量
                movement = transform.forward * forwardSpeed;
                movement += transform.right * moveHorizontal * sideSpeed;
            }
            else
            {
                movement = transform.forward * _ForwardSpeed * forwardSpeed;
                movement += transform.right * moveHorizontal * sideSpeed;
            }
        }
        // 检测前方是否有障碍物
        RaycastHit hit;
        if (Physics.Raycast(transform.position, movement.normalized, out hit, detectionDistance))
        {
            // 检查碰撞的物体是否为墙体
            if (hit.collider.CompareTag("StopWall")) // 禁行墙体的标签为 "StopWall"
            {
                // 如果是墙体，则不移动
                return;
            }
        }
        Debug.Log("移动" + defaultPositionY);
        Vector3 newPosition = rb.position + movement * Time.fixedDeltaTime;
        if (defaultPositionY > 0)
        {
            newPosition.y = defaultPositionY;
        }
        // 使用 Rigidbody 移动
        rb.MovePosition(newPosition);
    }

    //体积调整
    public void ChangeVolume(float volumeMultiplier, float durationTime)
    {
        // 立即调整到目标体积
        float targetVolume = startVolume * volumeMultiplier; // 计算目标体积
        transform.localScale = new Vector3(targetVolume, targetVolume, targetVolume); // 立即调整到目标体积
        if (volumeCoroutine != null)
        {
            StopCoroutine(volumeCoroutine);
        }
        // 启用协程来平滑恢复体积
        volumeCoroutine = StartCoroutine(ChangeVolumeOverTime(startVolume, durationTime));
    }

    public IEnumerator ChangeVolumeOverTime(float originalVolume, float durationTime)
    {
        float elapsedTime = 0f;
        float targetVolume = transform.localScale.x; // 当前体积（即目标体积）

        // 中间保持阶段
        yield return new WaitForSeconds(durationTime * 0.7f); // 中间70%时间保持

        // 减速阶段：回到原始体积
        float remainingTime = durationTime * 0.3f; // 后30%时间减速
        while (elapsedTime < remainingTime)
        {
            // 使用 SmoothStep 进行平滑过渡
            float t = elapsedTime / remainingTime;
            float newScale = Mathf.Lerp(targetVolume, originalVolume, Mathf.SmoothStep(0, 1, t));
            transform.localScale = new Vector3(newScale, newScale, newScale);
            elapsedTime += Time.deltaTime; // 增加经过的时间
            yield return null; // 等待下一帧
        }

        // 确保最终体积恢复到原始体积
        transform.localScale = new Vector3(originalVolume, originalVolume, originalVolume);
        volumeCoroutine = null;
    }

    //碰撞事件处理
    private void OnCollisionEnter(Collision collision)
    {
        //垂直速度设置为零
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 将垂直速度设置为零，保持水平速度
            Vector3 velocity = rb.velocity;
            velocity.y = 0; // 只清除垂直速度
            rb.velocity = velocity; // 更新速度
        }

        Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();

        if (otherRb != null && !otherRb.isKinematic)
        {
            // 计算当前速度
            Vector3 velocity = rb.velocity;
            float currentSpeed = velocity.magnitude;

            // 只有当速度超过最小值时才产生撞击效果
            if (currentSpeed > minImpactSpeed)
            {
                Vector3 direction = (collision.contacts[0].point - transform.position).normalized;

                // 根据距离计算力度
                float distance = Vector3.Distance(transform.position, collision.contacts[0].point);
                float forceFactor = 1f - Mathf.Clamp01(distance / maxForceDistance);

                // 计算最终力度
                float finalForce = impactForce * forceFactor * (currentSpeed / minImpactSpeed);

                // 应用力
                direction.y = upwardForce;
                otherRb.AddForce(direction * finalForce, ForceMode.Impulse);

                // 添加随机旋转
                if (addRandomRotation)
                {
                    otherRb.AddTorque(Random.insideUnitSphere * finalForce, ForceMode.Impulse);
                }
            }
        }
    }
}
