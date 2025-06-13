using UnityEngine;
using System.Collections.Generic;
using TTSDK;
using System;

public class AnalyticsManager : MonoBehaviour
{
    private static AnalyticsManager _instance;
    public static AnalyticsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("AnalyticsManager");
                _instance = obj.AddComponent<AnalyticsManager>();
                DontDestroyOnLoad(obj);
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

    // 添加事件冷却时间配置（单位：秒）
    private readonly Dictionary<string, float> _eventCooldowns = new Dictionary<string, float>
    {
        { "UserDie", 10f },      // 死亡事件60秒冷却
        { "UserRevive", 60f },   // 复活事件60秒冷却
        { "FirstClearance", 0f } // 首次通关不限制
    };

    // 记录事件最后触发时间
    private readonly Dictionary<string, DateTime> _lastEventTimes = new Dictionary<string, DateTime>();

    // 检查事件是否可以触发
    private bool CanTriggerEvent(string eventName)
    {
        if (!_eventCooldowns.ContainsKey(eventName))
            return true;

        float cooldown = _eventCooldowns[eventName];
        if (cooldown <= 0)
            return true;

        if (!_lastEventTimes.ContainsKey(eventName))
            return true;

        TimeSpan timeSinceLastTrigger = DateTime.Now - _lastEventTimes[eventName];
        return timeSinceLastTrigger.TotalSeconds >= cooldown;
    }

    // 更新事件触发时间
    private void UpdateEventTime(string eventName)
    {
        _lastEventTimes[eventName] = DateTime.Now;
    }

    // 修改通用上报方法
    private void ReportEvent(string eventName, Dictionary<string, object> parameters)
    {
        if (!CanTriggerEvent(eventName))
            return;
        Debug.Log("事件名：=====" + eventName);
        Debug.Log("上报事件:-----" + eventName == "UserDie" ? "用户死亡" : eventName == "UserRevive" ? "用户复活" : "首次通关");
#if UNITY_WEBGL && !UNITY_EDITOR
        TT.ReportAnalytics(eventName, parameters);
#endif
        UpdateEventTime(eventName);
    }

    // 用户死亡事件上报
    public void ReportUserDie(int roundId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "roundId", roundId },
            { "deviceId", SystemInfo.deviceUniqueIdentifier }
        };

        ReportEvent("UserDie", parameters);
    }

    // 用户复活事件上报
    public void ReportUserRevive(int roundId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "roundId", roundId },
            { "deviceId", SystemInfo.deviceUniqueIdentifier }
        };

        ReportEvent("UserRevive", parameters);
    }

    // 首次通关事件上报
    public void ReportFirstClearance(int roundId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "roundID", roundId },
            { "deviceId", SystemInfo.deviceUniqueIdentifier }
        };

        ReportEvent("FirstClearance", parameters);
    }
}