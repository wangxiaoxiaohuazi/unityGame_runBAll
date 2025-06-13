using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundProportionUICondition : MonoBehaviour
{
    private float DefaultDistance = 0; //默认距离
    public TextMeshProUGUI ProportionText;
    public TextMeshProUGUI LevelText;
    public float RoundProportion = 0; //比例

    // Start is called before the first frame update
    void Start()
    {
        Transform player = GameObject.FindWithTag("Player").transform;
        Transform EndNode = GameObject.FindWithTag("EndGame").transform;
        if (player != null && EndNode != null)
        {
            DefaultDistance = Vector3.Distance(player.position, EndNode.position);
        }
        ;
        if (RoundInfo.Instance.OnGetCurrentLevel() != null)
        {
            LevelText.text = "第" + RoundInfo.Instance.OnGetCurrentLevel().id + "关";
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (DefaultDistance != 0 && RoundProportion != 100)
        {
            Transform player = GameObject.FindWithTag("Player").transform;
            Transform EndNode = GameObject.FindWithTag("EndGame").transform;
            if (player != null && EndNode != null)
            {
                float distance = Vector3.Distance(player.position, EndNode.position);
                float proportion = 1 - distance / DefaultDistance;
                float ProprotionWidth = Mathf.Round(600 * proportion * 10) / 10f;
                GameObject.Find("ProgressImage").GetComponent<RectTransform>().sizeDelta =
                    new Vector2(ProprotionWidth, 45f);
                RoundProportion = Mathf.Round(100 * proportion);
                ProportionText.text = RoundProportion + "%";
            }
        }
    }
}
