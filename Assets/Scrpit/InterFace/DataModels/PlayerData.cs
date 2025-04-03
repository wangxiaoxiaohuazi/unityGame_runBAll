using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [Header("基础属性")]
    public int skinId = 1;
    public int defaultHP = 4;
    public int defaultVigourNumber = 10;
    public TodayVigour todayVigour = new TodayVigour();
    public int coin = 0; // 金币

    [Tooltip("最后保存时间")]
    public DateTime lastSaveTime = DateTime.UtcNow;
}
public class TodayVigour
{
    public int num = 10;
    // 或使用精确的次日零点刷新
    public DateTime reflashTime = DateTime.Today.AddDays(1);
}