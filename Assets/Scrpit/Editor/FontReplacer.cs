using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FontReplacer : EditorWindow
{
    private Font newUnityFont;
    private TMP_FontAsset newTMPFont;
    private bool includeInactive = true;

    [MenuItem("Tools/批量替换字体")]
    public static void ShowWindow()
    {
        GetWindow<FontReplacer>("字体替换工具");
    }

    void OnGUI()
    {
        GUILayout.Label("选择新字体", EditorStyles.boldLabel);
        newUnityFont = (Font)EditorGUILayout.ObjectField("UI Text 字体", newUnityFont, typeof(Font), false);
        newTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("TextMeshPro 字体", newTMPFont, typeof(TMP_FontAsset), false);
        includeInactive = EditorGUILayout.Toggle("包含未激活对象", includeInactive);

        if (GUILayout.Button("执行替换"))
        {
            ReplaceFontsInScene();
            ReplaceFontsInPrefabs();
            ReplaceFontsInAssets();
            Debug.Log("字体替换完成");
        }
    }

    private void ReplaceFontsInScene()
    {
        // 替换场景中的Text
        foreach (Text text in Resources.FindObjectsOfTypeAll<Text>())
        {
            if (ShouldProcess(text.gameObject))
            {
                Undo.RecordObject(text, "Change Font");
                text.font = newUnityFont;
            }
        }

        // 替换场景中的TextMeshPro
        foreach (TMP_Text tmpText in Resources.FindObjectsOfTypeAll<TMP_Text>())
        {
            if (ShouldProcess(tmpText.gameObject))
            {
                Undo.RecordObject(tmpText, "Change TMP Font");
                tmpText.font = newTMPFont;
            }
        }
    }

    private void ReplaceFontsInPrefabs()
    {
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab");
        foreach (string path in prefabPaths)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(path);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            bool modified = false;
            foreach (Text text in prefab.GetComponentsInChildren<Text>(includeInactive))
            {
                text.font = newUnityFont;
                modified = true;
            }

            foreach (TMP_Text tmpText in prefab.GetComponentsInChildren<TMP_Text>(includeInactive))
            {
                tmpText.font = newTMPFont;
                modified = true;
            }

            if (modified)
            {
                PrefabUtility.SavePrefabAsset(prefab);
            }
        }
    }

    private void ReplaceFontsInAssets()
    {
        // 替换ScriptableObject中的字体引用
        string[] assetPaths = AssetDatabase.FindAssets("t:ScriptableObject");
        foreach (string path in assetPaths)
        {
            ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                AssetDatabase.GUIDToAssetPath(path));
            
            SerializedObject serializedObject = new SerializedObject(asset);
            SerializedProperty prop = serializedObject.GetIterator();
            
            while (prop.NextVisible(true))
            {
                if (prop.propertyType == SerializedPropertyType.ObjectReference)
                {
                    if (prop.objectReferenceValue is Font)
                    {
                        prop.objectReferenceValue = newUnityFont;
                        serializedObject.ApplyModifiedProperties();
                    }
                    else if (prop.objectReferenceValue is TMP_FontAsset)
                    {
                        prop.objectReferenceValue = newTMPFont;
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }
    }

    private bool ShouldProcess(GameObject obj)
    {
        return includeInactive || obj.activeInHierarchy;
    }
}
