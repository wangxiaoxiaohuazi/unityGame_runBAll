using UnityEngine;
using UnityEditor;

public class LevelUnlockEditor : EditorWindow
{
    private int levelToUnlock;

    [MenuItem("Tools/解锁关卡")]
    public static void ShowWindow()
    {
        GetWindow<LevelUnlockEditor>("Unlock Level");
    }

    private void OnGUI()
    {
        GUILayout.Label("Unlock Level", EditorStyles.boldLabel);
        levelToUnlock = EditorGUILayout.IntField("Level ID:", levelToUnlock);

        if (GUILayout.Button("Unlock Level"))
        {
            UnlockLevel(levelToUnlock);
        }
    }

    private void UnlockLevel(int levelId)
    {
        // 假设 DataManager 是你的数据管理类
        if (DataManager.Instance != null)
        {
            // 解锁关卡的逻辑
            var roundInfo = DataManager.Instance.gameInfo.roundInfo;
            if (roundInfo != null)
            {
                // 这里假设你有一个方法来解锁关卡
                roundInfo.currentLevel = levelId;
                DataManager.Instance.SaveData();
                Debug.Log($"关卡 {levelId} 已解锁");
            }
            else
            {
                Debug.LogError("roundInfo 未初始化");
            }
        }
        else
        {
            Debug.LogError("DataManager 未初始化");
        }
    }
}