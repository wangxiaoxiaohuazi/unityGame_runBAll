using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;

public class InitRound : MonoBehaviour
{
    public AssetReference roundPref;
    // public AssetReference skyboxPref; // 添加天空盒引用

    // Start is called before the first frame update
    void Start()
    {
        CreateRound();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public async void CreateRound()
    {
        Debug.Log("创建关卡");
        await ClearScene();
        await LoadAndInstantiateRound();
        await PreloadNextRound();
    }

    private Task ClearScene()
    {
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            if (obj != gameObject)
            {
                Debug.Log($"移除物体: {obj.name}");
                Destroy(obj);
            }
        }
        return Task.CompletedTask;
    }

    private async Task LoadAndInstantiateRound()
    {
        try
        {
            GameObject roundPrefab;
            if (roundPref != null && roundPref.RuntimeKeyIsValid())
            {
                roundPrefab = await LoadRoundPrefabAsync(roundPref);
            }
            else
            {
                string roundName = RoundInfo.Instance.OnGetCurrentLevel().scenePath;
                roundPrefab = await LoadRoundPrefabAsync(roundName);
            }

            if (roundPrefab != null)
            {
                Instantiate(roundPrefab, transform.parent);
            }
            else
            {
                Debug.LogError("加载关卡预制体失败");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"加载关卡时发生错误: {e.Message}");
        }
    }

    private async Task<GameObject> LoadRoundPrefabAsync(object key)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(key);
        return await handle.Task;
    }

    private async Task PreloadNextRound()
    {
        try
        {
            string nextRoundName = RoundInfo.Instance.GetNextLevel().scenePath;
            await LoadRoundPrefabAsync(nextRoundName);
        }
        catch (Exception e)
        {
            Debug.LogError($"预加载下一关时发生错误: {e.Message}");
        }
    }
}
