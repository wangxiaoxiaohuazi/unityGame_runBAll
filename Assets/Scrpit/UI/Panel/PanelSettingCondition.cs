using UnityEngine;
using UnityEngine.UI;

public class PanelSettingCondition : MonoBehaviour
{
    private PanelManager panelManager;
    public Toggle MusicToggle;
    public Toggle SoundToggle;
    public Toggle VibrationToggle;


    // Start is called before the first frame update
    void Start()
    {
        PanelManager panelManager = FindObjectOfType<PanelManager>();
        // 修复3：使用回调锁定模式初始化Toggle状态
        SetToggleStateWithoutNotify(MusicToggle, DataManager.Instance.gameInfo.player.musicVisible);
        SetToggleStateWithoutNotify(SoundToggle, DataManager.Instance.gameInfo.player.soundVisible);
        SetToggleStateWithoutNotify(VibrationToggle, DataManager.Instance.gameInfo.player.vibrationVisible);
    }

    void OnEnable()
    {
        // 修复3：使用回调锁定模式初始化Toggle状态
        SetToggleStateWithoutNotify(MusicToggle, DataManager.Instance.gameInfo.player.musicVisible);
        SetToggleStateWithoutNotify(SoundToggle, DataManager.Instance.gameInfo.player.soundVisible);
        SetToggleStateWithoutNotify(VibrationToggle, DataManager.Instance.gameInfo.player.vibrationVisible);
    }
    // 修复4：添加安全的状态设置方法
    private void SetToggleStateWithoutNotify(Toggle toggle, bool state)
    {
        toggle.onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        toggle.isOn = state;
        toggle.onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
    }
    public void OnClickBack()
    {
        PanelManager panelManager = FindObjectOfType<PanelManager>();
        panelManager.showHome();
    }
    public void onMusicChange(bool isOn)
    {
        DataManager.Instance.gameInfo.player.musicVisible = !DataManager.Instance.gameInfo.player.musicVisible;
        Debug.Log("music" + DataManager.Instance.gameInfo.player.musicVisible);

        DataManager.Instance.SaveData();
        if (DataManager.Instance.gameInfo.player.musicVisible)
        {
            AudioManager.Instance.StartPlaylist($"music{Random.Range(1, 6)}");
        }
        else
        {
            AudioManager.Instance.StopMusic();
        }
    }
    public void onSoundChange(bool isOn)
    {
        DataManager.Instance.gameInfo.player.soundVisible = !DataManager.Instance.gameInfo.player.musicVisible;
        DataManager.Instance.SaveData();
        Debug.Log("Sound: " + DataManager.Instance.gameInfo.player.musicVisible);
    }
    public void onVibratioChangen(bool isOn)
    {
        DataManager.Instance.gameInfo.player.vibrationVisible = !DataManager.Instance.gameInfo.player.musicVisible;
        DataManager.Instance.SaveData();
        Debug.Log("Vibration: " + DataManager.Instance.gameInfo.player.musicVisible);

    }
}
