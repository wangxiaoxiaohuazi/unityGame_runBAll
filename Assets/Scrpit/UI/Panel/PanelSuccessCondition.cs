using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelSuccessCondition : MonoBehaviour
{
    public GameObject Ribbon;


    void OnEnable()
    {
        ToFadeInChildren(gameObject, 0.2f); // 直接调用静态方法
        GameDataManager.Instance.isPause = true;
        AudioManager.Instance.PlaySFX("success");
        //随机展示插屏广告
        if (RoundInfo.Instance.OnGetCurrentLevel().id > 3)
        {
            ADManager.Instance.OnIInterstitialADShow();
        }
    }
    void OnDisable()
    {
        GameDataManager.Instance.isPause = false;
    }

    void OnDestroy()
    {
        GameDataManager.Instance.isPause = false;
    }

    public void ToFadeInChildren(
        GameObject parentObject,
        float duration,
        float waitTime = 0.1f,
        Action callback = null
    )
    {
        // 启动协程
        StartCoroutine(
            FadeInChildrenCoroutine(parentObject.transform, duration, waitTime, callback)
        );
    }

    private IEnumerator FadeInChildrenCoroutine(
        Transform parentTransform,
        float duration,
        float waitTime,
        Action callback = null
    )
    {
        // 获取所有子物体
        foreach (Transform child in parentTransform)
        {
            CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                // 如果子物体没有 CanvasGroup 组件，则添加一个
                canvasGroup = child.gameObject.AddComponent<CanvasGroup>();
            }

            // 设置初始透明度为 0
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false; // 禁用交互
        }

        // 淡入动画
        foreach (Transform child in parentTransform)
        {
            CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();

            // 淡入动画
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null; // 等待下一帧
            }

            // 确保最终透明度为 1
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true; // 启用交互
            ReadData(child);
            // 等待一段时间再淡入下一个子物体
            yield return new WaitForSeconds(waitTime); // 可以根据需要调整间隔时间
        }
        foreach (Transform child in parentTransform)
        {
            CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                // 如果子物体没有 CanvasGroup 组件，则添加一个
                canvasGroup = child.gameObject.AddComponent<CanvasGroup>();
            }

            // 设置初始透明度为 0
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true; // 禁用交互
        }
        callback?.Invoke();
    }
    void ReadData(Transform child)
    {
        switch (child.name)
        {
            case "Reward":
                child.Find("Num").GetComponent<Text>().text = (GameDataManager.Instance.goldenCoin * 3).ToString();
                break;
            case "Score":
                Transform[] childTransforms = child.GetComponentsInChildren<Transform>();
                for (int index = 0; index < childTransforms.Length; index++)
                {
                    GameObject item = childTransforms[index].gameObject; // 获取 GameObject
                    if (index >= GameDataManager.Instance.playerLives)
                    {
                        item.SetActive(true); // 根据需要处理 item
                    }
                }
                break;
            default:
                break;
        }
    }
    public void NextScene()
    {
        // 正确调用方式：通过单例实例调用
        RoundInfo.Instance.OnNextRound(() =>
        {
            PublicGameData gameData = DataManager.Instance.gameInfo;
            string scenePath = gameData
                .roundInfo.levelSceneList.FirstOrDefault(x =>
                    x.id - 1 == RoundInfo.Instance.OnGetCurrentLevel().id
                ) // 使用FirstOrDefault
                ?.scenePath; // 使用空条件运算符以防止空引用异常
            if (scenePath != null)
            {
                AddressablesLoaderManager.Instance.SwitchScene(scenePath);
            }
            else
            {
                AddressablesLoaderManager.Instance.ReloadCurrentScene(
                    progress =>
                    {
                        Debug.Log($"重载进度: {progress:P0}");
                        // 这里可以更新UI进度条
                    });
            }
        });
    }

    public void DoubleReward()
    {
        //多倍奖励
        ADManager.Instance.OnADShow(() =>
        {
            GameDataManager.Instance.goldenCoin = GameDataManager.Instance.goldenCoin * 3;
            NextScene();
        });
    }
}
