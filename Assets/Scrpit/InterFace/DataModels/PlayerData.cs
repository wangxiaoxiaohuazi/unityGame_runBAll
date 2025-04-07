using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [Serializable]
    public struct TodayVigour
    {
        public int num;
        public DateTime reflashTime;
        
        public TodayVigour(int defaultNum)
        {
            num = defaultNum;
            reflashTime = DateTime.UtcNow.Date.AddDays(1);
        }
    }

    [Header("基础属性")]
    public int skinId = 1;
    public int defaultHP = 4;
    public int defaultVigourNumber = 10;
    public TodayVigour todayVigour = new TodayVigour(10);
    public int coin = 0; // 金币

    [Tooltip("最后保存时间")]
    public DateTime lastSaveTime = DateTime.UtcNow;

    public void CheckDailyRefresh()
    {
        if (DateTime.UtcNow >= todayVigour.reflashTime)
        {
            todayVigour = new TodayVigour(defaultVigourNumber);
            Debug.Log($"执行每日刷新 | UTC时间：{DateTime.UtcNow}");
        }
    }
}