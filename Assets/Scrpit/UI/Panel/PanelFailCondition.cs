using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TTSDK;
using System.Collections.Generic;
public class PanelFailCondition : MonoBehaviour
{
    public Text ProgressNum;
    public Text CoinNum;
    public GameObject ProgressBar;

    void OnEnable()
    {
        float proportion = ProgressBar.GetComponent<RoundProportionUICondition>().RoundProportion;
        ProgressNum.text = "已完成" + proportion + "%";
        ToFadeInChildren(gameObject, 0.2f); // 直接调用静态方法
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
    void Update()
    {
        CoinNum.text = '*' + (GameDataManager.Instance.goldenCoin + 100).ToString();
    }
    public void ToFadeInChildren(
        GameObject parentObject,
        float duration,
        float waitTime = 0.1f,
        Action callback = null
    )
    {
#if UNITY_WEBGL && !UNITY_EDITOR

        TT.ReportAnalytics("UserDie", new Dictionary<string, object>
                {
                    { "roundId", DataManager.Instance.gameInfo.roundInfo.currentLevel },
                    { "deviceId", SystemInfo.deviceUniqueIdentifier }
                });
#endif
        // 启动协程
        StartCoroutine(
            FadeInChildrenCoroutine(parentObject.transform, duration, waitTime, callback)
        );
    }

    private IEnumerator FadeInChildrenCoroutine(
        Transform parentTransform,
        float duration,
        float waitTime,
        Action callback
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

            // 等待一段时间再淡入下一个子物体
            yield return new WaitForSeconds(waitTime); // 可以根据需要调整间隔时间
        }
        callback?.Invoke();
    }
    public void OnGiveUp()
    {
        Debug.Log("放弃");
        GameManager manager = new GameManager();
        manager.OnResetGame(); // 调用重置游戏事件
    }
    //复活
    public void OnRevive()
    {
        //多倍奖励
        ADManager.Instance.OnADShow(() =>
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            TT.ReportAnalytics("UserRevive", new Dictionary<string, object>
            {
                { "roundId", DataManager.Instance.gameInfo.roundInfo.currentLevel },
                { "deviceId", SystemInfo.deviceUniqueIdentifier }
            });
#endif
            Debug.Log("复活成功");

            if (GameDataManager.Instance == null)
            {
                Debug.LogError("GameDataManager.Instance 为 null");
                return;
            }

            if (GameDataManager.Instance.goldenCoin > 0)
            {
                GameDataManager.Instance.goldenCoin += 100;
            }

            Debug.Log("复活成功1");
            Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
            Debug.Log("复活成功2");

            if (player == null)
            {
                Debug.LogError("没有找到玩家");
                return;
            }

            Debug.Log("复活成功3");
            var playerComponent = player.GetComponent<player>();
            if (playerComponent == null)
            {
                Debug.LogError("玩家对象没有 player 组件");
                return;
            }

            GameDataManager.Instance.ChangePlayerLives(playerComponent.defaultBlood);
            Debug.Log("复活成功4");
            // 检查 gameObject 是否有效
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("gameObject 为 null");
            }
        });
    }


}
