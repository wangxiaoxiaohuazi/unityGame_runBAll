using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * 游戏内数据管理器
 *控制游戏内发生的临时数据  
 */
public class GameDataManager : MonoBehaviour
{
    // 单例实例
    public static GameDataManager Instance { get; private set; }
    [Header("游戏属性变量")]
    // 游戏属性变量
    public int playerScore; // 玩家得分
    public float playerLives; // 玩家生命值
    public string currentLevel; // 当前关卡名称
    public float energy; //能量值
    public bool isInvincible = false; // 是否无敌
    public int goldenCoin = 0; // 金币数量
    private float lastDamageTime = 0f; // 上次处理伤害的时间
    public float damageDebounceTime = 1f; // 防抖时间，单位为秒
    public bool endGameVisible = false;// 游戏是否结束
    //公共变色色值
    public Color32 BaseColor;
    //禁止碰撞变色色值
    public Color32 NoCollisionColor;
    //可碰撞变色色值
    public Color32 CollisionColor;

    public GameObject _player;// 玩家对象

    // 在游戏中常驻
    private void Awake()
    {
        // 确保单例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 在场景切换时不销毁
        }
        else
        {
            Destroy(gameObject); // 如果已经存在实例，则销毁新的实例
        }
    }

    // Start is called before the first frame update

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 订阅场景加载事件
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 取消订阅
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 场景加载完成后重新绑定玩家对象
        _player = GameObject.FindGameObjectWithTag("Player");

        // 可选：添加null检查
        if (_player == null)
            Debug.LogError("Player对象未找到，请检查标签设置");
    }
    void Start()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }
        Debug.Log("游戏数据初始化完成:" + _player.name);
        player playerComponent = _player.GetComponent<player>();
        // 初始化游戏数据
        playerScore = 0; // 初始化得分
        playerLives = playerComponent.blood; // 初始化生命值;
        currentLevel = "Level1"; // 设置初始关卡
        // 禁用垂直同步
        QualitySettings.vSyncCount = 0;
        // 设置目标帧率为30
        Application.targetFrameRate = 30;
    }
    // Update is called once per frame
    void Update()
    {
        // 更新游戏数据（如果需要）
    }

    public void SaveGameData()
    {
        // 保存游戏数据的逻辑
        PlayerPrefs.SetInt("PlayerScore", playerScore);
        PlayerPrefs.SetFloat("PlayerLives", playerLives);
        PlayerPrefs.SetString("CurrentLevel", currentLevel);
        PlayerPrefs.SetInt("goldenCoin", goldenCoin);
        PlayerPrefs.Save(); // 保存到 PlayerPrefs
    }

    public void LoadGameData()
    {
        player playerComponent = _player.GetComponent<player>();
        // 加载游戏数据的逻辑
        playerScore = PlayerPrefs.GetInt("PlayerScore", 0); // 默认值为 0
        playerLives = PlayerPrefs.GetFloat("PlayerLives", playerComponent.blood); // 默认值为 3
        currentLevel = PlayerPrefs.GetString("CurrentLevel", "Level1"); // 默认值为 "Level1"
        goldenCoin = PlayerPrefs.GetInt("goldenCoin", 0); // 默认值为 0
    }
    public void ChangePlayerLives(float change)
    {
        // 如果玩家处于无敌状态，则不改变生命值
        if (isInvincible) return;
        if (_player == null)
        {
            Debug.LogWarning("玩家对象已被销毁，无法更改生命值。");
            return; // 如果玩家对象为 null，直接返回
        }
        player playerComponent = _player.GetComponent<player>();
        // 获取当前时间
        float currentTime = Time.time;
        if (change > 0)
        {
            // 处理伤害逻辑
            playerLives += change; // 更新生命值
            playerLives = Mathf.Clamp(playerLives, 0, playerComponent.blood); // 确保生命值不低于 0
        }

        // 检查是否超过防抖时间
        if (currentTime - lastDamageTime >= damageDebounceTime)
        {

            if (change < 0)
            {
                // 处理伤害逻辑
                playerLives += change; // 更新生命值
                playerLives = Mathf.Clamp(playerLives, 0, playerComponent.blood); // 确保生命值不低于 0
                // 显示受伤特效面板
                PanelManager panelManager = FindObjectOfType<PanelManager>();
                panelManager.ShowPanel(panelManager.panels[2]);
                //减少level
                _player.GetComponent<player>().levelNumber = Mathf.RoundToInt(_player.GetComponent<player>().levelNumber / 2f); // 四舍五入取整
                //减少积分
                _player.GetComponent<player>().scoreNumber = Mathf.Round(_player.GetComponent<player>().scoreNumber * 0.8f * 100) / 100; // 保留两位小数
                // 启动协程
                StartCoroutine(HidePanelCoroutine());
                // 震动手机
                // Handheld.Vibrate();
            }


            // 更新上次处理伤害的时间
            lastDamageTime = currentTime;
        }

        // 更新 UI
        PanelGameCondition panel = FindObjectOfType<PanelGameCondition>();
        if (panel != null)
        {
            panel.UpdatePlayerLives(playerLives);
        }
        if (playerLives <= 0)
        {
            // 游戏结束逻辑
            EndGame endGame = FindObjectOfType<EndGame>();
            if (endGame != null)
            {
                endGame.endGame();
                endGameVisible = true;
            }
        }
    }
    public void ChangePlayerEnergy(float change)
    {
        energy += change;

        // energy = Mathf.Clamp(energy, 0.01f, 10f);
        // 更新 UI
        PanelGameCondition panel = FindObjectOfType<PanelGameCondition>();
        if (panel != null)
        {
            panel.UpdatePlayerPower(energy);
        }

    }
    // 调用此方法以在 0.5 秒后隐藏面板
    private IEnumerator HidePanelCoroutine()
    {
        yield return new WaitForSeconds(0.5f); // 等待 0.5 秒
        PanelManager panelManager = FindObjectOfType<PanelManager>();
        panelManager.HidePanel(panelManager.panels[2]);
    }

}


