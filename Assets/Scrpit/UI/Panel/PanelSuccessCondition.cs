
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelSuccessCondition : MonoBehaviour
{
    public GameObject Ribbon;
    // Start is called before the first frame update
    void Start()
    {

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
    public void NextScene()
    {
        if (PlayerInfo.Instance.GetVigourNumber() < 2)
        {
            Debug.Log("体力不足");
            return;
        }
        Debug.Log("下一关");
        // 场景切换
        SceneManager.LoadScene("GameScene");
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
                Debug.Log("场景路径为" + scenePath);
                SceneManager.LoadScene(scenePath);
            }
            else
            {
                Debug.Log("场景路径为空");
            }
        });
    }
    public void DoubleReward()
    {


        //多倍奖励
        ADManager.Instance.OnADShow(() =>
        {
            GameDataManager.Instance.goldenCoin += GameDataManager.Instance.goldenCoin * 3;
            NextScene();
        });
    }
   
}
