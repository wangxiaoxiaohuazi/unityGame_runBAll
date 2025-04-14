using System;
using System.Collections;
using UnityEngine;

public class FadeInChildren : MonoBehaviour
{
    // 将方法改为静态方法
    public static void ToFadeInChildren(GameObject parentObject, float duration, float waitTime = 0.5f, Action callback = null)
    {
        // 启动协程
        parentObject.GetComponent<FadeInChildren>().StartCoroutine(FadeInChildrenCoroutine(parentObject.transform, duration, waitTime, callback));
    }

    private static IEnumerator FadeInChildrenCoroutine(Transform parentTransform, float duration, float waitTime = 0.5f, Action callback = null)
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
}