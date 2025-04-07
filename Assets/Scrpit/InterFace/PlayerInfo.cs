using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    private static PlayerInfo _instance;
    public static PlayerInfo Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerInfo>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("PlayerInfo");
                    _instance = obj.AddComponent<PlayerInfo>();
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
    //获取体力数量
    public int GetVigourNumber()
    {
        if (Instance == null || Instance.gameData?.player?.todayVigour == null)
        {
            Debug.LogError("PlayerInfo 数据未初始化!");
            return 0;
        }

        var vigour = Instance.gameData.player.todayVigour;
        if (vigour.reflashTime.Date < DateTime.Today)
        {
            vigour.num = Instance.gameData.player.defaultVigourNumber;
            vigour.reflashTime = DateTime.Today.AddDays(1);
            Debug.Log($"跨天刷新体力，当前时间：{DateTime.Now}");
            DataManager.Instance.SaveData();
        }
        return vigour.num;
    }
    public void AddVigourNumber(int num)
    {
        if (Instance == null || Instance.gameData?.player?.todayVigour == null)
        {
            Debug.LogError("PlayerInfo 数据未初始化!");
        }
        //临时暂停体力扣除
        // DataManager.Instance.gameInfo.player.todayVigour.num += num;
        Debug.Log("体力增加：" + DataManager.Instance.gameInfo.player.todayVigour.num);
        DataManager.Instance.SaveData();
    }
}
