using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // 单例实例（全局访问点）
    public static AudioManager Instance { get; private set; }

    [Header("音频源配置")]
    [SerializeField] private AudioSource musicSource;  // 背景音乐源
    [SerializeField] private AudioSource sfxSource;     // 音效源

    [Header("音量控制")]
    [Range(0, 1)] public float musicVolume = 1f;
    [Range(0, 1)] public float sfxVolume = 1f;

    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    [Header("音效池配置")]
    [SerializeField] private int initialPoolSize = 2; // 初始对象池大小

    private List<AudioSource> sfxPool = new List<AudioSource>();
    private Coroutine musicFadeCoroutine;

    public enum PlayMode { SingleLoop, ListLoop, Random };
    [SerializeField] private PlayMode musicPlayMode = PlayMode.ListLoop;
    [SerializeField] private List<string> playList = new List<string>();
    private int currentMusicIndex = -1;
    private Coroutine musicMonitorCoroutine;
    //// 在游戏启动时调用
    // 加载背景音乐
    // AudioClip mainBGM = Resources.Load<AudioClip>("Audio/Music/MainTheme");
    // AudioManager.Instance.AddAudioClip("MainBGM", mainBGM);

    // // 加载音效
    // AudioClip buttonSFX = Resources.Load<AudioClip>("Audio/SFX/ButtonClick");
    // AudioManager.Instance.AddAudioClip("ButtonClick", buttonSFX);

    // 绑定到音乐音量Slider
    // public void OnMusicVolumeChanged(float value)
    // {
    //     AudioManager.Instance.SetMusicVolume(value);
    // }

    // // 绑定到音效音量Slider
    // public void OnSFXVolumeChanged(float value)
    // {
    //     AudioManager.Instance.SetSFXVolume(value);
    // }
    //     // 开始游戏时播放背景音乐
    // AudioManager.Instance.PlayMusic("MainBGM");

    // // 切换战斗音乐（带淡入淡出）
    // AudioManager.Instance.FadeMusic("BattleBGM", 2f);
    // 连续播放多次音效
    // public void PlayMultipleSFX()
    // {
    //     for (int i = 0; i < 10; i++)
    //     {
    //         AudioManager.Instance.PlaySFX("爆炸音效"); // 使用对象池自动管理
    //     }
    // }
    void Awake()
    {
        // 单例模式初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 跨场景保留
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        // 初始化音效池
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewSFXSource();
        }
    }

    // 加载音频资源（在编辑器中将音频文件拖拽到对应位置）
    public void AddAudioClip(string name, AudioClip clip)
    {
        if (!audioClips.ContainsKey(name))
        {
            audioClips.Add(name, clip);
            Debug.Log("Added audio clip: " + name);
            Debug.Log("Total audio clips: " + audioClips.Count);
        }
    }
    void Update()
    {
        // 检测鼠标点击
        if (Input.GetMouseButtonDown(0)) // 0 表示左键点击
        {
            PlaySFX("click");
        }

        // 检测触摸输入
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) // 触摸开始
            {
                PlaySFX("click");
            }
        }
        // 修改后的自然结束处理（增加停止状态判断）
        if (musicSource.clip != null &&
            !musicSource.isPlaying &&
            musicSource.time >= musicSource.clip.length - 0.1f &&
            currentMusicIndex != -1) // 增加有效索引判断
        {
            PlayNextMusic();
        }
    }
    /// <summary>
    /// 设置播放列表
    /// </summary>
    public void SetPlayList(List<string> newList, string defaultBGM = "")
    {
        playList.Clear();
        playList.AddRange(newList);

        if (!string.IsNullOrEmpty(defaultBGM))
        {
            int index = playList.IndexOf(defaultBGM);
            currentMusicIndex = index != -1 ? index : 0;
        }
        else
        {
            currentMusicIndex = 0;
        }
    }

    /// <summary>
    /// 开始播放背景音乐列表
    /// </summary>
    public void StartPlaylist(string startBGM)
    {
        if (playList.Count == 0 || DataManager.Instance.gameInfo.player.musicVisible == false)
        {
            Debug.LogWarning("播放列表为空");
            return;
        }
        int startIndex = playList.IndexOf(startBGM);
        if (startIndex == -1)
        {
            Debug.LogError($"起始音乐{startBGM}不在播放列表中");
            startIndex = 0;
        }

        currentMusicIndex = startIndex;
        musicSource.loop = false;
        musicSource.Stop(); // 确保从干净状态开始
        PlayMusicDirectly(playList[currentMusicIndex]);
        if (musicMonitorCoroutine != null)
            StopCoroutine(musicMonitorCoroutine);
        musicMonitorCoroutine = StartCoroutine(MonitorMusicPlaying());
    }

    private void PlayNextMusic()
    {
        if (playList.Count == 0 || currentMusicIndex == -1) return; // 增加停止状态判断

        // 根据播放模式选择下一首
        switch (musicPlayMode)
        {
            case PlayMode.ListLoop:
                currentMusicIndex = (currentMusicIndex + 1) % playList.Count;
                break;
            case PlayMode.Random:
                currentMusicIndex = Random.Range(0, playList.Count);
                break;
        }

        string nextMusic = playList[currentMusicIndex];
        PlayMusicDirectly(nextMusic);

        // 启动播放监控协程
        if (musicMonitorCoroutine != null)
            StopCoroutine(musicMonitorCoroutine);
        musicMonitorCoroutine = StartCoroutine(MonitorMusicPlaying());
    }
    private IEnumerator MonitorMusicPlaying()
    {
        // 等待当前音乐真正开始播放
        yield return new WaitUntil(() => musicSource.isPlaying);

        AudioClip currentClip = musicSource.clip;
        float checkInterval = 0.1f;

        while (true)
        {
            // 情况1：用户主动切歌
            if (musicSource.clip != currentClip) yield break;

            // 情况2：进度超过95%或自然结束
            if (musicSource.time >= currentClip.length * 0.95f || !musicSource.isPlaying)
            {
                // 重置时间防止循环检测
                if (!musicSource.isPlaying) musicSource.time = 0;

                FadeToNextMusic();
                yield break;
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }
    private void PlayMusicDirectly(string musicName)
    {

        if (audioClips.TryGetValue(musicName, out AudioClip clip))
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
        else
        {
            Debug.Log("找不到音乐" + musicName);
        }
    }
    // 背景音乐控制
    public void PlayMusic(string musicName)
    {
        if (!DataManager.Instance.gameInfo.player.musicVisible) return;

        // 如果是播放列表中的音乐，重置索引
        int index = playList.IndexOf(musicName);
        if (index != -1)
        {
            currentMusicIndex = index;
            PlayMusicDirectly(musicName);

            if (musicMonitorCoroutine != null)
                StopCoroutine(musicMonitorCoroutine);
            musicMonitorCoroutine = StartCoroutine(MonitorMusicPlaying());
        }
        else
        {
            Debug.LogError("音乐不在播放列表中: " + musicName);
        }
    }

    public void StopMusic()
    {
        // 停止所有音乐相关协程
        if (musicMonitorCoroutine != null)
        {
            StopCoroutine(musicMonitorCoroutine);
            musicMonitorCoroutine = null;
        }

        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
            musicFadeCoroutine = null;
        }

        // 完全重置音乐源状态
        musicSource.Stop();
        musicSource.clip = null;
        currentMusicIndex = -1;
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    // 音效控制
    public void PlaySFX(string sfxName)
    {
        if (!DataManager.Instance.gameInfo.player.soundVisible) return;
        if (audioClips.TryGetValue(sfxName, out AudioClip clip))
        {
            AudioSource source = GetAvailableSFXSource();
            source.clip = clip;
            source.volume = sfxVolume;
            source.Play();
        }
    }
    private AudioSource GetAvailableSFXSource()
    {
        foreach (AudioSource source in sfxPool)
        {
            if (!source.isPlaying) return source;
        }
        return CreateNewSFXSource(); // 自动扩容
    }
    private AudioSource CreateNewSFXSource()
    {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        sfxPool.Add(newSource);
        return newSource;
    }
    // 音量调节
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);

        // 更新所有音效源的音量
        foreach (var source in sfxPool)
        {
            source.volume = sfxVolume;
        }
    }
    public void FadeMusic(string newMusicName, float duration = 1f)
    {
        if (!DataManager.Instance.gameInfo.player.musicVisible) return;
        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
        }
        musicFadeCoroutine = StartCoroutine(FadeMusicCoroutine(newMusicName, duration));
    }
    private IEnumerator FadeMusicCoroutine(string newMusicName, float duration)
    {
        // 淡出当前音乐
        float startVolume = musicSource.volume;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, timer / duration);
            yield return null;
        }

        musicSource.clip = audioClips[newMusicName];
        musicSource.Play();
        musicSource.volume = 0; // 初始静音

        // 淡入新音乐
        timer = 0;
        startVolume = 0;
        float targetVolume = musicVolume;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
            yield return null;
        }
    }
    /// <summary>
    /// 带淡出效果的切歌
    /// </summary>
    public void FadeToNextMusic(float fadeDuration = 1f)
    {
        StartCoroutine(FadeAndSwitch(fadeDuration));
    }

    private IEnumerator FadeAndSwitch(float duration)
    {
        // 淡出当前音乐
        float startVolume = musicSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, timer / duration);
            yield return null;
        }

        // 确保停止当前播放
        musicSource.Stop();

        // 切换并播放下一首
        PlayNextMusic();

        // 淡入新音乐
        timer = 0f;
        while (timer < duration && musicSource.clip != null)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, musicVolume, timer / duration);
            yield return null;
        }
    }
    void Start()
    {
        // 加载保存的音量设置
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

}
