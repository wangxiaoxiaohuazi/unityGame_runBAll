using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicItem : MonoBehaviour
{
    [Header("BodyChange")]
    public bool IsBodyChange = false;//是否改变身体
    public float Size = 1.5f;//增加的大小
    public float Time = 2;//持续时间
    [Header("GoldenChange")]
    public bool IsGoldenChange = false;//是否改变金币
    public int GoldenNum = 1;//金币数量
    [Header("HPChange")]
    public bool IsHPChange = false;//是否改变血量
    public int HP = 1;//血量
    [Header("levelNumberChange")]
    public bool IsLevelNumberChange = false;//是否改变关卡数
    public int LevelNumber = 1;//关卡数
    [Header("PowerChange")]
    public bool IsPowerChange = false;//是否改变能量
    public float Power = 0.01f;
    [Header("scoreNumberChange")]
    public bool IsScoreNumberChange = false;//是否改变分数
    public int ScoreNumber = 1;//分数
    [Header("公共设置")]
    public bool IsDestroy = true;//是否销毁
    public bool IsExplosion = true;//是否爆炸
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (IsBodyChange)
            {
                BodyChange bodyChange = new BodyChange();
                bodyChange.ChangeStart(other, Size, Time, IsDestroy);
            }
            if (IsGoldenChange)
            {
                GoldenChange goldenChange = new GoldenChange();
                goldenChange.ChangeStart(other, GoldenNum, 0, IsDestroy);

            }
            if (IsHPChange)
            {
                HPChange hpChange = new HPChange();
                hpChange.ChangeStart(other, HP, 0, IsDestroy);
            }

            if (IsLevelNumberChange)
            {
                levelNumberChange levelNumberChange = new levelNumberChange();
                levelNumberChange.ChangeStart(other, LevelNumber, 0, IsDestroy);
            }
            if (IsPowerChange)
            {
                PowerChange powerChange = new PowerChange();
                powerChange.ChangeStart(other, Power, 0, IsDestroy);
            }
            if (IsScoreNumberChange)
            {
                scoreNumberChange scoreNumberChange = new scoreNumberChange();
                scoreNumberChange.ChangeStart(other, ScoreNumber, 0, IsDestroy);
            }
            if (gameObject != null && IsDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
}
