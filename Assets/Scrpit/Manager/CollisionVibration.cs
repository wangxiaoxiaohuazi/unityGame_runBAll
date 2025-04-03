// 震动控制脚本（挂载到碰撞物体）
using UnityEngine;

public class CollisionVibration : MonoBehaviour
{
    [Header("震动设置")]
    [Range(0, 1)] public float vibrationIntensity = 0.7f; // 震动强度
    public float vibrationDuration = 0.3f; // 持续时间

#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject vibrationService;
    
    void Start() {
        AndroidJavaClass contextClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = contextClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        vibrationService = vibrator;
    }
#endif

    public void TriggerVibration()
    {
        if (SystemInfo.supportsVibration)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // 使用Android原生API实现精确震动
            long milliseconds = (long)(vibrationDuration * 1000);
            vibrationService.Call("vibrate", milliseconds);
#elif UNITY_IOS
            // iOS使用预设震动模式
            // Handheld.Vibrate();
#else
            // 通用震动方式
            // Handheld.Vibrate();
#endif
        }
    }
}
