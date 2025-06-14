using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [Serializable]
    public class TodayVigour
    {
        public int num = 8;
        // 额外的属性和方法
        public DateTime nextRecoveryTime
        {
            get
            {
                if (string.IsNullOrEmpty(nextRecoveryTimeString))
                {
                    return DateTime.UtcNow; // 如果未设置，返回当前 UTC 时间
                }
                return DateTime.Parse(nextRecoveryTimeString).ToUniversalTime(); // 确保返回 UTC 时间
            }
            set
            {
                nextRecoveryTimeString = value.ToString("o"); // 转换为字符串
            }
        }
        public const int MaxVigour = 8;   // 最大体力值
        public const int RecoveryIntervalHours = 2; // 恢复间隔
        public string nextRecoveryTimeString; // 使用字符串存储时间
        // 新增自动恢复计算方法
        public void CalculateAutoRecovery()
        {
            var _nextRecoveryTime = DataManager.Instance.gameInfo.player.todayVigour.nextRecoveryTime; // 恢复间隔，单位为小时
            // 如果nextRecoveryTime未设置，初始化为当前时间
            if (string.IsNullOrEmpty(nextRecoveryTimeString))
            {
                _nextRecoveryTime = DateTime.UtcNow;
            }
            // 计算距离上次恢复时间过去了多久
            TimeSpan timeSinceLast = _nextRecoveryTime - DateTime.UtcNow;

            // 如果已经超过恢复时间
            if (timeSinceLast <= TimeSpan.Zero)
            {
                // 计算应该恢复的体力值
                int recoveryCycles = (int)(timeSinceLast.TotalHours / RecoveryIntervalHours) + 1;
                // 更新下一次恢复时间
                if (recoveryCycles >= MaxVigour)
                {
                    _nextRecoveryTime = DateTime.UtcNow;
                }
                else
                {
                    Debug.Log("Next recovery time: " + nextRecoveryTime);

                    _nextRecoveryTime = DateTime.UtcNow.AddHours(RecoveryIntervalHours);
                }
                if (DataManager.Instance.gameInfo.player.todayVigour.num >= MaxVigour)
                {
                    _nextRecoveryTime = DateTime.UtcNow;
                }
                DataManager.Instance.gameInfo.player.todayVigour.nextRecoveryTime = _nextRecoveryTime;
                DataManager.Instance.gameInfo.player.todayVigour.num = recoveryCycles;
            }
        }
    }

    [Header("基础属性")]
    public int skinId = 1;
    public int artId = 0; // 玩家形象ID
    public int defaultHP = 4;
    public int defaultVigourNumber = 8;
    public int adWatchTime = 0;
    public int coin = 0; // 金币
    public bool isGuide = false; // 是否完成新手引导

    public bool musicVisible = true;
    public bool soundVisible = true;
    public bool vibrationVisible = true;
    public bool dailyReward = false;

    [Tooltip("最后保存时间")]
    public string lastSaveTimeString;
    public DateTime lastSaveTime
    {
        get => string.IsNullOrEmpty(lastSaveTimeString) ? DateTime.UtcNow : DateTime.Parse(lastSaveTimeString);
        set => lastSaveTimeString = value.ToString("o"); // 转换为字符串
    }
    // public void CheckDailyRefresh()
    // {
    //     if (DateTime.UtcNow >= todayVigour.reflashTime)
    //     {
    //         todayVigour = new TodayVigour(defaultVigourNumber);
    //         Debug.Log($"执行每日刷新 | UTC时间：{DateTime.UtcNow}");
    //     }
    // }

    public TodayVigour todayVigour = new TodayVigour();

}