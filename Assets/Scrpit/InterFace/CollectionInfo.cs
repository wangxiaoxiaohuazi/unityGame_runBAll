using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionInfo : MonoBehaviour
{
    private static CollectionInfo _instance;
    public static CollectionInfo Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CollectionInfo>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("CollectionInfo");
                    _instance = obj.AddComponent<CollectionInfo>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    void Awake()
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


    public void rUnlockSkin(int id, Action callback = null)
    {
        if (Instance == null || Instance.gameData?.player?.todayVigour == null)
        {
            Debug.LogError("PlayerInfo 数据未初始化!");
        }
        gameData.collections.skins.ForEach(item =>
        {
            if (item.id == id)
            {
                item.isLocked = true;
            }
        });
        DataManager.Instance.SaveData();
        callback?.Invoke();
    }
       public void rUnlockBodyArt(int id, Action callback = null)
    {
        if (Instance == null || Instance.gameData?.player?.todayVigour == null)
        {
            Debug.LogError("PlayerInfo 数据未初始化!");
        }
        gameData.collections.bodyParts.ForEach(item =>
        {
            if (item.id == id)
            {
                item.isLocked = true;
            }
        });
        DataManager.Instance.SaveData();
        callback?.Invoke();
    }
}
