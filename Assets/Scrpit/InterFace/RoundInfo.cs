using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TTSDK;
public class RoundInfo : MonoBehaviour
{
    private static RoundInfo _instance;
    public static RoundInfo Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RoundInfo>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("RoundInfo");
                    _instance = obj.AddComponent<RoundInfo>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public PublicGameData gameData => DataManager.Instance.gameInfo;

    //下一关
    public void OnNextRound(Action callback = null)
    {
        PublicGameData _gameInfo = DataManager.Instance.gameInfo;
        _gameInfo.player.coin += GameDataManager.Instance.goldenCoin;
        if (_gameInfo.roundInfo.levelSceneList.Count > _gameInfo.roundInfo.currentLevel)
        {
            //正常通过下一关
            if (_gameInfo.roundInfo.pickLevel == _gameInfo.roundInfo.currentLevel)
            {
                _gameInfo.roundInfo.currentLevel++;
                _gameInfo.roundInfo.pickLevel = _gameInfo.roundInfo.currentLevel;
#if UNITY_WEBGL && !UNITY_EDITOR
                TT.ReportAnalytics("FirstClearance", new Dictionary<string, object>
                {
                    { "roundID", _gameInfo.roundInfo.currentLevel - 1 },
                    { "deviceId", SystemInfo.deviceUniqueIdentifier }
                });
#endif

            }
            else
            {
                //选关通过下一关
                _gameInfo.roundInfo.pickLevel++;
            }

        }
        Debug.Log("当前关卡:" + _gameInfo.roundInfo.pickLevel);
        DataManager.Instance.SaveData();
        callback?.Invoke();
    }

    public LevelList OnGetCurrentLevel()
    {
        //获取当前场景
        int sceneId = DataManager.Instance.gameInfo.roundInfo.pickLevel;
        PublicGameData _gameInfo = DataManager.Instance.gameInfo;
        for (int i = 0; i < _gameInfo.roundInfo.levelSceneList.Count; i++)
        {

            if (sceneId == _gameInfo.roundInfo.levelSceneList[i].id)
            {
                return _gameInfo.roundInfo.levelSceneList[i];
            }
        }
        return null;
    }
    // 获取下一关信息
    public LevelList GetNextLevel()
    {
        //获取当前场景
        int currentSceneId = DataManager.Instance.gameInfo.roundInfo.pickLevel;
        PublicGameData _gameInfo = DataManager.Instance.gameInfo;

        // 遍历关卡列表找到当前关卡
        int currentIndex = -1;
        for (int i = 0; i < _gameInfo.roundInfo.levelSceneList.Count; i++)
        {
            if (currentSceneId == _gameInfo.roundInfo.levelSceneList[i].id)
            {
                currentIndex = i;
                break;
            }
        }

        // 如果找到当前关卡且不是最后一关，返回下一关信息
        if (currentIndex != -1 && currentIndex < _gameInfo.roundInfo.levelSceneList.Count - 1)
        {
            return _gameInfo.roundInfo.levelSceneList[currentIndex + 1];
        }

        return null;
    }
}
