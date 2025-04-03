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
        Debug.Log("结束："+collision.gameObject.tag);
        // 输出碰撞对象的名称
        if (ignoredTags.Contains(collision.gameObject.tag))
        {
            Debug.Log("碰撞对象被忽略: ");
            endGame();
            // 你可以在这里添加胜利场景的加载或其他逻辑
            // 例如，重新加载当前场景或加载胜利场景
            // SceneManager.LoadScene("VictoryScene"); // 替换为你的胜利场景名称
        }

    }
    public void endGame()
    {
        // 游戏胜利逻辑
        Debug.Log("游戏胜利！");
        Time.timeScale = 0;
        PanelManager panelManager = FindObjectOfType<PanelManager>(); // 获取 PanelManager 组件

        if (panelManager != null)
        {
            panelManager.ShowPanel(panelManager.panels[0]); // 显示 PanelSuccess 面板
        }
    }
}
