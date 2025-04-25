using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private float idleTime = 0f; // 记录无输入时间
    private float pauseThreshold = 0.5f; // 设置暂停阈值（例如 2 秒）

    private PanelManager panelManager;
    internal bool isInvincible;

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
        panelManager = FindObjectOfType<PanelManager>();
        // 可选：添加null检查
        // if (panelManager == null)
        //     Debug.LogError("Player对象未找到，请检查标签设置");
    }
    // Start is called before the first frame update
    void Start()
    {
        // 初始化
        Time.timeScale = 1; // 确保游戏开始时是运行状态
        if (panelManager == null)
            panelManager = FindObjectOfType<PanelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // checkInput();
    }
    public void checkInput()
    {
        if (!GameDataManager.Instance.isPause)
        {
            if (GameDataManager.Instance.playerLives <= 0 || GameDataManager.Instance.endGameVisible)
            {
                return;
            }
            // 检测输入
            if (Input.anyKey || Input.touchCount > 0)
            {
                // 有输入，重置无输入时间
                idleTime = 0f;
                Time.timeScale = 1; // 恢复游戏
            }
            // else
            // {
            //     // 没有输入，增加无输入时间
            //     idleTime += Time.deltaTime;

            //     // 检查是否超过暂停阈值
            //     if (idleTime >= pauseThreshold)
            //     {
            //         Time.timeScale = 0; // 暂停游戏
            //     }
            // }
        }
        // 检测 R 键输入
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnResetGame(); // 调用重置游戏事件
        }

    }

    // 在GameManager类中修改OnResetGame方法
    public void OnResetGame()
    {
        GameDataManager.Instance.endGameVisible = false;
        
        // 在场景加载完成后执行重置逻辑
        AddressablesLoaderManager.Instance.ReloadCurrentScene(progress =>
        {
            if (progress >= 1f)  // 场景加载完成
            {
                // 通过场景加载事件来处理重置逻辑
                SceneManager.sceneLoaded += OnResetSceneLoaded;
            }
            Debug.Log($"重载进度: {progress:P0}");
        });

        Debug.Log("重开场景");
    }

    private void OnResetSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 确保只执行一次
        SceneManager.sceneLoaded -= OnResetSceneLoaded;
        
        // 重置玩家数据
        var playerObj = GameObject.Find("Player");
        if (playerObj != null && playerObj.TryGetComponent<player>(out var playerComponent))
        {
            GameDataManager.Instance.ChangePlayerLives(playerComponent.blood);
        }
        GameDataManager.Instance.goldenCoin = 0;
        Time.timeScale = 1;
    }
}
