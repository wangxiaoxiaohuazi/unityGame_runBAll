using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [Serializable]
    public class TodayVigour
    {
        private int _num = 8;
        public int num
        {
            get => _num;
            set => _num = Mathf.Abs(value); // 确保值始终为正数
        }
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
            if (_nextRecoveryTime < DateTime.UtcNow && DataManager.Instance.gameInfo.player.todayVigour.num < MaxVigour)
            {
                var hours = (DateTime.UtcNow - _nextRecoveryTime).TotalHours;
                Debug.Log("体力恢复" + hours);
                var recoveryAmount = Mathf.FloorToInt((float)(hours / RecoveryIntervalHours));
                if (recoveryAmount < 0)
                {
                    recoveryAmount = -recoveryAmount;
                }
                num += recoveryAmount + 1;
                if (num > MaxVigour)
                {
                    num = MaxVigour;
                }
                DataManager.Instance.gameInfo.player.todayVigour.num = num; // 更新体力值
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


    public TodayVigour todayVigour = new TodayVigour();

}