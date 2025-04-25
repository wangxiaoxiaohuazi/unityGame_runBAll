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

        // // 先进行自动恢复计算
        // vigour.CalculateAutoRecovery();

        // // 保存更新后的数据
        // DataManager.Instance.SaveData();

        return vigour.num;
    }

    public void AddVigourNumber(int num, Action callback = null)
    {
        if (Instance == null || Instance.gameData?.player?.todayVigour == null)
        {
            Debug.LogError("PlayerInfo 数据未初始化!");
            return;
        }

        var vigour = Instance.gameData.player.todayVigour;

        // 自动恢复计算
        vigour.CalculateAutoRecovery();

        // 体力变化前记录旧值
        int oldValue = vigour.num;

        // 应用变化并限制范围
        vigour.num = Mathf.Clamp(vigour.num + num, 0, PlayerData.TodayVigour.MaxVigour);

        // 如果体力有变化则更新时间戳
        if (vigour.num != oldValue)
        {
            vigour.lastRecoveryTime = DateTime.UtcNow;
        }
        //体力小于1，则弹出体力不足提示
        if (num < 0 && DataManager.Instance.gameInfo.player.todayVigour.num < 1)
        {
            PanelManager manager = FindObjectOfType<PanelManager>();
            manager.ShowPop(null, null, () =>
            {
                ADManager.Instance.OnADShow(() =>
                {
                    Debug.Log("广告播放完成");
                    PlayerInfo.Instance.AddVigourNumber(5);
                    manager.HidePanel(PanelManager.PanelName.PanelPop);
                });
            }, null);
        }
        //临时暂停体力扣除
        DataManager.Instance.gameInfo.player.todayVigour.num += num;
        // Debug.Log("体力增加：" + DataManager.Instance.gameInfo.player.todayVigour.num);


        DataManager.Instance.SaveData();
        callback?.Invoke();
    }
    public void SaveCoin(int num, Action callback = null)
    {
        Debug.Log("金币增加：" + DataManager.Instance.gameInfo.player.coin);
        if (Instance == null || Instance.gameData?.player?.todayVigour == null)
        {
            Debug.LogError("PlayerInfo 数据未初始化!");
        }
        if (num < 0 && num + DataManager.Instance.gameInfo.player.coin < 0)
        {
            Debug.LogError("金币不足");
            return;
        }
        DataManager.Instance.gameInfo.player.coin += num;
        DataManager.Instance.SaveData();
        callback?.Invoke();
    }
    public void rSetDefaultSkin(int id, Action callback = null)
    {
        gameData.player.skinId = id;
        DataManager.Instance.SaveData();
        callback?.Invoke();
    }
    public void rSetDefaultBodyArt(int id, Action callback = null)
    {
        gameData.player.artId = id;
        DataManager.Instance.SaveData();
        callback?.Invoke();
    }

}
