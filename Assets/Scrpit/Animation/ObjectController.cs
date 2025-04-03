
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    // 定义可用的功能
    public enum MovementMode
    {
        None,
        Movement,
        // Rotation,
        SelfRotation
    }

    [Header("Active Mode")]
    public MovementMode activeMode = MovementMode.None; // 当前激活的模式

    // 移动设置
    [Header("Movement Settings")]
    public float moveDistanceX = 0f;    // 左右移动距离
    public float moveDistanceY = 0f;    // 上下移动距离
    public float moveDistanceZ = 0f;    // 前后移动距离
    public float moveSpeed = 2f;         // 移动速度

    private Vector3 startPosition;        // 初始位置
    private float moveTimer;              // 移动计时器

    // // 旋转设置
    // [Header("Rotation Settings")]
    // public float rotationRadius = 1f;      // 旋转半径
    // public float rotationSpeed = 30f;      // 旋转速度
    // public Vector3 rotationDirection = new Vector3(0, 1, 0); // 旋转方向

    // 自转设置
    [Header("Self-Rotation Settings")]
    public float selfRotationSpeed = 30f;   // 自转速度
    public bool clockwise = true;            // 自转方向
    public Vector3 selfRotationAxis = new Vector3(0, 1, 0); // 自转轴

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position; // 记录初始位置
        //如果绑定物体没有刚体和碰撞检测，则添加
        // if (!GetComponent<Rigidbody>())
        // {
        //     gameObject.AddComponent<Rigidbody>();
        //     //
        //     gameObject.GetComponent<Rigidbody>().useGravity = false;
        //     gameObject.GetComponent<Rigidbody>().isKinematic = false;
        // }
        // if (!GetComponent<Collider>())
        // {
        //     CreateCollider();
        // }
    }

    // Update is called once per frame
    void Update()
    {
        switch (activeMode)
        {
            case MovementMode.Movement:
                MoveObject();
                break;
            // case MovementMode.Rotation:
            //     RotateObject();
            //     break;
            case MovementMode.SelfRotation:
                SelfRotateObject();
                break;
        }
    }

    private void MoveObject()
    {
        moveTimer += Time.deltaTime * moveSpeed;
        float x = Mathf.Sin(moveTimer) * moveDistanceX;
        float y = Mathf.Sin(moveTimer) * moveDistanceY;
        float z = Mathf.Sin(moveTimer) * moveDistanceZ;

        transform.position = startPosition + new Vector3(x, y, z);
    }

    // private void RotateObject()
    // {
    //     // 计算旋转角度
    //     float angle = rotationSpeed * Time.deltaTime;

    //     // 计算旋转的方向
    //     Vector3 rotationAxis = rotationDirection.normalized; // 确保旋转方向是单位向量

    //     // 计算旋转后的新位置
    //     Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis);
    //     Vector3 offset = transform.position - startPosition; // 计算当前物体位置与起始位置的偏移
    //     offset = rotation * offset; // 应用旋转
    //     transform.position = startPosition + offset; // 更新物体位置
    // }

    private void SelfRotateObject()
    {
        float angle = selfRotationSpeed * Time.deltaTime;
        if (!clockwise)
        {
            angle = -angle; // 逆时针旋转
        }
        transform.Rotate(selfRotationAxis * angle);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (activeMode == MovementMode.None)
        {
            return;
        }
// 
      //  Debug.Log($"发生碰撞，撞击到：{collision.gameObject.name}");

        // 检查碰撞的物体是否有 Rigidbody
        Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
        if (otherRb != null)
        {
            // Debug.Log("效果移除");
            // 碰撞后移除所有效果
            activeMode = MovementMode.None;
        }
        else
        {
            Debug.Log("碰撞物体没有 Rigidbody 组件");
        }

    }
    private void CreateCollider()
    {
        // 创建一个新的 MeshCollider
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;

        // 创建一个新的 Mesh
        Mesh mesh = new Mesh();
        meshCollider.sharedMesh = mesh;

        // 获取所有子节点的渲染器
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // 如果没有子节点，返回
        if (renderers.Length == 0)
        {
            Debug.LogWarning("没有子节点可用于创建碰撞体");
            return;
        }

        // 计算包围盒
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        // 创建一个立方体的顶点
        Vector3[] vertices = new Vector3[8];
        vertices[0] = bounds.min; // 左下前
        vertices[1] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z); // 左下后
        vertices[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z); // 左上前
        vertices[3] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z); // 左上后
        vertices[4] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z); // 右下前
        vertices[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z); // 右下后
        vertices[6] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z); // 右上前
        vertices[7] = bounds.max; // 右上后

        // 创建三角形
        int[] triangles = new int[]
        {
            0, 2, 1, 1, 2, 3, // 左面
            4, 5, 6, 4, 6, 7, // 右面
            0, 1, 4, 4, 1, 5, // 前面
            2, 6, 3, 3, 6, 7, // 后面
            0, 4, 2, 2, 4, 6, // 底面
            1, 3, 5, 5, 3, 7  // 顶面
        };

        // 设置网格数据
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // 计算法线

        // 将 MeshCollider 的网格设置为新创建的网格
        meshCollider.sharedMesh = mesh;

        Debug.Log("已创建包裹子节点的 MeshCollider");
    }
}
