using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [Serializable]
    public struct TodayVigour
    {
        public int num;
        public DateTime lastRecoveryTime; // 改为最后恢复时间
        public const int MaxVigour = 8;   // 最大体力值
        public const int RecoveryIntervalHours = 2; // 恢复间隔

        public TodayVigour(int defaultNum)
        {
            num = Mathf.Clamp(defaultNum, 0, MaxVigour);
            lastRecoveryTime = DateTime.UtcNow;
        }

        // 新增自动恢复计算方法
        public void CalculateAutoRecovery()
        {
            TimeSpan timeSinceLast = DateTime.UtcNow - lastRecoveryTime;
            int recoveryCycles = Mathf.FloorToInt((float)timeSinceLast.TotalHours / RecoveryIntervalHours);

            if (recoveryCycles > 0)
            {
                num = Mathf.Min(num + recoveryCycles, MaxVigour);
                // 更新最后恢复时间（保留余数时间）
                lastRecoveryTime = lastRecoveryTime.AddHours(recoveryCycles * RecoveryIntervalHours);
            }
        }
    }

    [Header("基础属性")]
    public int skinId = 1;
    public int artId = 0; // 玩家形象ID
    public int defaultHP = 4;
    public int defaultVigourNumber = 8;
    public int adWatchTime = 0;
    public TodayVigour todayVigour = new TodayVigour(8);
    public int coin = 0; // 金币
    public bool isGuide = false; // 是否完成新手引导

    public bool musicVisible = true;
    public bool soundVisible = true;
    public bool vibrationVisible = true;

    [Tooltip("最后保存时间")]
    public DateTime lastSaveTime = DateTime.UtcNow;

    // public void CheckDailyRefresh()
    // {
    //     if (DateTime.UtcNow >= todayVigour.reflashTime)
    //     {
    //         todayVigour = new TodayVigour(defaultVigourNumber);
    //         Debug.Log($"执行每日刷新 | UTC时间：{DateTime.UtcNow}");
    //     }
    // }
}