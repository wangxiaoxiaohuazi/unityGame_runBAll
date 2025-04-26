using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelRoundCondition : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform content;
    private List<LevelList> scenesList;
    private PanelManager panelManager;
    private int currentLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        panelManager = FindObjectOfType<PanelManager>();
        itemPrefab.SetActive(false);
        scenesList = new List<LevelList>(DataManager.Instance.gameInfo.roundInfo.levelSceneList);
        Debug.Log("场景列表" + scenesList.Count);
        currentLevel = DataManager.Instance.gameInfo.roundInfo.currentLevel;
        initNode();
        GameDataManager.Instance.isPause = true;
    }
    void OnEnable()
    {
        if (DataManager.Instance == null || DataManager.Instance.gameInfo?.roundInfo == null)
        {
            Debug.LogError("DataManager或roundInfo未初始化!");
            return;
        }

        if (scenesList == null)
        {
            scenesList = new List<LevelList>(DataManager.Instance.gameInfo.roundInfo.levelSceneList);
        }

        if (content == null)
        {
            Debug.LogError("content Transform未赋值!");
            return;
        }

        currentLevel = DataManager.Instance.gameInfo.roundInfo.currentLevel;
        initNode();
        GameDataManager.Instance.isPause = true;
    }
    void OnDisable()
    {
        GameDataManager.Instance.isPause = false;
    }

    void OnDestroy()
    {
        GameDataManager.Instance.isPause = false;
    }

    void initNode()
    {
        if (content == null || scenesList == null)
        {
            Debug.LogError("必要组件未初始化!");
            return;
        }

        // 清空之前的按钮
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        //填充页面
        scenesList.ForEach(
            (LevelList sceneInfo) =>
            {
                // 计算当前关卡与场景信息的差值
                int flag = currentLevel - sceneInfo.id;
                // 实例化新的关卡项
                GameObject item = Instantiate(itemPrefab, content);
                item.SetActive(true);

                // 获取子物体的引用
                GameObject levelPass = item.transform.Find("LevelPass").gameObject;
                GameObject levelUnlock = item.transform.Find("Levelunlock").gameObject;
                GameObject levelNow = item.transform.Find("LevelNow").gameObject;
                // 根据flag的值设置子物体的可见性
                levelPass.SetActive(flag > 0);
                levelUnlock.SetActive(flag < 0);
                levelNow.SetActive(flag == 0);

                if (flag == 0)
                {
                    levelNow.transform.Find("LevelName").GetComponent<Text>().text =
                        sceneInfo.id.ToString();
                    Button levelNowButton = levelNow.GetComponent<Button>();
                    levelNowButton.interactable = true;
                    levelNowButton.onClick.AddListener(() =>
                    {
                        Debug.Log("按钮点击" + sceneInfo.scenePath);
                        SceneChange(sceneInfo.scenePath);
                    });
                }
                if (flag > 0)
                {
                    levelPass.transform.Find("Image").Find("LevelName").GetComponent<Text>().text =
                        sceneInfo.id.ToString();
                    Button levelPassButton = levelPass.GetComponent<Button>();
                    levelPassButton.interactable = true;
                    levelPassButton.onClick.AddListener(() => SceneChange(sceneInfo.scenePath));
                    if (sceneInfo.score != 0)
                    {
                        for (int i = 0; i < sceneInfo.score; i++)
                        {
                            levelPass
                                .transform.Find("StartImages")
                                .GetComponentsInChildren<Image>()[i]
                                .gameObject.SetActive(false);
                        }
                    }
                }
            }
        );
    }

    public void SceneChange(string sceneName)
    {
        if (sceneName == null || sceneName == "")
            return;
        Debug.Log("切换场景" + sceneName == null);

        // 跳转到对应的场景
        //         AddressablesLoaderManager.Instance.SwitchScene(
        //     sceneName
        //  );
        SceneManager.LoadScene(sceneName);
    }

    public void OnClickBack()
    {
        panelManager.showHome();
    }
}
