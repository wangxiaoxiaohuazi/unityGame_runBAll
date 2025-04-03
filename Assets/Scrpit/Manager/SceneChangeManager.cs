using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections.Generic;

public class SceneChangeManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject buttonPrefab; // 按钮的Prefab
    public Transform scrollViewContent; // ScrollView的内容容器
    private List<DataManager.SceneInfo> scenesList;

    void Start()
    {
        InitSceneList();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void InitSceneList()
    {
        // 获取所有的场景名称
        Debug.Log("场景数量1：" + DataManager.Instance.scenesList.Length);
        scenesList = new List<DataManager.SceneInfo>(DataManager.Instance.scenesList);
        Debug.Log("场景数量：" + scenesList.Count);
        string[] sceneNames = new string[scenesList.Count];
        for (int i = 0; i < scenesList.Count; i++)
        {
            sceneNames[i] = scenesList[i].SceneName; // 获取场景路径
        }
        // 渲染所有场景到ScrollView
        RenderSceneButtons(sceneNames);
    }

    void RenderSceneButtons(string[] sceneNames)
    {
        // 清空之前的按钮
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        foreach (string sceneName in sceneNames)
        {
            // 创建按钮实例
            GameObject buttonObject = Instantiate(buttonPrefab, scrollViewContent);
            Button button = buttonObject.GetComponent<Button>();
            Text buttonText = buttonObject.GetComponentInChildren<Text>();
            // 设置按钮文本为场景名称
            buttonText.text = System.IO.Path.GetFileNameWithoutExtension(sceneName);

            // 为按钮添加点击事件
            button.onClick.AddListener(() => SceneChange(buttonText.text));

            // Debugging: 确认按钮是否被创建
            Debug.Log("Button created for scene: " + buttonText.text + " under parent: " + buttonObject.transform.parent.name);
        }
        Debug.Log("Current child count in scrollViewContent: " + scrollViewContent.childCount);
    }

    public void SceneChange(string sceneName)
    {
        // 跳转到对应的场景
        SceneManager.LoadScene(sceneName);
    }
}
