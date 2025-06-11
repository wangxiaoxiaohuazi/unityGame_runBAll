#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class AddressableKeyLister : MonoBehaviour
{
    [MenuItem("Tools/检索并打印Addressables包含的Key")]
    public static void ListAllAddressableKeys()
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressable Settings not found!");
            return;
        }

        foreach (AddressableAssetGroup group in settings.groups)
        {
            if (group == null) continue;

            foreach (AddressableAssetEntry entry in group.entries)
            {
                Debug.Log($"Key: {entry.address} | Group: {group.name} | Path: {entry.AssetPath}");
            }
        }
    }
}
#endif