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
            if (nextRecoveryTime == null)
            {
                nextRecoveryTime = DateTime.UtcNow; // 初始化为当前时间
            }
            TimeSpan timeSinceLast = DateTime.UtcNow - nextRecoveryTime;
            int recoveryCycles = Mathf.FloorToInt((float)timeSinceLast.TotalHours / RecoveryIntervalHours);
            // Debug.Log("时间差: " + recoveryCycles);
            // Debug.Log("更新时间: " + timeSinceLast.TotalHours);
            // Debug.Log("上次更新时间: " + nextRecoveryTime);
            if (recoveryCycles > 0)
            {
                // Debug.Log("修改nextRecoveryTime" + nextRecoveryTime);
                num = Mathf.Min(num + recoveryCycles, MaxVigour);
                // 更新最后恢复时间（保留余数时间）
                // nextRecoveryTime = nextRecoveryTime.AddHours(recoveryCycles * RecoveryIntervalHours);
                // 如果达到上限则更新时间戳到未来
                if (num >= MaxVigour)
                {
                    nextRecoveryTime = DateTime.UtcNow;
                }
                else
                {
                    // 如果没有达到最大值，继续增加恢复时间
                    nextRecoveryTime = nextRecoveryTime.AddHours(RecoveryIntervalHours); // 增加恢复时间
                }
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