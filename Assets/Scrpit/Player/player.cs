using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class player : MonoBehaviour
{

    private Rigidbody rb;
    public float forceMagnitude = 10f; // 调整这个值来改变击飞的力度

    public float blood = 4; //血量
    public bool isInvincible = false; //无敌状态
    // public float speed = 5f; //速度
    // public float jumpForce = 5f; //跳跃力
    // public float jumpTime = 0.5f; //跳跃时间
    public float power = 0; //能量
    public int levelNumber = 0; //等级
    public float scoreNumber = 0; //分数                                 
    public int attack = 1;//攻击力

    // private VibrationManager vibrationManager;
    [Header("默认角色")]
    public GameObject _defaultPlayer;
    public GameObject _defaultTrailing;
    private int priveAttactNodeID;

    void Start()
    {
        //初始化基础赋值
        rb = GetComponent<Rigidbody>();
        // vibrationManager = gameObject.AddComponent<VibrationManager>();
        InitDefaultPlayer(); //初始化默认角色
        InitDefaultTraling(); //初始化默认拖尾
        Debug.Log("当前总获得金币：" + DataManager.Instance.gameInfo.player.coin);
    }



    void Update()
    {

        // 检测 R 键输入
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnResetGame(); // 调用重置游戏事件
        }
    }

    public void OnResetGame()
    {

        // 重新加载当前场景
        if (PlayerInfo.Instance.GetVigourNumber() > 1)
        {
            PlayerInfo.Instance.AddVigourNumber(-3);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            GameDataManager.Instance.ChangePlayerLives(blood);
            GameDataManager.Instance.endGameVisible = false;
        }
        Time.timeScale = 1;
    }

    //初始化玩家皮肤
    public void InitDefaultPlayer()
    {
        // 获取当前player对象
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        // 获取body对象
        Transform bodyComponent = player.transform.Find("Body");
        Debug.Log(_defaultPlayer + "默认");
        if (bodyComponent != null)
        {
            //移除所有子对象
            foreach (Transform child in bodyComponent)
            {
                Destroy(child.gameObject);
            }
            // 添加默认皮肤
            Instantiate(_defaultPlayer, bodyComponent.transform.position, _defaultPlayer.transform.rotation, bodyComponent);
            Debug.Log("添加默认皮肤" + _defaultPlayer.name);
        }
        setLevelNode(0);
    }
    public void setLevelNode(int num, Collider _Collider = null)
    {
        //发生碰撞增加等级
        if (num != 0 && _Collider == null && priveAttactNodeID == _Collider.gameObject.GetInstanceID())
        {
            return;
        }
        //获取gameObject 下面的TextMeshPro 物体。并设置文本
        levelNumber += num;
        Transform levelNumberTransform = transform.Find("levelNumber"); // 在当前物体下查找名为 levelNumber 的物体
        if (levelNumberTransform != null)
        {
            GameObject gameObject = levelNumberTransform.gameObject; // 获取 GameObject
            gameObject.GetComponent<TextMeshPro>().text = "Level " + levelNumber;
        }
        else
        {
            Debug.LogError("未找到名为 levelNumber 的物体");
        }
    }
    public void InitDefaultTraling()
    {
        // 获取当前player对象
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        // 获取Trailing对象
        Transform trailingComponent = player.transform.Find("Trailing");
        if (trailingComponent != null)
        {
            foreach (Transform child in trailingComponent)
            {
                Destroy(child.gameObject);
            }
            // 添加默认拖尾
            Instantiate(_defaultTrailing, trailingComponent.transform.position, _defaultTrailing.transform.rotation, trailingComponent);
        }
    }
    void OnTriggerEnter(Collider other)
    {

    }
    void OnCollisionEnter(Collision collision)
    {
        // 检查碰撞的对象是否有刚体组件
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
        string[] tagsToIgnore = { "Wall", "StopWall" };
        if (System.Array.Exists(tagsToIgnore, tag => collision.gameObject.CompareTag(tag))) return;
        if (rb != null)
        {
            // 启用重力并取消 Kinematic
            rb.isKinematic = false;
            rb.useGravity = true;
            // Debug.Log("Player:" + collision.gameObject.name);
            // 计算击飞方向
            Vector3 forceDirection = collision.transform.position - transform.position;
            forceDirection.y = 1; // 确保力有一个向上的分量
            forceDirection.Normalize();

            // 应用力
            // 力的大小可以根据需要调整
            rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);

        }
        //如果碰撞到的物体不是墙体则震动手机
        if (collision.gameObject.tag != "Wall" && Application.isMobilePlatform)
        {
            // 震动手机
            // vibrationManager.VibrateLightly();
            // 震动手机
            // Handheld.Vibrate();
        }

    }
    public void Quit()
    {
        Application.Quit();
    }
}





