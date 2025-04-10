using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            _gameInfo.roundInfo.currentLevel++;
        }
        DataManager.Instance.SaveData();
        PlayerInfo.Instance.AddVigourNumber(-3);
        callback?.Invoke();
    }

    public int? OnGetCurrentLevel()
    {
        //获取当前场景名
        string sceneName = SceneManager.GetActiveScene().name;
        PublicGameData _gameInfo = DataManager.Instance.gameInfo;
        for (int i = 0; i < _gameInfo.roundInfo.levelSceneList.Count; i++)
        {
            Debug.Log("场景：===" + sceneName);
            Debug.Log("场景：===" + _gameInfo.roundInfo.levelSceneList[i].name);
            if (sceneName == _gameInfo.roundInfo.levelSceneList[i].scenePath)
            {
                return _gameInfo.roundInfo.levelSceneList[i].id;
            }
        }
        return null;
    }
}
