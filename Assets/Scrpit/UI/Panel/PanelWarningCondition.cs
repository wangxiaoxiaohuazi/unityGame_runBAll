using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelWarningCondition : MonoBehaviour
{
    public float warningTime = 2f;
    private Image background;
    private GameObject BorderTop;
    private GameObject BorderBottom;
    private GameObject CenterNode;
    private float scrollSpeed = 300f; // 滚动速度
    public float resetPositionX = -600f; // 重置位置的X坐标

    // Start is called before the first frame update
    void Start()
    {
      
        // 获取子物体Background的Image组件
        background = transform.Find("Background").GetComponent<Image>();
        BorderTop = transform.Find("BorderTop").gameObject;
        BorderBottom = transform.Find("BorderBottom").gameObject;
        CenterNode = transform.Find("Center").gameObject;
        StartCoroutine(BlinkBackground());
        StartCoroutine(ScrollBorders());
        StartCoroutine(FadeInCenterNode());
    }

    private IEnumerator BlinkBackground()
    {
        while (true) // 无限循环，您可以根据需要添加条件来停止
        {
            // 逐渐变为透明
            for (float alpha = 1f; alpha >= 0f; alpha -= 0.1f)
            {
                Color color = background.color;
                color.a = alpha; // 设置透明度
                background.color = color;
                yield return new WaitForSeconds(0.02f); // 每次减少透明度后等待
            }

            // 逐渐变为不透明
            for (float alpha = 0f; alpha <= 1f; alpha += 0.1f)
            {
                Color color = background.color;
                color.a = alpha; // 设置透明度
                background.color = color;
                yield return new WaitForSeconds(0.02f); // 每次增加透明度后等待
            }

            yield return new WaitForSeconds(0.2f); // 在每次闪烁之间等待0.2秒
        }
    }

    private IEnumerator ScrollBorders()
    {
        while (true)
        {
            // BorderTop 向左滚动
            BorderTop.transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);
            // BorderBottom 向右滚动
            BorderBottom.transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime);

            // 检查 BorderTop 是否需要重置位置
            if (BorderTop.transform.localPosition.x < resetPositionX)
            {
                BorderTop.transform.localPosition = new Vector3(Screen.width, BorderTop.transform.localPosition.y, BorderTop.transform.localPosition.z);
            }

            // 检查 BorderBottom 是否需要重置位置
            if (BorderBottom.transform.localPosition.x > Screen.width)
            {
                BorderBottom.transform.localPosition = new Vector3(resetPositionX, BorderBottom.transform.localPosition.y, BorderBottom.transform.localPosition.z);
            }

            yield return null; // 等待下一帧
        }
    }
    private IEnumerator FadeInCenterNode()
    {
        // 获取CenterNode的CanvasGroup组件
        CanvasGroup canvasGroup = CenterNode.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            // 如果没有CanvasGroup组件，添加一个
            canvasGroup = CenterNode.AddComponent<CanvasGroup>();
        }
        // 设置初始透明度为0
        canvasGroup.alpha = 0f;
        // 计算每次增加的透明度
        float fadeDuration = 0.5f; // 总淡入时间
        float fadeStep = 0.1f; // 每次增加的透明度
        float waitTime = fadeDuration / (1f / fadeStep); // 计算每次等待的时间

        // 淡入效果
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += fadeStep; // 增加透明度
            yield return new WaitForSeconds(waitTime); // 每次增加透明度后等待
        }
        // 确保最终透明度为1
        canvasGroup.alpha = 1f;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
