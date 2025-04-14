using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ADManager : MonoBehaviour
{
    private static ADManager _instance;
    public static ADManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ADManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("ADManager");
                    _instance = obj.AddComponent<ADManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public PublicGameData gameData => DataManager.Instance.gameInfo;

    //下一关
    public void OnADShow(Action callback = null)
    {
        try
        {
            //广告回调
            callback?.Invoke();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}