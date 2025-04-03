using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    // 公共面板队列，允许在编辑器中自由增加和绑定面板
    public List<CanvasGroup> panels = new List<CanvasGroup>();

    // Start is called before the first frame update
    void Start()
    {
        // 初始化面板状态（可选）
        HideAllPanels();
    }

    // 显示指定的面板
    public void ShowPanel(CanvasGroup panelToShow)
    {
        if (panelToShow == null) return; // 如果传入的面板为空，则直接返回
        HideAllPanels(); // 隐藏所有面板

        // 显示指定的面板
        if (panels.Contains(panelToShow))
        {
            panelToShow.alpha = 1f; // 设置透明度为 1
            panelToShow.interactable = true; // 启用交互
            panelToShow.blocksRaycasts = true; // 启用射线检测
        }
        else
        {
            Debug.LogWarning("面板未在队列中: " + panelToShow.name);
        }
    }

    // 隐藏指定的面板
    public void HidePanel(CanvasGroup panelToHide)
    {
        if (panels.Contains(panelToHide) && panelToHide)
        {
            panelToHide.alpha = 0f; // 设置透明度为 0
            panelToHide.interactable = false; // 禁用交互
            panelToHide.blocksRaycasts = false; // 禁用射线检测
        }
        else
        {
            Debug.LogWarning("面板未在队列中: "+panelToHide);
        }
    }

    // 隐藏所有面板
    public void HideAllPanels()
    {
        foreach (CanvasGroup panel in panels)
        {
            HidePanel(panel);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
