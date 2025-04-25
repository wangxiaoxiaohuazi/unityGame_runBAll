using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTipsCondition : MonoBehaviour
{
    public Text ContentText;
    public Transform ContentTrans;
    private Coroutine autoCloseCoroutine;
    private Coroutine moveCoroutine;
    private Vector3 originalPosition; // 保存原始位置

    void Awake()
    {
        // 在Awake中保存原始位置
        originalPosition = ContentTrans.localPosition;
    }

    public void TipsShow(string content)
    {
        if (content != null)
        {
            ContentText.text = content;
        }

        // 如果之前有正在运行的协程，先停止它们
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
        }
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // 重置到原始位置
        ContentTrans.localPosition = originalPosition;

        // 启动移动动画协程
        moveCoroutine = StartCoroutine(MoveUpAnimation());

        // 启动新的自动关闭协程
        autoCloseCoroutine = StartCoroutine(AutoClosePanel());
    }

    private IEnumerator MoveUpAnimation()
    {
        float duration = 0.5f; // 动画持续时间
        float elapsedTime = 0f;
        Vector3 startPos = originalPosition;
        Vector3 targetPos = startPos + new Vector3(0, 100f, 0); // 在原始位置基础上向上移动100单位

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // 使用平滑插值
            progress = Mathf.SmoothStep(0, 1, progress);

            ContentTrans.localPosition = Vector3.Lerp(startPos, targetPos, progress);
            yield return null;
        }

        ContentTrans.localPosition = targetPos;
    }

    private IEnumerator AutoClosePanel()
    {
        yield return new WaitForSeconds(1.5f);
        PanelManager panelManager = FindObjectOfType<PanelManager>();
        panelManager.HidePanel(PanelManager.PanelName.PanelTips); // 隐藏提示面板

    }
}
