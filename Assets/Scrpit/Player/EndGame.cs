using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public List<string> ignoredTags = new List<string> { "Player" };
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("结束：" + collision.gameObject.tag);
        // 输出碰撞对象的名称
        if (ignoredTags.Contains(collision.gameObject.tag))
        {
            Debug.Log("碰撞对象被忽略: ");
            endGame();
            
        }

    }
    public void endGame()
    {
        // 游戏胜利逻辑
        Debug.Log("游戏胜利！");
        // Time.timeScale = 0;
        PanelManager panelManager = FindObjectOfType<PanelManager>(); // 获取 PanelManager 组件
        if (panelManager != null)
        {
            if (GameDataManager.Instance.playerLives > 0)
            {
                panelManager.ShowPanel(PanelManager.PanelName.PanelSuccess); // 显示 PanelSuccess 面板
            }
            else
            {
                panelManager.ShowPanel(PanelManager.PanelName.PanelFail); // 显示 PanelFail 面板
            }
        }
    }
}
