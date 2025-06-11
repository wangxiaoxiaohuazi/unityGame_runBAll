using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class BatchSceneToPrefab 
{
    [MenuItem("Tools/批量场景转换预制体")]
    public static void ConvertAllScenesToPrefabs()
    {
        // 确保目标目录存在
        string prefabFolderPath = "Assets/prefabs/level";
        if (!Directory.Exists(prefabFolderPath))
        {
            Directory.CreateDirectory(prefabFolderPath);
            AssetDatabase.Refresh();
            Debug.Log($"创建目录: {prefabFolderPath}");
        }

        // 获取Build Settings中的所有场景
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        int processedCount = 0;
        
        // 遍历所有场景
        foreach (var buildScene in buildScenes)
        {
            if (!buildScene.enabled) 
                continue;

            string scenePath = buildScene.path;
            if (string.IsNullOrEmpty(scenePath))
                continue;

            // 打开场景
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            
            // 获取场景根对象
            GameObject[] rootObjects = scene.GetRootGameObjects();
            
            if (rootObjects.Length == 0)
            {
                Debug.LogWarning($"场景 {scene.name} 没有根节点，跳过");
                continue;
            }

            // 创建根节点容器
            GameObject sceneRoot = new GameObject(scene.name + "_Root");
            
            // 将所有根对象设为容器的子对象
            foreach (GameObject obj in rootObjects)
            {
                obj.transform.SetParent(sceneRoot.transform);
            }

            // 确保场景名称为有效文件名
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            sceneName = sceneName.Replace(" ", "_");

            // 预制体路径
            string prefabPath = Path.Combine(prefabFolderPath, $"{sceneName}.prefab");
            
            // 创建预制体
            PrefabUtility.SaveAsPrefabAsset(sceneRoot, prefabPath);
            Debug.Log($"成功创建预制体: {prefabPath}");

            // 销毁临时创建的根对象
            Object.DestroyImmediate(sceneRoot);

            processedCount++;
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("完成", $"已成功处理 {processedCount} 个场景", "确定");
    }
}
