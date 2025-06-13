using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

public class InitRound : MonoBehaviour
{
    public AssetReference roundPref;
    public AssetReference[] skyboxPrefs; // 天空盒材质数组

    // Start is called before the first frame update
    void Start()
    {
        CreateRound();
        // 随机选择一个天空盒
        if (skyboxPrefs != null && skyboxPrefs.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, skyboxPrefs.Length);
            skyboxPrefs[randomIndex].LoadAssetAsync<Material>().Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    RenderSettings.skybox = handle.Result;
                }
            };
        }
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
        await UnloadPreviousRound();
        //随机渲染天空盒

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

        GameObject roundPrefab;
        if (roundPref != null && roundPref.RuntimeKeyIsValid())
        {
            roundPrefab = await LoadRoundPrefabAsync(roundPref);
            Debug.Log("加载预制体成功1");
        }
        else
        {
            string roundName = RoundInfo.Instance.OnGetCurrentLevel().scenePath;
            roundPrefab = await LoadRoundPrefabAsync(roundName);
            Debug.Log("加载预制体成功2");
        }

        if (roundPrefab != null)
        {
            Instantiate(roundPrefab, transform.parent);
            Debug.Log("实例化预制体成功");
        }
        else
        {
            Debug.LogError("加载关卡预制体失败");
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
    //卸载前一个关卡预制体
    private async Task UnloadPreviousRound()
    {
        string prevRoundName = RoundInfo.Instance.GetPreLevel()?.scenePath;
        if (!string.IsNullOrEmpty(prevRoundName))
        {
            await Task.Run(() => Addressables.Release(prevRoundName));
        }
    }
}
