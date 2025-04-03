using UnityEngine;

public class DistanceChecker : MonoBehaviour
{
    public float detectionRadius = 5f; // 检测半径
    public bool showView; // 显示视图脚本
    void Update()
    {
        // 检测在指定半径内的所有 Collider
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider collider in colliders)
        {
            // 检查 Collider 是否附加了 BreakOnCollision 脚本
            // BreakOnCollision breakOnCollision = collider.GetComponent<BreakOnCollision>();
            // if (breakOnCollision != null)
            // {
            //     // 调用 ActiveBrothers 方法
            //     breakOnCollision.ActiveBrothers(collider.GetComponent<Collision>());
            // }
        }
    }

    // 可选：可视化检测范围
    private void OnDrawGizmos()
    {
        if (showView)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }

    }
}