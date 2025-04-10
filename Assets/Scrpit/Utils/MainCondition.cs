using System;
using System.Collections.Generic;
using TMPro;
using TTSDK;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

public class MainCondition : MonoBehaviour
{
    public GameObject scrollviewNode = null;
    public GameObject sideView = null;
    public DataManager.SceneInfo[] scenesList;

    public Text coinNum = null;
    public Text VigourNumber = null;
    public TextMeshProUGUI levelText = null;
    private PublicGameData _gameData;

    // Start is called before the first frame update
    void Start()
    {
        _gameData = DataManager.Instance.gameInfo;
        coinNum.text = _gameData.player.coin.ToString();
        VigourNumber.text =
            PlayerInfo.Instance.GetVigourNumber() + "/" + _gameData.player.defaultVigourNumber;
        InitScrollviewNode();
        levelText.text = "第" + DataManager.Instance.gameInfo.roundInfo.currentLevel + "关";
    }

    // Update is called once per frame
    void Update()
    {
        InitSideNode();
    }

    private void InitScrollviewNode()
    {
        //获取Scenes文件下的所有场景文件 将其添加到scrollview中
        scenesList = DataManager.Instance.scenesList;
        Debug.Log(scenesList[0]);
        //scrollviewNode中展示的节点为选中的场景
    }

    private void InitSideNode()
    {
        // Debug.Log("体力值" + _gameData.player.DefaultvigourNumber);
        if (_gameData.player.defaultVigourNumber > 10)
        {
            sideView.transform.Find("NavigateSideButton").gameObject.SetActive(false);
            sideView.transform.Find("GetReward").gameObject.SetActive(false);
        }
        else if (_gameData.player.defaultVigourNumber == 10)
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
    }

    public void OnStartGameClick(string sceneName)
    {
        if (PlayerInfo.Instance.GetVigourNumber() < 2)
        {
            //体力不足 TODO 弹窗
            return;
        }
        //跳转到对应场景
        string path;
        Debug.Log("当前关卡" + PlayerInfo.Instance.GetVigourNumber());
        path = _gameData.roundInfo.levelSceneList[_gameData.roundInfo.currentLevel - 1].scenePath; //跳转到当前关卡场景
        PlayerInfo.Instance.AddVigourNumber(-3); // }
        UnityEngine.SceneManagement.SceneManager.LoadScene(path);
    }

    public void OnSideViewVisible()
    {
        sideView.SetActive(!sideView.activeSelf);
    }

    public void OnGetRwardClick()
    {
        _gameData.player.defaultVigourNumber += 3;
        DataManager.Instance.SaveData();
    }

    public void OnAddVigourNumber()
    {
        PlayerInfo.Instance.AddVigourNumber(10);
    }
}
