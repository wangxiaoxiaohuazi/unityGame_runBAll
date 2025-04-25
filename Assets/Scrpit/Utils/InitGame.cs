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
    [SerializeField][Range(0, 1)] private float audioLoadWeight = 0.4f;  // 音频加载权重
    [SerializeField][Range(0, 1)] private float sceneLoadWeight = 0.6f;  // 场景加载权重
                                                                         // 存储加载句柄
    private AsyncOperationHandle<SceneInstance> _sceneLoadHandle;
    // 需要加载的音频路径配置
    private readonly string[] musicPaths =
    {
        "Music/music1",
        "Music/music2",
        "Music/music3",
        "Music/music4",
        "Music/music5",
        "Music/music6",
    };

    private readonly string[] sfxPaths =
    {
        "Music/SFX/click",
        "Music/SFX/success"
    };

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
    }

    private IEnumerator LoadAllContent(string sceneKey)
    {
        float totalProgress = 0f;

        // 第一阶段：音频加载
        yield return StartCoroutine(LoadAudioResources(progress =>
        {
            totalProgress = progress * audioLoadWeight;
            UpdateProgress(totalProgress);
        }));
        //可拓展加载其他内容


        // 第二阶段：场景加载
        yield return StartCoroutine(LoadAddressableSceneWithProgress(sceneKey, progress =>
        {
            totalProgress = audioLoadWeight + progress * sceneLoadWeight;
            UpdateProgress(totalProgress);
        }));
        // 可选：加载完成后的延迟
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator LoadAudioResources(System.Action<float> onProgress)
    {
        List<ResourceRequest> requests = new List<ResourceRequest>();
        int totalCount = musicPaths.Length + sfxPaths.Length;
        int completedCount = 0;
        float currentProgress = 0f;

        // 开始加载所有音频
        foreach (var path in musicPaths)
        {
            requests.Add(Resources.LoadAsync<AudioClip>(path));
        }
        foreach (var path in sfxPaths)
        {
            requests.Add(Resources.LoadAsync<AudioClip>(path));
        }

        // 监控加载进度
        while (completedCount < totalCount)
        {
            completedCount = 0;
            float totalProgress = 0f;

            foreach (var req in requests)
            {
                if (req.isDone) completedCount++;
                totalProgress += req.progress;
            }

            currentProgress = totalProgress / totalCount;
            onProgress?.Invoke(currentProgress);
            yield return null;
        }

        // 将加载的资源注册到AudioManager
        foreach (var req in requests)
        {
            if (req.asset is AudioClip clip)
            {
                AudioManager.Instance.AddAudioClip(clip.name, clip);
            }
        }
    }
    private IEnumerator LoadAddressableSceneWithProgress(string sceneKey, System.Action<float> onProgress)
    {
        // 开始加载场景
        AddressablesLoaderManager.Instance.SwitchScene(sceneKey);

        // 设置播放列表
        List<string> myPlaylist = new List<string>();
        foreach (var path in musicPaths)
        {
            var match = System.Text.RegularExpressions.Regex.Match(path, @"[^/]+$");
            if (match.Success)
            {
                myPlaylist.Add(match.Value);
            }
        }

        if (myPlaylist.Count > 0)
        {
            AudioManager.Instance.SetPlayList(myPlaylist, myPlaylist[0]);
            System.Random random = new System.Random();
            int randomIndex = random.Next(myPlaylist.Count);
            string randomSong = myPlaylist[randomIndex];
            AudioManager.Instance.StartPlaylist(randomSong);
        }
        yield return null;  // 添加这行代码，确保协程在加载场景后继续执行
    }

    private void UpdateProgress(float progress)
    {
        Progress.GetComponent<Scrollbar>().size = progress;
        ProgressText.text = $"资源加载中... {progress * 100:0}%";
    }
}
