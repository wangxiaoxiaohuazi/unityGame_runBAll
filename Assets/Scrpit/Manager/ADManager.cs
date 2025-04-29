using System;
using System.Collections;
using System.Collections.Generic;
using TTSDK;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ADManager : MonoBehaviour
{
    private static ADManager _instance;
    //糖果
    private string BannerAdId = "57jm2c8nd5nd80nj36";
    private string InterstitialAdId = "5g5lic64666h50f5fh";
    private string RewardedAdId = "1hlk2c7emm4e3el75i";
    //球球
    //   private string BannerAdId = "";
    //     private string InterstitialAdId = "5g5lic64666h50f5fh";
    //     private string RewardedAdId = "";
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
        BannerADCreat();
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
        m_bannerAdIns.OnClose += () => Debug.Log($"Banner关闭 ");
        m_bannerAdIns.OnLoad += () => Debug.Log($"Banner加载成功 ");
        Debug.Log("Banner加载中");
    }

    void InterstitialADCreate()
    {
        var param = new CreateInterstitialAdParam { InterstitialAdId = InterstitialAdId };
        m_InterAdIns = TT.CreateInterstitialAd(param);
        m_InterAdIns.OnClose += () => Debug.Log("插屏广告关闭");
        m_InterAdIns.OnLoad += () => Debug.Log("插屏广告加载");
        m_InterAdIns.OnError += (code, message) => Debug.Log($"插屏错误 ： {code}  {message}");
    }
    //展示
    public void ShowBannerAd()
    {
        Debug.Log("展示banner");
        m_bannerAdIns.Show();
    }
    public void RewardADCreat()
    {
        string videoAdId = RewardedAdId;
        var param = new CreateRewardedVideoAdParam { AdUnitId = videoAdId };
        RewardedVideoAd = TT.CreateRewardedVideoAd(param);
        RewardedVideoAd.OnClose += (ended, count) => Debug.Log($"激励视频关闭 ended: {ended}, count: {count}");
        RewardedVideoAd.OnError += (errorCode, errorMessage) => Debug.Log($"激励视频错误 errorCode: {errorCode}");
    }

    //下一关
    public void OnADShow(Action callback = null)
    {
        try
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            RewardedVideoAd.Show();
            RewardedVideoAd.OnClose += (ended, count) =>
            {
                if (ended)
                {
                    //增加观看次数
                    DataManager.Instance.gameInfo.player.adWatchTime++;
                    DataManager.Instance.SaveData();
                    //广告回调
                    callback?.Invoke();
                }
            };
#else
            //非抖音小游戏环境直接执行回调
            DataManager.Instance.gameInfo.player.adWatchTime++;
            DataManager.Instance.SaveData();
            callback?.Invoke();
#endif


        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    public void OnIInterstitialADShow()
    {
        if (m_InterAdIns != null)
        {
            m_InterAdIns.Show();
        }
    }
}