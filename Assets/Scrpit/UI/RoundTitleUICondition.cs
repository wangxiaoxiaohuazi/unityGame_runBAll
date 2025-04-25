using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RoundTitleUICondition : MonoBehaviour
{
    // Start is called before the first frame update
    private List<float> widthList = new List<float> { 90f, 130f, 260f, 330f, 600f };
    private List<List<LevelList>> slicedLevelSceneList; // 声明slicedLevelSceneList

    void Start()
    {
        InitNode();
    }
    
    // Update is called once per frame
    void Update() { }

    void InitNode()
    {
        int currenRound = RoundInfo.Instance.OnGetCurrentLevel() != null ? RoundInfo.Instance.OnGetCurrentLevel().id : 0;

        // 取5的余数
        int index = currenRound % 5;
        // float width = widthList[index]; // 访问widthList
        float width = 90f;
        // Debug.Log("widthList[index - 1] = " + widthList[index]);
        //修改子物体ProgressImage 的宽度
        transform.Find("ProgressImage").GetComponent<RectTransform>().sizeDelta = new Vector2(
            width,
            30f
        );
        for (int i = 0; i < 5; i++)
        {
            bool _flag = currenRound > i;
            transform.Find("item" + i).Find("DefaultImage").gameObject.SetActive(!_flag);
            transform.Find("item" + i).Find("PickImage").gameObject.SetActive(_flag);
            transform.Find("item" + i).Find("Boss").gameObject.SetActive((currenRound + i) % 5 == 0);
            transform.Find("item" + i).Find("LevelNum").GetComponent<Text>().text = (
                currenRound + i
            ).ToString();
        }
    }


}
