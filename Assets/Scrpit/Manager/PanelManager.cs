using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    private List<panelItem> PanelsList = new List<panelItem>();
    public enum PanelName
    {
        PanelSuccess,
        PanelFail,
        PanelHurt,
        PanelWarn,
        PanelRound,
        PanelSetting,
        PanelPop,
        PanelTips
    }

    private List<PanelName> panelsName = new List<PanelName> {
    PanelName.PanelSuccess,
    PanelName.PanelFail,
    PanelName.PanelHurt,
    PanelName.PanelWarn,
    PanelName.PanelRound,
    PanelName.PanelSetting,
    PanelName.PanelPop,
    PanelName.PanelTips
};
    // Start is called before the first frame update
    void Start()
    {
        initPanels();

    }
    void initPanels()
    {
        Transform canvasTransform = gameObject.transform.Find("Canvas");
        if (canvasTransform == null)
        {
            // Debug.LogError("找不到 Canvas 物体！请检查场景结构");
            return;
        }

        panelsName.ForEach(name =>
        {
            Transform panelTransform = canvasTransform.Find(name.ToString());
            if (panelTransform != null)
            {
                PanelsList.Add(new panelItem(name, panelTransform.gameObject));
            }
            else
            {
                // Debug.LogError($"找不到面板: {name}，请检查物体命名和层级");
            }
        });

        HideAllPanels();
    }

    // 显示指定的面板
    public void ShowPanel(PanelName name)
    {

        HideAllPanels();

        panelItem panelToShow = PanelsList.Find(panel => panel.name == name);
        if (panelToShow != null)
        {
            panelToShow.panel.SetActive(true);
            Debug.Log("面板已显示: " + name);
        }
        else
        {
            Debug.LogWarning("面板未在队列中: " + name);
        }
    }

    // 隐藏指定的面板
    public void HidePanel(PanelName name) // 保持参数为 name
    {

        panelItem panelToHide = PanelsList.Find(panel => panel.name == name); // 根据 name 查找 panelItem
        if (panelToHide != null) // 检查是否在 PanelsList 中
        {
            panelToHide.panel.SetActive(false); // 隐藏面板
        }
        else
        {
            Debug.LogWarning("面板未在队列中: " + name); // 使用 name 输出警告
        }
    }

    // 隐藏所有面板
    public void HideAllPanels()
    {
        foreach (panelItem panel in PanelsList) // 从 PanelsList 中遍历
        {
            panel.panel.SetActive(false); // 隐藏面板
        }
    }
    public void showHome()
    {
        HideAllPanels();
        Transform PanelTransform = gameObject.transform.Find("Canvas");
        if (PanelTransform == null) return;

        if (PanelTransform.Find("PanelGameCondition") != null)
        {
            PanelTransform.Find("PanelGameCondition").gameObject.SetActive(false);
        }
        // 查找并激活 HomePanel

        if (PanelTransform.Find("HomePanel") != null)
        {
            PanelTransform.Find("HomePanel").gameObject.SetActive(true);
        }
        GameDataManager.Instance.isPause = true;
    }
    public void showFight()
    {
        HideAllPanels();
        Transform panelTransform = gameObject.transform.Find("Canvas");
        if (panelTransform == null) return;

        // 隐藏 HomePanel
        if (panelTransform.Find("HomePanel") != null)
        {
            panelTransform.Find("HomePanel").gameObject.SetActive(false);
        }

        // 查找并激活 FightPanel
        if (panelTransform.Find("PanelGameCondition") != null)
        {
            panelTransform.Find("PanelGameCondition").gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("PanelGameCondition not found");
        }

        GameDataManager.Instance.isPause = false;
    }
    public void ShowSkin()
    {
        HideAllPanels();
        Transform panelTransform = gameObject.transform.Find("Canvas");
        Transform ThreeDimension = gameObject.transform.Find("Canvas3D");
        if (panelTransform == null || ThreeDimension == null) return;

        // 隐藏 HomePanel
        if (panelTransform.Find("HomePanel") != null)
        {
            panelTransform.Find("HomePanel").gameObject.SetActive(false);
        }
        // 隐藏 FightPanel
        if (panelTransform.Find("PanelGameCondition") != null)
        {
            panelTransform.Find("PanelGameCondition").gameObject.SetActive(false);
        }
        if (ThreeDimension.Find("PanelSkin") != null)
        {
            ThreeDimension.Find("PanelSkin").gameObject.SetActive(true);
        }

    }
    public void ShowBodyArt()
    {
        HideAllPanels();
        Transform panelTransform = gameObject.transform.Find("Canvas");
        Transform ThreeDimension = gameObject.transform.Find("Canvas3D");
        if (panelTransform == null || ThreeDimension == null) return;

        // 隐藏 HomePanel
        if (panelTransform.Find("HomePanel") != null && panelTransform.Find("HomePanel").gameObject.activeSelf)
        {
            panelTransform.Find("HomePanel").gameObject.SetActive(false);
        }
        // 隐藏 FightPanel
        if (panelTransform.Find("PanelGameCondition") != null && panelTransform.Find("PanelGameCondition").gameObject.activeSelf)
        {
            panelTransform.Find("PanelGameCondition").gameObject.SetActive(false);
        }
        if (ThreeDimension.Find("PanelBodyArt") != null)
        {
            ThreeDimension.Find("PanelBodyArt").gameObject.SetActive(true);
        }

    }
    public void ShowPop(string title, string content, Action confirmAction, Action cancelAction)
    {
        panelItem panelToShow = PanelsList.Find(panel => panel.name == PanelName.PanelPop);
        if (panelToShow != null)
        {
            Debug.Log("AAAAShowPop" + confirmAction);
            panelToShow.panel.SetActive(true);
            panelToShow.panel.GetComponent<PanelPopCondition>().PopShow(title, content, confirmAction, cancelAction);
        }

    }
    public void ShowTips(string content)
    {
        panelItem panelToShow = PanelsList.Find(panel => panel.name == PanelName.PanelTips);
        if (panelToShow != null)
        {
            panelToShow.panel.SetActive(true);
            panelToShow.panel.GetComponent<PanelTipsCondition>().TipsShow(content);
        }

    }
}
public class panelItem
{
    public PanelManager.PanelName name;
    public GameObject panel;

    public panelItem(PanelManager.PanelName name, GameObject panel)
    {
        this.name = name;
        this.panel = panel;
    }
}