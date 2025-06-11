using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelUIInit : MonoBehaviour
{
    public GameObject PanelUI;
    // Start is called before the first frame update
    void Start()
    {
        // 检查 PanelUI 是否为 null
        if (PanelUI == null)
        {
            Debug.LogError("PanelUI is not assigned!");
            return; // 如果为 null，提前返回
        }
        //将PanelUI设置为兄弟节点
        // 实例化 PanelUI，并将其设置为当前物体的同级物体
        GameObject instantiatedPanel = Instantiate(PanelUI, transform.parent);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
