using System;
using System.Collections;
using System.Collections.Generic;
using TTSDK;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ADManager : MonoBehaviour
{
    private static ADManager _instance;
    // //糖果
    // private string BannerAdId = "57jm2c8nd5nd80nj36";
    // private string InterstitialAdId = "5g5lic64666h50f5fh";
    // private string RewardedAdId = "1hlk2c7emm4e3el75i";
    //球球
    private string BannerAdId = "r2gmf5ye8j6t9g6m0h";
    private string InterstitialAdId = "fjokdmrfljdgk4kkl4";
    private string RewardedAdId = "3i388l5k9n492i8gne";
    private float lastADShowTime = 0f; // 添加一个字段来存储上次调用时间
    private float lastInterstitialShowTime = 0f; // 添加一个字段来存储上次插屏广告展示时间
    private Action _currentCallback;
    public static ADManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ADManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("ADManager");
                    _instance = obj.AddComponent<ADManager>();
                    DontDestroyOnLoad(obj);
                }
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
        CreatADParm();
    }

    public PublicGameData gameData => DataManager.Instance.gameInfo;
    public TTRewardedVideoAd RewardedVideoAd;
    public TTInterstitialAd m_InterAdIns = null;
    private TTBannerStyle m_style = new TTBannerStyle();
    private TTBannerAd m_bannerAdIns;
    public void CreatADParm()
    {
        RewardADCreat();
        InterstitialADCreate();
        // BannerADCreat();
    }
    public void BannerADCreat()
    {
        m_style.top = 10;
        m_style.left = 30;
        m_style.width = 120;
        var param = new CreateBannerAdParam
        {
            BannerAdId = BannerAdId,
            Style = m_style,
            AdIntervals = 60
        };
        m_bannerAdIns = TT.CreateBannerAd(param);
        m_bannerAdIns.OnError += (errorCode, errorMessage) => Debug.Log($"Banner加载失败 errorCode: {errorCode} errorMessage: {errorMessage}");
        m_bannerAdIns.OnLoad += () => Debug.Log($"Banner加载成功 ");
    }

    void InterstitialADCreate()
    {
        var param = new CreateInterstitialAdParam { InterstitialAdId = InterstitialAdId };
        m_InterAdIns = TT.CreateInterstitialAd(param);
        m_InterAdIns.OnLoad += () => Debug.Log("插屏广告加载");
        m_InterAdIns.OnError += (code, message) => Debug.Log($"插屏错误 ： {code}  {message}");
    }
    //展示
    public void ShowBannerAd()
    {
        // Debug.Log("展示banner");
        // if (m_bannerAdIns != null)
        // {
        //     m_bannerAdIns.Show();
        // }
        // else
        // {
        //     BannerADCreat();
        //     m_bannerAdIns.Show();
        // }
    }
    //关闭
    public void CloseBannerAd()
    {
        // Debug.Log("关闭banner");
        // m_bannerAdIns.Hide();
    }
    public void RewardADCreat()
    {
        string videoAdId = RewardedAdId;
        var param = new CreateRewardedVideoAdParam { AdUnitId = videoAdId };
        RewardedVideoAd = TT.CreateRewardedVideoAd(param);
        RewardedVideoAd.OnError += (errorCode, errorMessage) => Debug.Log($"激励视频错误 errorCode: {errorCode}");
    }

    //下一关
    public void OnADShow(Action callback = null)
    {
        if (Time.time - lastADShowTime < 2f)
        {
            Debug.Log("防抖中，无法再次调用OnADShow");
            return;
        }

        lastADShowTime = Time.time;
        try
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (RewardedVideoAd == null)
            {
                RewardADCreat();
            }

            RewardedVideoAd.OnClose -= HandleRewardedAdClose;
            _currentCallback = callback;
            RewardedVideoAd.OnClose += HandleRewardedAdClose;
            
            RewardedVideoAd.Show();
#else
            DataManager.Instance.gameInfo.player.adWatchTime++;
            DataManager.Instance.SaveData();
            callback?.Invoke();
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"广告展示错误: {e.Message}");
            _currentCallback = null;
        }
    }



    private void HandleRewardedAdClose(bool ended, int count)
    {
        if (ended)
        {
            DataManager.Instance.gameInfo.player.adWatchTime++;
            DataManager.Instance.SaveData();
            _currentCallback?.Invoke();
        }
        else
        {
            Debug.Log($"观看不完整: {count}");
        }
        _currentCallback = null;
    }

    public void OnIInterstitialADShow()
    {
        // 检查是否在防抖时间内 抖音要求30秒内展示一次
        if (Time.time - lastInterstitialShowTime < 30f) // 30秒的防抖时间
        {
            Debug.Log("防抖中，无法再次调用OnIInterstitialADShow");
            return;
        }
        lastInterstitialShowTime = Time.time; // 更新上次展示时间

        if (m_InterAdIns != null)
        {
            m_InterAdIns.Show();
        }
        else
        {
            InterstitialADCreate();
            m_InterAdIns.Show();
        }
    }
}