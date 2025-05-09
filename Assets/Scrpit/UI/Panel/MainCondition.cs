
using TMPro;
using TTSDK;
using UnityEngine;
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

    // Start is called before the first frame update
    void Start()
    {
        _gameData = DataManager.Instance.gameInfo;
        InitScrollviewNode();
        if (RoundInfo.Instance.OnGetCurrentLevel() != null)
        {
            levelText.text = "第" + RoundInfo.Instance.OnGetCurrentLevel().id + "关";
        }
        ADManager.Instance.ShowBannerAd();
         UpDataVigourNumber();
    }
    void OnEnable()
    {
        ADManager.Instance.ShowBannerAd();
        //更新体力
        DataManager.Instance.gameInfo.player.todayVigour.CalculateAutoRecovery();
        UpDataVigourNumber();

    }
    // Update is called once per frame
    void Update()
    {
        InitSideNode();
        coinNum.text = _gameData.player.coin.ToString();


    }
    public void UpDataVigourNumber()
    {
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
        // Debug.Log("场景值" + launchOption.Scene);
        if (launchOption?.Scene == "021001")
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
        //获取300金币
        PlayerInfo.Instance.SaveCoin(300, () =>
        {
            coinNum.text = _gameData.player.coin.ToString();
        });
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
                PlayerInfo.Instance.AddVigourNumber(8);
                manager.HidePanel(PanelManager.PanelName.PanelPop);
                UpDataVigourNumber();
            });

        }, null);
    }
    public void OnShowTTSide()
    {
        TTFunction ttFunction = new TTFunction();
        ttFunction.NavigateToTTSidebar();
    }
}
