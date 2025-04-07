using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // 添加DateTime需要
using System.Text;
using TTSDK;
/**
 * 数据管理器
 *控制需要留存的数据  
 */
public class DataManager : MonoBehaviour
{
    #region 单例模式改进
    private static DataManager _instance;
    private static readonly object _lock = new object();
    private static bool _isApplicationQuitting = false;
    public static DataManager Instance
    {
        get
        {
            if (_isApplicationQuitting)
            {
                Debug.LogWarning("[DataManager] 程序已退出，实例不可用");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<DataManager>();
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject("DataManager");
                        _instance = singleton.AddComponent<DataManager>();
                        DontDestroyOnLoad(singleton);
                    }
                    else
                    {
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }
                return _instance;
            }
        }
    }
    #endregion
    public SceneInfo[] scenesList;
    private const string SAVE_FILE = "game_data.dat";
    // 当前游戏数据实例
    private PublicGameData _gameData;
    public PublicGameData gameInfo => _gameData;
    public event Action OnDataChanged;

    #region 生命周期管理
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
#if UNITY_WEBGL && !UNITY_EDITOR
        TT.InitSDK();
#endif
        _instance = this;
        DontDestroyOnLoad(gameObject);
        Initialize();
        GetAllBuildScenes();
    }

    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
        ForceSave();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) ForceSave();
    }
    #endregion

    #region 增强保存系统
    [SerializeField] private float _autoSaveInterval = 300f;
    private Coroutine _autoSaveCoroutine;

    private void Initialize()
    {
        LoadData();
        // InitDefaultValues();
        StartAutoSave();
    }

    private void StartAutoSave()
    {
        if (_autoSaveCoroutine != null)
            StopCoroutine(_autoSaveCoroutine);

        _autoSaveCoroutine = StartCoroutine(AutoSaveRoutine());
    }

    private IEnumerator AutoSaveRoutine()
    {
        var wait = new WaitForSecondsRealtime(_autoSaveInterval);
        while (true)
        {
            yield return wait;
            ForceSave();
        }
    }

    public void ForceSave()
    {
        Debug.Log("ForceSave");
        try
        {
            // 使用PlayerPrefs替代文件存储
            string json = JsonUtility.ToJson(_gameData);
#if UNITY_WEBGL && !UNITY_EDITOR
            TT.PlayerPrefs.SetString("GameData", json);
            TT.PlayerPrefs.Save(); // 确保立即写入
#else
            PlayerPrefs.SetString("GameData", json);
            PlayerPrefs.Save(); // 确保立即写入
#endif
            OnDataChanged?.Invoke();
            Debug.Log("数据已保存到PlayerPrefs");
        }
        catch (Exception e)
        {
            Debug.LogError($"强制保存失败: {e.Message}");
        }
    }
    #endregion
    private void LoadData()
    {
        try
        {
            if (PlayerPrefs.HasKey("GameData"))
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                          string json = TT.PlayerPrefs.GetString("GameData");
#else
                string json = PlayerPrefs.GetString("GameData");

#endif
                _gameData = JsonUtility.FromJson<PublicGameData>(json);
                MigrateDataIfNeeded();
            }
            else
            {
                Debug.Log("没有找到保存的数据，创建新的数据");
                CreateNewData();
            }
            // 添加时间校验
            CheckDailyRefresh();
        }
        catch (Exception e)
        {
            Debug.LogError($"数据加载失败: {e.Message}");
            CreateNewData();
        }
    }
    public void CheckDailyRefresh()
    {
        // 检查是否需要刷新每日数据
        // if (DateTime.Now >= todayVigour.reflashTime)
        // {
        //     todayVigour.num = defaultVigourNumber;
        //     todayVigour.reflashTime = DateTime.Today.AddDays(1);
        //     Debug.Log($"执行每日刷新 | 当前时间：{DateTime.Now} | 刷新时间：{todayVigour.reflashTime}");
        // }
    }
    private void NotifyDataChanged()
    {
        OnDataChanged?.Invoke();
    }

    private void CreateNewData()
    {
        _gameData = new PublicGameData();
        SaveData();
    }
    private void MigrateDataIfNeeded()
    {
        // 数据版本迁移示例
        if (_gameData.dataVersion < 2)
        {
            // 添加新版本需要的字段
            _gameData.dataVersion = 2;
            _gameData.createTime = DateTime.UtcNow;
        }
    }

    public void SaveData()
    {
        try
        {
            string json = JsonUtility.ToJson(_gameData);
#if UNITY_WEBGL && !UNITY_EDITOR
            TT.PlayerPrefs.SetString("GameData", json);
            TT.PlayerPrefs.Save(); // 确保立即写入
             Debug.Log("webgl保存成功");
#else
            PlayerPrefs.SetString("GameData", json);
            PlayerPrefs.Save(); // 确保立即写入
            Debug.Log("普通保存成功");
#endif
            NotifyDataChanged(); // 添加通知
            Debug.Log($"数据保存成功:");
        }
        catch (Exception e)
        {
            Debug.LogError($"数据保存失败: {e.Message}");
        }
    }

    // 获取所有构建场景信息
    public void GetAllBuildScenes()
    {
        List<SceneInfo> scenes = new List<SceneInfo>();

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);

            scenes.Add(new SceneInfo
            {
                BuildIndex = i,
                SceneName = sceneName,
                ScenePath = path
            });
        }
        scenesList = scenes.ToArray();
    }
    // 场景信息结构
    public struct SceneInfo
    {
        public int BuildIndex;
        public string SceneName;
        public string ScenePath;
    }

}