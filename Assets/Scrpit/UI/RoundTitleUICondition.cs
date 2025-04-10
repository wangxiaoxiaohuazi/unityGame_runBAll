using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundTitleUICondition : MonoBehaviour
{
    // Start is called before the first frame update
    private List<float> widthList = new List<float> { 90f, 130f, 260f, 330f, 420f };

    void Start()
    {
        InitNode();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void InitNode()
    {
        int currenRound = DataManager.Instance.gameInfo.roundInfo.currentLevel;
        //取5的余数
        currenRound = currenRound % 5;
        Debug.Log("currenRound:" + currenRound);
        if (currenRound > 0)
        {
            float width = widthList[currenRound - 1];
            //修改子物体ProgressImage 的宽度
            transform.Find("ProgressImage").GetComponent<RectTransform>().sizeDelta = new Vector2(width, 30f);
        }
        if (currenRound == 0)
        {
            transform.Find("ProgressImage").GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 30f);
        }
        for (int i = 0; i < 4; i++)
        {
            bool _flag = currenRound > i;
            transform.Find("item" + i).Find("DefaultImage").gameObject.SetActive(!_flag);
            transform.Find("item" + i).Find("PickImage").gameObject.SetActive(_flag);
            transform.Find("item" + i).Find("LevelNum").GetComponent<Text>().text = (currenRound + i).ToString();
        }
    }
}
