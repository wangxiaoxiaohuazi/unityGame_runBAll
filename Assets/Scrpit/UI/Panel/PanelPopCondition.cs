using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelPopCondition : MonoBehaviour
{
    private PanelManager panelManager;
    public Text TitleText;
    public Text ContentText;
    public Button ConfirmButton;
    public Button CancelButton;

    void Start()
    {
         panelManager = FindObjectOfType<PanelManager>();
    }
    void Update()
    {

    }
    public void PopShow(string title, string content, Action confirmAction, Action cancelAction)
    {
        Debug.Log("PopShow===="+confirmAction);
        if(title != null){
            TitleText.text = title;
        }
        if (content != null)
        {
            ContentText.text = content;
        }
        if (confirmAction != null)
        {
            ConfirmButton.onClick.AddListener(() => confirmAction());
        }
        if (cancelAction != null)
        {
            CancelButton.onClick.AddListener(() =>{cancelAction(); OnClickBack(); });
        }else{
              CancelButton.onClick.AddListener(() =>{ OnClickBack();  });
        }
    }

    public void OnClickBack()
    {
       panelManager.HidePanel(PanelManager.PanelName.PanelPop);
    }
}
