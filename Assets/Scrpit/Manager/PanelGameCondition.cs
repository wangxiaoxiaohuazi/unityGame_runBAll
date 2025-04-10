using System.Collections.Generic;
using System.Linq; // 添加此行以引入LINQ命名空间
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 如果使用的是 UI.Text

// using TMPro; // 如果使用的是 TextMeshPro

public class PanelGameCondition : MonoBehaviour
{
    public GameObject bloodBar; // 血条
    public Sprite heartSprite; // 血量图标
    public Sprite DisheartSprite; // 血量图标
    public Text FrowardSpeedText; // 如果使用的是 UI.Text
    public Text HorizontalSpeedText; // 如果使用的是 UI.Text
    public Text FpsText; // 如果使用的是 UI.Text
    private float refreshRate = 0.5f; // 刷新间隔
    private float timer = 0f; // 计时器
    private float fps; // 存储当前帧率
    private GameObject sphere; // 在 Inspector 中拖放 Sphere 对象
    public GameObject roundView; // 如果使用的是 UI.Text
    public Text ScoreText; // 如果使用的是 UI.Text
    public Text RoundNameText; // 如果使用的是 UI.Text
    public Text GoldenText; // 如果使用的是 UI.Text

    void Start()
    {
        // 初始化显示玩家生命值
        UpdatePlayerLives(GameDataManager.Instance.playerLives);
        // 添加空引用检查
        if (sphere == null)
        {
            // 尝试自动查找玩家
            sphere = GameObject.FindGameObjectWithTag("Player");

            if (sphere == null)
            {
                Debug.LogError("Camera Follow: Could not find player with 'Player' tag!");
                enabled = false; // 禁用脚本
                return;
            }
        }
        SphereController sphereMove = sphere.GetComponent<SphereController>();
        //如果不是活动场景则隐藏
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            gameObject.SetActive(false);
        }
        // 加载保存的值
        // if (PlayerPrefs.HasKey("ForwardSpeed"))
        // {
        //     sphereMove.forwardSpeed = PlayerPrefs.GetFloat("ForwardSpeed");
        // }

        // if (PlayerPrefs.HasKey("HorizontalSpeed"))
        // {
        //     sphereMove.sideSpeed = PlayerPrefs.GetFloat("HorizontalSpeed");
        // }
        //设置当前关卡
        RoundNameText.text = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        // 你可以在这里定期更新生命值（如果需要）
        // 例如，监听生命值变化的事件
        SphereController sphereMove = sphere.GetComponent<SphereController>();
        player _player = sphere.GetComponent<player>();
        HorizontalSpeedText.text = "水平速度：" + sphereMove.sideSpeed.ToString();
        // FrowardSpeedText.text = "前进速度：" + sphereMove.forwardSpeed.ToString();
        FrowardSpeedText.text =
            "皮肤数量：" + DataManager.Instance.gameInfo.collections.skins.Count;
        ScoreText.text = "得分：" + _player.scoreNumber.ToString();
        GoldenText.text = "金币：" + GameDataManager.Instance.goldenCoin.ToString();
        // 更新计时器
        timer += Time.deltaTime;
        // 每 0.5 秒更新一次帧率
        if (timer >= refreshRate)
        {
            fps = 1.0f / Time.deltaTime; // 计算帧率
            FpsText.text = "帧率: " + fps.ToString("F2"); // 更新文本
            timer = 0f; // 重置计时器
        }
    }

    public void UpdatePlayerLives(float lives)
    {
        // 获取当前血条中的心形数量
        int currentHearts = bloodBar.transform.childCount;

        // 当生命值增加时
        if (lives > currentHearts)
        {
            for (int i = currentHearts; i < lives; i++)
            {
                // 创建新的心形图标
                GameObject heart = new GameObject("Heart_" + (i + 1));
                Image heartImage = heart.AddComponent<Image>();
                heartImage.sprite = heartSprite;
                heartImage.preserveAspect = true;
                // 设置父对象并重置变换
                heart.transform.SetParent(bloodBar.transform);
                heart.transform.localScale = Vector3.one;
                heart.transform.localPosition = Vector3.zero;
            }
        }
        // 当生命值减少时
        else if (lives < currentHearts)
        {
            // 从最后一个开始销毁
            for (int i = currentHearts - 1; i >= lives; i--)
            {
                bloodBar.transform.GetChild(i).GetComponent<Image>().sprite = DisheartSprite;
            }
        }
    }

    public void UpdatePlayerPower(float lives)
    {
        // playerPowerBar.size = (float)lives / 10f;
        // playerPowerText.text = "能量值：" + (float)lives / 10f; ;
    }

    public void UpdateFrowardSpeed(float forwardSpeed)
    {
        SphereController sphereMove = sphere.GetComponent<SphereController>();
        sphereMove.forwardSpeed += forwardSpeed;

        // 保存到 PlayerPrefs
        PlayerPrefs.SetFloat("ForwardSpeed", sphereMove.forwardSpeed);
        PlayerPrefs.Save(); // 确保保存
    }

    public void UpdateHorizontalSpeed(float horizontalSpeed)
    {
        SphereController sphereMove = sphere.GetComponent<SphereController>();
        sphereMove.sideSpeed += horizontalSpeed; // 假设您有一个 horizontalSpeed 属性

        // 保存到 PlayerPrefs
        PlayerPrefs.SetFloat("HorizontalSpeed", sphereMove.sideSpeed);
        PlayerPrefs.Save(); // 确保保存
    }

    public void roundChangeVisible()
    {
        roundView.SetActive(!roundView.activeSelf);
    }

    public void ResetGame()
    {
        if (sphere)
        {
            sphere.GetComponent<player>().OnResetGame();
        }
    }

    public void NextScene()
    {
        if (PlayerInfo.Instance.GetVigourNumber() < 2)
        {
            Debug.Log("体力不足");
            return;
        }
        // 正确调用方式：通过单例实例调用
        RoundInfo.Instance.OnNextRound(() =>
        {
            PublicGameData gameData = DataManager.Instance.gameInfo;
            string scenePath = gameData
                .roundInfo.levelSceneList.FirstOrDefault(x =>
                    x.id - 1 == RoundInfo.Instance.OnGetCurrentLevel()
                ) // 使用FirstOrDefault
                ?.scenePath; // 使用空条件运算符以防止空引用异常
            if (scenePath != null)
            {
                SceneManager.LoadScene(scenePath);
            }
        });
    }
}
