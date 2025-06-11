// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;
// using UnityEngine.ResourceManagement.ResourceProviders;
// using UnityEngine.SceneManagement;
// using System.Collections.Generic;
// using System.Collections;
// using System;
// using UnityEngine.ResourceManagement.ResourceLocations;

// public class AddressablesLoaderManager : MonoBehaviour
// {
//     private static AddressablesLoaderManager _instance;
//     public static AddressablesLoaderManager Instance => _instance;
//     // 新增私有字段存储Addressables场景Key
//     private string _currentAddressableKey;
//     // 当前加载的场景句柄
//     private SceneInstance _currentScene;
//     // 预加载的场景句柄缓存
//     private Dictionary<string, SceneInstance> _preloadedScenes = new Dictionary<string, SceneInstance>();
//     // 预加载的资源缓存
//     private Dictionary<string, AsyncOperationHandle> _preloadedAssets = new Dictionary<string, AsyncOperationHandle>();

//     [Header("预加载配置")]
//     [SerializeField] private float _preloadTimeout = 10f;
//     [SerializeField] private int _maxParallelLoads = 3;

//     [Header("调试信息")]
//     [SerializeField] private string _currentSceneName;
//     [SerializeField] private string _loadingProgress;


//     void Awake()
//     {
//         if (_instance == null)
//         {
//             _instance = this;
//             DontDestroyOnLoad(gameObject);
//             Initialize();
//         }
//         else
//         {
//             Destroy(gameObject);
//         }

//     }

//     void Initialize()
//     {
//         Addressables.InitializeAsync();
//         _currentScene = new SceneInstance();
//         _currentSceneName = SceneManager.GetActiveScene().name;

//     }
//     /// <summary>
//     /// 重新加载当前场景（带进度回调）
//     /// </summary>
//     public void ReloadCurrentScene(Action<float> onProgress = null)
//     {
//         if (string.IsNullOrEmpty(_currentAddressableKey))
//         {
//             Debug.LogError("当前没有有效的Addressables场景" + _currentAddressableKey);
//             return;
//         }
//         StartCoroutine(ReloadSceneRoutine(_currentAddressableKey, onProgress));
//     }

//     private IEnumerator ReloadSceneRoutine(string addressableKey, Action<float> onProgress)
//     {
//         // 阶段1：卸载当前场景
//         float progress = 0f;
//         var unloadOp = Addressables.UnloadSceneAsync(_currentScene);
//         while (!unloadOp.IsDone)
//         {
//             progress = Mathf.Lerp(0f, 0.3f, unloadOp.PercentComplete);
//             onProgress?.Invoke(progress);
//             yield return null;
//         }

//         // 阶段2：加载新场景
//         var loadHandle = Addressables.LoadSceneAsync(
//             addressableKey,
//             LoadSceneMode.Single,
//             false,
//             0 // 重要：禁止自动释放
//         );

//         while (!loadHandle.IsDone)
//         {
//             progress = Mathf.Lerp(0.3f, 0.9f, loadHandle.PercentComplete);
//             onProgress?.Invoke(progress);
//             yield return null;
//         }

//         if (loadHandle.Status != AsyncOperationStatus.Succeeded)
//         {
//             Debug.LogError($"场景重载失败: {loadHandle.OperationException}");
//             yield break;
//         }

//         // 阶段3：激活场景
//         var activateOp = loadHandle.Result.ActivateAsync();
//         while (!activateOp.isDone)
//         {
//             progress = Mathf.Lerp(0.9f, 1f, activateOp.progress);
//             onProgress?.Invoke(progress);
//             yield return null;
//         }

//         // 更新当前场景信息
//         _currentScene = loadHandle.Result;
//         _currentSceneName = _currentScene.Scene.name;

//         // 释放旧句柄但保留场景引用
//         Addressables.Release(loadHandle);
//     } /// <summary>
//       /// 完整场景切换流程
//       /// </summary>
//     public void SwitchScene(string addressableKey, List<string> preloadAssets = null)
//     {
//         _currentAddressableKey = addressableKey;
//         StartCoroutine(SceneTransitionRoutine(addressableKey, preloadAssets));
//     }

//     private IEnumerator SceneTransitionRoutine(string addressableKey, List<string> preloadAssets)
//     {
//         // 阶段0：预加载资源
//         if (preloadAssets != null)
//         {
//             var preloadHandle = Addressables.LoadAssetsAsync<object>(
//                 preloadAssets,
//                 null,
//                 Addressables.MergeMode.Union
//             );

//             while (!preloadHandle.IsDone)
//             {
//                 _loadingProgress = $"资源预加载: {preloadHandle.PercentComplete:P0}";
//                 yield return null;
//             }
//         }

//         // 阶段1：加载场景
//         yield return StartCoroutine(LoadSceneRoutine(addressableKey));

//         // 阶段2：清理资源
//         CleanupPreviousSceneResources(addressableKey);
//     }
//     /// <summary>
//     /// 预加载常用资源
//     /// </summary>
//     public IEnumerator PreloadCommonAssets(List<string> assetKeys)
//     {
//         List<AsyncOperationHandle> handles = new List<AsyncOperationHandle>();

//         foreach (string key in assetKeys)
//         {
//             if (!_preloadedAssets.ContainsKey(key))
//             {
//                 var handle = Addressables.LoadAssetAsync<object>(key);
//                 handles.Add(handle);
//                 _preloadedAssets.Add(key, handle);
//             }
//         }

//         foreach (var handle in handles)
//         {
//             yield return handle;
//             if (handle.Status != AsyncOperationStatus.Succeeded)
//             {
//                 Debug.LogError($"预加载失败: {handle.DebugName}");
//             }
//         }
//     }

//     /// <summary>
//     /// 预加载下一个场景（后台静默加载）
//     /// </summary>
//     public void PreloadNextScene(string sceneName)
//     {
//         if (!_preloadedScenes.ContainsKey(sceneName))
//         {
//             StartCoroutine(PreloadSceneRoutine(sceneName));
//         }
//     }

//     private IEnumerator PreloadSceneRoutine(string sceneName)
//     {
//         var loadHandle = Addressables.LoadSceneAsync(
//             sceneName,
//             LoadSceneMode.Additive,
//             false // 不激活场景
//         );

//         _preloadedScenes.Add(sceneName, default);

//         while (!loadHandle.IsDone)
//         {
//             _loadingProgress = $"预加载进度: {loadHandle.PercentComplete:P0}";
//             yield return null;
//         }

//         if (loadHandle.Status == AsyncOperationStatus.Succeeded)
//         {
//             _preloadedScenes[sceneName] = loadHandle.Result;
//             Debug.Log($"场景预加载完成: {sceneName}");
//         }
//     }

//     private IEnumerator LoadSceneRoutine(string addressableKey)
//     {
//         // 卸载当前场景
//         if (_currentScene.Scene.IsValid())
//         {
//             yield return Addressables.UnloadSceneAsync(_currentScene).Task;
//         }

//         // 检查预加载
//         if (_preloadedScenes.TryGetValue(addressableKey, out SceneInstance preloadedScene))
//         {
//             _currentScene = preloadedScene;
//             var activateOp = _currentScene.ActivateAsync();
//             while (!activateOp.isDone) yield return null;
//         }
//         else
//         {
//             var loadHandle = Addressables.LoadSceneAsync(
//                 addressableKey,
//                 LoadSceneMode.Single,
//                 false, // 不自动激活
//                 0   // 自动释放
//             );

//             yield return loadHandle;

//             if (loadHandle.Status == AsyncOperationStatus.Succeeded)
//             {
//                 _currentScene = loadHandle.Result;
//                 var activateOp = _currentScene.ActivateAsync();
//                 while (!activateOp.isDone) yield return null;
//             }
//         }

//         _currentAddressableKey = addressableKey;
//         _currentSceneName = _currentScene.Scene.name;
//     }
//     private IEnumerator PreloadAssetsRoutine(List<string> assetKeys)
//     {
//         List<AsyncOperationHandle> currentLoads = new List<AsyncOperationHandle>();
//         int totalCount = assetKeys.Count;
//         int loadedCount = 0;

//         foreach (string key in assetKeys)
//         {
//             if (_preloadedAssets.ContainsKey(key)) continue;

//             var handle = Addressables.LoadAssetAsync<object>(key);
//             currentLoads.Add(handle);

//             if (currentLoads.Count >= _maxParallelLoads)
//             {
//                 yield return AwaitProgress(currentLoads, totalCount);
//                 loadedCount += currentLoads.Count;
//                 currentLoads.Clear();
//             }
//         }

//         if (currentLoads.Count > 0)
//         {
//             yield return AwaitProgress(currentLoads, totalCount);
//             loadedCount += currentLoads.Count;
//         }
//     }

//     private IEnumerator AwaitProgress(List<AsyncOperationHandle> handles, int total)
//     {
//         int counter = 0;
//         foreach (var handle in handles)
//         {
//             while (!handle.IsDone)
//             {
//                 UpdateProgress(counter + handle.PercentComplete, total);
//                 yield return null;
//             }

//             if (handle.Status == AsyncOperationStatus.Succeeded)
//             {
//                 _preloadedAssets.Add(handle.DebugName, handle);
//                 counter++;
//                 UpdateProgress(counter, total);
//             }
//         }
//     }

//     private void UpdateProgress(float current, float total)
//     {
//         _loadingProgress = $"资源加载: {current}/{total} ({current / total:P0})";
//     }

//     private void CleanupPreviousSceneResources(string newScene)
//     {
//         List<string> toRemove = new List<string>();

//         foreach (var asset in _preloadedAssets)
//         {
//             if (!IsAssetUsedInScene(asset.Key, newScene))
//             {
//                 Addressables.Release(asset.Value);
//                 toRemove.Add(asset.Key);
//             }
//         }

//         foreach (string key in toRemove)
//         {
//             _preloadedAssets.Remove(key);
//         }

//         // 清理预加载场景缓存
//         if (_preloadedScenes.Count > 2) // 保持最近两个场景缓存
//         {
//             var oldest = _preloadedScenes.Keys.GetEnumerator();
//             oldest.MoveNext();
//             Addressables.Release(_preloadedScenes[oldest.Current]);
//             _preloadedScenes.Remove(oldest.Current);
//         }
//     }

//     private bool IsAssetUsedInScene(string assetKey, string sceneName)
//     {
//         // 这里需要实现你的资源使用判断逻辑
//         // 示例：根据命名规则判断
//         return assetKey.StartsWith("Common/") ||
//                assetKey.StartsWith($"{sceneName}/");
//     }

//     /// <summary>
//     /// 主动释放资源
//     /// </summary>
//     public void ReleaseAsset(string assetKey)
//     {
//         if (_preloadedAssets.TryGetValue(assetKey, out var handle))
//         {
//             Addressables.Release(handle);
//             _preloadedAssets.Remove(assetKey);
//         }
//     }

//     public T GetAsset<T>(string assetKey) where T : class
//     {
//         if (_preloadedAssets.TryGetValue(assetKey, out var handle))
//         {
//             return handle.Result as T;
//         }
//         return null;
//     }
// }

