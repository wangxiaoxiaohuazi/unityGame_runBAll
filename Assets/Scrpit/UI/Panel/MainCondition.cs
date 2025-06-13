using System;
using TMPro;
using TTSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainCondition : MonoBehaviour
{
    public GameObject scrollviewNode = null;
    public GameObject sideView = null;
    public DataManager.SceneInfo[] scenesList;

    public Text coinNum = null;
    public Text VigourNumber = null;
    public TextMeshProUGUI levelText = null;
    private PublicGameData _gameData;
    public Transform StartGame = null;
    public GameObject Countdown = null;
    // Start is called before the first frame update
    void Start()
    {
        _gameData = DataManager.Instance.gameInfo;
        InitScrollviewNode();
        if (RoundInfo.Instance.OnGetCurrentLevel() != null)
        {
            levelText.text = "第" + RoundInfo.Instance.OnGetCurrentLevel().id + "关";
        }
        UpDataVigourNumber();
    }
    void OnEnable()
    {
        ADManager.Instance.ShowBannerAd();
        UpDataVigourNumber();
        InitSideNode();
    }
    void OnDisable()
    {
        ADManager.Instance.CloseBannerAd();
    }
    void OnDestroy()
    {
        ADManager.Instance.CloseBannerAd();
    }
    // Update is called once per frame
    void Update()
    {

        coinNum.text = _gameData.player.coin.ToString();
        // 计算并更新倒计时
        UpdateCountdown();

    }
    public void ResetTime()
    {
        _gameData.player.todayVigour.nextRecoveryTime = DateTime.Now.AddHours(-3);
        DataManager.Instance.SaveData();
    }
    public void UpDataVigourNumber()
    {
        if (PlayerInfo.Instance == null)
        {
            return;
        }
        if (VigourNumber == null)
        {
            return;
        }
        if (_gameData == null || _gameData.player == null)
        {
            return;
        }

        VigourNumber.text =
                  PlayerInfo.Instance.GetVigourNumber() + "/" + _gameData.player.defaultVigourNumber;
        StartGame.Find("Start").gameObject.SetActive(PlayerInfo.Instance.GetVigourNumber() > 0);
        StartGame.Find("End").gameObject.SetActive(PlayerInfo.Instance.GetVigourNumber() < 1);
    }
    private void InitScrollviewNode()
    {
        //获取Scenes文件下的所有场景文件 将其添加到scrollview中
        scenesList = DataManager.Instance.scenesList;
    }

    private void InitSideNode()
    {
        LaunchOption launchOption = TT.GetLaunchOptionsSync();
        Debug.Log("场景值" + launchOption.Scene);
        if (launchOption?.Scene == "021036")
        {
            sideView.transform.Find("GetReward").gameObject.SetActive(true);
            sideView.transform.Find("NavigateSideButton").gameObject.SetActive(false);
            return;
        }
        sideView.transform.Find("NavigateSideButton").gameObject.SetActive(true);
        sideView.transform.Find("GetReward").gameObject.SetActive(false);
    }

    public void OnStartGameClick(string sceneName)
    {
        if (PlayerInfo.Instance.GetVigourNumber() < 1)
        {
            OnShowADEnergy();
            return;
        }
        GameManager _gamemanager = new GameManager();
        PanelManager manager = FindObjectOfType<PanelManager>();
        manager.showFight();
        PlayerInfo.Instance.AddVigourNumber(-1);
    }

    public void OnSideViewVisible()
    {
        sideView.SetActive(!sideView.activeSelf);
    }

    public void OnGetRwardClick()
    {
        PanelManager manager = FindObjectOfType<PanelManager>();
        //获取300金币
        if (!DataManager.Instance.gameInfo.player.dailyReward)
        {
            PlayerInfo.Instance.SaveCoin(300, () =>
            {
                coinNum.text = _gameData.player.coin.ToString();
                manager?.ShowTips("奖励已发放成功");
                DataManager.Instance.gameInfo.player.dailyReward = true;
            });
        }
        else
        {
            manager?.ShowTips("已领取");
        }
    }


    public void OnShowPanelRound()
    {
        PanelManager manager = FindObjectOfType<PanelManager>();
        manager.ShowPanel(PanelManager.PanelName.PanelRound);
    }
    public void OnShowPanelSkin()
    {
        PanelManager manager = FindObjectOfType<PanelManager>();
        manager.ShowSkin();
    }
    public void OnShowPanelBodyArt()
    {
        PanelManager manager = FindObjectOfType<PanelManager>();
        manager.ShowBodyArt();
    }
    public void OnShowPanelSetting()
    {
        PanelManager manager = FindObjectOfType<PanelManager>();
        manager.ShowPanel(PanelManager.PanelName.PanelSetting);
    }
    public void OnShowADEnergy()
    {
        PanelManager manager = FindObjectOfType<PanelManager>();
        manager.ShowPop(null, null, () =>
        {
            ADManager.Instance.OnADShow(() =>
            {
                Debug.Log("广告播放完成");
                PlayerInfo.Instance.AddVigourNumber(8);
                UpDataVigourNumber();
                manager.HidePanel(PanelManager.PanelName.PanelPop);

            });

        }, null);
    }
    public void OnShowTTSide()
    {
        TTFunction ttFunction = new TTFunction();
        ttFunction.NavigateToTTSidebar();
    }
    private void UpdateCountdown()
    {
        if (Countdown == null || _gameData?.player?.todayVigour == null)
        {
            return;
        }

        var nextRecoveryTime = _gameData.player.todayVigour.nextRecoveryTime;
        var timeDiff = nextRecoveryTime - DateTime.UtcNow;

        // 如果时间差小于等于0或体力已满，直接隐藏倒计时
        if (timeDiff <= TimeSpan.Zero ||
    PlayerInfo.Instance?.GetVigourNumber() >= _gameData.player.defaultVigourNumber)
        {
            Countdown.SetActive(false);
            if (timeDiff <= TimeSpan.Zero)  // 修改这里，处理所有小于等于0的情况
            {
                PlayerInfo.Instance.AddVigourNumber(1, () =>
                {
                    UpDataVigourNumber();
                });
            }
            return;
        }

        try
        {
            // 计算剩余时间
            var totalSeconds = (int)timeDiff.TotalSeconds;
            var hours = totalSeconds / 3600;
            var minutes = (totalSeconds % 3600) / 60;
            var seconds = totalSeconds % 60;

            // 更新倒计时文本
            var countdownText = Countdown.transform.Find("Text (Legacy)")?.GetComponent<Text>();
            if (countdownText != null)
            {
                countdownText.text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
                Countdown.SetActive(true);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"更新倒计时时发生错误: {e.Message}");
            Countdown.SetActive(false);
        }
    }
}
