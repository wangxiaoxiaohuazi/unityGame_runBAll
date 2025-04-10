using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BloodUICondition : MonoBehaviour
{
    public GameObject Target;//目标
    public GameObject ListNode;//UI
    private float HPNum = 0;
    private List<GameObject> BloodList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        initNode();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void initNode()
    {
        HPNum = Target.GetComponent<EnemyStateCondition>().hp;
        BloodList = ListNode.GetComponentsInChildren<Image>(true)
           .Where(img => img.transform != ListNode.transform)
           .Select(img => img.gameObject)
           .ToList();
        //创建血条
        int num = BloodList.Count();
        Debug.Log("血量");
        Debug.Log(num);
        Debug.Log(HPNum);
        Debug.Log("结束");
        if (HPNum - num > 0)
        {
            for (int i = 0; i < HPNum - num; i++)
            {
                Instantiate(BloodList[0].gameObject, transform.position, transform.rotation, ListNode.transform);
            }
        }
        else if (HPNum - num < 0)
        {
            for (int i = 0; i < num - HPNum; i++)
            {
                Destroy(BloodList[0].gameObject);
            }
        }
    }
    public void ChangeBloodUI()
    {
        // 确保获取最新列表
        BloodList = ListNode.GetComponentsInChildren<Image>(true)
            .Where(img => img.transform != ListNode.transform)
            .Select(img => img.gameObject)
            .ToList();

        int num = BloodList.Count;
        HPNum = Target.GetComponent<EnemyStateCondition>().hp;

        int needUpdate = Mathf.Clamp(num - (int)HPNum, 0, num);
        
        for (int i = 0; i < needUpdate; i++)
        {
            int safeIndex = Mathf.Clamp(num - i - 1, 0, num - 1);
            if(safeIndex < BloodList.Count && BloodList[safeIndex] != null)
            {
                BloodList[safeIndex].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
        }
    }
}
