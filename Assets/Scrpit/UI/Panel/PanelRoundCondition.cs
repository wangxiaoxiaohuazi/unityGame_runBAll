using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelRoundCondition : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform content;
    public Button btnLeft;
    public Button btnRight;
    public Text txtPage;
    private List<LevelList> scenesList;
    private PanelManager panelManager;
    private int currentLevel = 1;
    private int currentPage = 0; // 当前页码
    private const int itemsPerPage = 24; // 每页显示的关卡数量
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
        // 获取选中的关卡ID
        int pickLevel = DataManager.Instance.gameInfo.roundInfo.pickLevel;

        // 根据pickLevel计算当前页码 
        currentPage = pickLevel / itemsPerPage; // 计算当前页码
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
        // 计算当前页的关卡索引范围
        int startIndex = currentPage * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, scenesList.Count);
        int totalPages = Mathf.CeilToInt((float)scenesList.Count / itemsPerPage); // 计算总页数
        txtPage.text = $"{currentPage + 1}/{totalPages}"; // 更新文本为 当前页数/总页数
        //填充页面
        for (int i = startIndex; i < endIndex; i++)
        {
            LevelList sceneInfo = scenesList[i];
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
                    SceneChange(sceneInfo);
                });
            }
            if (flag > 0)
            {
                levelPass.transform.Find("Image").Find("LevelName").GetComponent<Text>().text =
                    sceneInfo.id.ToString();
                Button levelPassButton = levelPass.GetComponent<Button>();
                levelPassButton.interactable = true;
                levelPassButton.onClick.AddListener(() => SceneChange(sceneInfo));
                if (sceneInfo.score != 0)
                {
                    for (int n = 0; n < sceneInfo.score; n++)
                    {
                        levelPass
                            .transform.Find("StartImages")
                            .GetComponentsInChildren<Image>()[n]
                            .gameObject.SetActive(false);
                    }
                }
            }
        }
        ;
        // 更新按钮状态
        UpdatePageButtons();
    }
    void UpdatePageButtons()
    {
        // 假设你有两个按钮，上一页和下一页
        Button previousButton = btnLeft; // 获取上一页按钮的引用
        Button nextButton = btnRight; // 获取下一页按钮的引用

        previousButton.interactable = currentPage > 0; // 如果不是第一页，允许点击
        nextButton.interactable = (currentPage + 1) * itemsPerPage < scenesList.Count; // 如果不是最后一页，允许点击
    }

    public void OnClickPreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            initNode(); // 重新初始化节点以更新显示
        }
    }

    public void OnClickNextPage()
    {
        if ((currentPage + 1) * itemsPerPage < scenesList.Count)
        {
            currentPage++;
            initNode(); // 重新初始化节点以更新显示
        }
    }
    public void SceneChange(LevelList scenceList)
    {
        if (scenceList == null)
            return;

        DataManager.Instance.gameInfo.roundInfo.pickLevel = scenceList.id;
        Debug.Log("当前关卡:" + DataManager.Instance.gameInfo.roundInfo.pickLevel);
        // 跳转到对应的场景
        //         AddressablesLoaderManager.Instance.SwitchScene(
        //     sceneName
        //  );
        // SceneManager.LoadScene(scenceList.scenePath);
        GameManager manager = new GameManager();
        manager.OnResetGame(); // 调用重置游戏事件
    }

    public void OnClickBack()
    {
        panelManager.showHome();
    }
}
