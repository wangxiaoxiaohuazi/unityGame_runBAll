using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
public class InitGame : MonoBehaviour
{
    public GameObject Progress;
    public Text ProgressText; // 进度文本引用
    [Header("加载配置")]
    [SerializeField][Range(0, 1)] private float addressableLoadWeight = 0.5f;  // Addressable加载权重
    [SerializeField][Range(0, 1)] private float audioLoadWeight = 0.25f;  // 音频加载权重
    [SerializeField][Range(0, 1)] private float sceneLoadWeight = 0.25f;  // 场景加载权重
                                                                          // 存储加载句柄
    private AsyncOperationHandle<SceneInstance> _sceneLoadHandle;
    // 需要加载的音频路径配置Assets/Origin/Music/music1.mp3
    // private readonly string[] musicPaths =
    // {
    //     "Assets/Origin/Music/music1",
    //     "Assets/Origin/Music/music2",
    //     "Assets/Origin/Music/music3",
    //     "Assets/Origin/Music/music4",
    //     "Assets/Origin/Music/music5",
    //     "Assets/Origin/Music/music6",
    // };

    // private readonly string[] sfxPaths =
    // {
    //     "Assets/Origin/Music/SFX/click",
    //     "Assets/Origin/Music/SFX/success"
    // };
    private readonly string[] musicPaths =
  {
        "Assets/Origin/Music/music1.mp3",
        "Assets/Origin/Music/music2.mp3",
        "Assets/Origin/Music/music3.mp3",
        "Assets/Origin/Music/music4.mp3",
        "Assets/Origin/Music/music5.mp3",
        "Assets/Origin/Music/music6.mp3",
    };

    private readonly string[] sfxPaths =
    {
        // "Assets/Origin/Music/SFX/click.mp3",
        // "Assets/Origin/Music/SFX/success.mp3"
    };
    //     private readonly string[] musicPaths =
    //   {
    //         "Music/music1",
    //         "Music/music2",
    //         "Music/music3",
    //         "Music/music4",
    //         "Music/music5",
    //         "Music/music6",
    //     };

    //     private readonly string[] sfxPaths =
    //     {
    //         "Music/SFX/click",
    //         "Music/SFX/success"
    //     };
    private string[] groupNames = { }; // 这里需要替换为实际的Group名称
    void Start()
    {
        _Init();
    }

    void _Init()
    {
        foreach (var item in DataManager.Instance.gameInfo.roundInfo.levelSceneList)
        {
            if (item.id == DataManager.Instance.gameInfo.roundInfo.currentLevel)
            {
                StartCoroutine(LoadAllContent(item.scenePath));
                break;
            }
        }
        DataManager.Instance.gameInfo.roundInfo.pickLevel = DataManager.Instance.gameInfo.roundInfo.currentLevel; // 初始化当前关卡

    }

    private IEnumerator LoadAllContent(string sceneKey)
    {
        float totalProgress = 0f;

        // 第一阶段：Addressable资源加载
        yield return StartCoroutine(LoadAddressableResources(progress =>
        {
            totalProgress = progress * addressableLoadWeight;
            UpdateProgress(totalProgress);
        }));

        // 第二阶段：音频加载
        yield return StartCoroutine(LoadAudioResources(progress =>
        {
            totalProgress = addressableLoadWeight + progress * audioLoadWeight;
            UpdateProgress(totalProgress);
        }));

        // 第三阶段：场景加载
        yield return StartCoroutine(LoadSceneWithProgress(sceneKey, progress =>
        {
            totalProgress = addressableLoadWeight + audioLoadWeight + progress * sceneLoadWeight;
            UpdateProgress(totalProgress);
        }));

        // 可选：加载完成后的延迟
        yield return new WaitForSeconds(0.5f);
    }

    //Addressable资源加载
    private IEnumerator LoadAudioResources(System.Action<float> onProgress)
    {
        List<AsyncOperationHandle<AudioClip>> requests = new List<AsyncOperationHandle<AudioClip>>();
        int totalCount = musicPaths.Length + sfxPaths.Length;
        int completedCount = 0;
        float currentProgress = 0f;

        // 开始加载所有音频
        foreach (var path in musicPaths)
        {
            // requests.Add(Addressables.LoadAssetAsync<AudioClip>(path + ".mp3"));
            requests.Add(Addressables.LoadAssetAsync<AudioClip>(path));
        }
        foreach (var path in sfxPaths)
        {
            // requests.Add(Addressables.LoadAssetAsync<AudioClip>(path + ".mp3"));
            requests.Add(Addressables.LoadAssetAsync<AudioClip>(path));
        }

        // 监控加载进度
        while (completedCount < totalCount)
        {
            completedCount = 0;
            float totalProgress = 0f;

            foreach (var req in requests)
            {
                if (req.IsDone) completedCount++;
                totalProgress += req.PercentComplete;
            }

            currentProgress = totalProgress / totalCount;
            onProgress?.Invoke(currentProgress);
            yield return null;
        }

        // 将加载的资源注册到AudioManager
        foreach (var req in requests)
        {
            if (req.Status == AsyncOperationStatus.Succeeded)
            {
                AudioClip clip = req.Result;
                clip.LoadAudioData();
                AudioManager.Instance.AddAudioClip(clip.name, clip);
            }
            else
            {
                Debug.LogError("加载音乐失败: " + req.Result.name);
            }
        }

        // // 释放加载句柄
        // foreach (var req in requests)
        // {
        //     Addressables.Release(req);
        // }
    }

    //Resource资源加载
    // private IEnumerator LoadAudioResources(System.Action<float> onProgress)
    // {
    //     List<ResourceRequest> requests = new List<ResourceRequest>();
    //     int totalCount = musicPaths.Length + sfxPaths.Length;
    //     int completedCount = 0;
    //     float currentProgress = 0f;

    //     // 开始加载所有音频
    //     foreach (var path in musicPaths)
    //     {
    //         requests.Add(Resources.LoadAsync<AudioClip>(path));
    //     }
    //     foreach (var path in sfxPaths)
    //     {
    //         requests.Add(Resources.LoadAsync<AudioClip>(path));
    //     }

    //     // 监控加载进度
    //     while (completedCount < totalCount)
    //     {
    //         completedCount = 0;
    //         float totalProgress = 0f;

    //         foreach (var req in requests)
    //         {
    //             if (req.isDone) completedCount++;
    //             totalProgress += req.progress;
    //         }

    //         currentProgress = totalProgress / totalCount;
    //         onProgress?.Invoke(currentProgress);
    //         yield return null;
    //     }

    //     // 将加载的资源注册到AudioManager
    //     foreach (var req in requests)
    //     {
    //         if (req.asset is AudioClip clip)
    //         {
    //             AudioManager.Instance.AddAudioClip(clip.name, clip);
    //         }
    //     }
    // }
    private IEnumerator LoadSceneWithProgress(string scenePath, System.Action<float> onProgress)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");
        List<string> myPlaylist = new List<string>();

        foreach (var path in musicPaths)
        {
            // 提取最后一个斜杠后的字符串，并去掉文件扩展名
            Debug.Log("提取的名字1：" + path);
            // var match = System.Text.RegularExpressions.Regex.Match(path, @"[^/]+$"); // 使用正则表达式提取最后一个斜杠后的字符串
            var match = System.Text.RegularExpressions.Regex.Match(path, @"[^/]+(?=\.[^/.]+$)");
            Debug.Log("提取的名字2：" + match.Value);
            if (match.Success)
            {
                myPlaylist.Add(match.Value); // 添加到 myPlaylist
            }
        }
        if (myPlaylist.Count > 0)
        {
            AudioManager.Instance.SetPlayList(myPlaylist, myPlaylist[0]);
            if (DataManager.Instance.gameInfo.player.musicVisible)
            {
                System.Random random = new System.Random();
                int randomIndex = random.Next(myPlaylist.Count); // 生成随机索引
                string randomSong = myPlaylist[randomIndex]; // 获取随机歌曲
                AudioManager.Instance.StartPlaylist(randomSong);
            }
        }
        string roundName = RoundInfo.Instance.OnGetCurrentLevel().scenePath;
        var loadGroupOperation = Addressables.LoadAssetAsync<GameObject>(
                 roundName
              );
        asyncLoad.allowSceneActivation = false;
        // 第一阶段：0-0.9的加载
        while (asyncLoad.progress < 0.9f)
        {
            onProgress?.Invoke(asyncLoad.progress);
            yield return null;
        }
        // 第二阶段：等待用户允许激活
        asyncLoad.allowSceneActivation = true;

        // 确保加载完成
        while (!asyncLoad.isDone)
        {
            onProgress?.Invoke(1f);
            yield return null;
        }
    }

    private IEnumerator LoadAddressableResources(System.Action<float> onProgress)
    {
        // 获取所有需要加载的Group名称

        List<AsyncOperationHandle> loadOperations = new List<AsyncOperationHandle>();
        float totalProgress = 0f;
        Addressables.InitializeAsync().Completed += op =>
        {
            foreach (var groupName in groupNames)
            {
                var loadGroupOperation = Addressables.LoadAssetsAsync<GameObject>(
                   groupName,
                   null,
                   Addressables.MergeMode.Intersection
                );
                loadOperations.Add(loadGroupOperation);
            }
        };
        // 为每个Group创建加载操作


        // 监控所有加载操作的进度
        while (!loadOperations.TrueForAll(op => op.IsDone))
        {
            totalProgress = 0f;
            foreach (var operation in loadOperations)
            {
                totalProgress += operation.PercentComplete;
            }

            float currentProgress = totalProgress / loadOperations.Count;
            onProgress?.Invoke(currentProgress);
            yield return null;
        }

        // // 完成加载后，可以在这里处理加载完的资源
        // foreach (var operation in loadOperations)
        // {
        //     if (operation.Status == AsyncOperationStatus.Succeeded)
        //     {
        //         // 可以在这里处理加载成功的资源
        //         // 比如将资源存储到某个管理器中
        //     }
        // }
    }

    private void UpdateProgress(float progress)
    {
        progress = Mathf.Clamp01(progress); // 确保进度在0-1之间
        Progress.GetComponent<Scrollbar>().size = progress;
        if (progress == 1)
        {
            ProgressText.text = $"正在进入游戏...";

        }
        else
        {
            ProgressText.text = $"资源加载中... {progress * 100:0}%";
        }
    }
}
