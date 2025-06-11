using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelBodyArtCondition : MonoBehaviour
{
    public GameObject SkinContent;
    public GameObject SkinItemPrefab;
    public GameObject SkinViewPrefab;
    public GameObject ButtonListPrefab;
    public Text TipsText;
    public Text coinNum;

    private PanelManager panelManager;
    private SkinItem ViewSkinItem = null;
    private List<SkinItem> SkinData;
    private string artSSkinPath = "VFX/";
    private int lastPickId = 0;

    // Start is called before the first frame update
    void Start()
    {
        panelManager = FindObjectOfType<PanelManager>();

    }
    void OnEnable()
    {
        initNode();
    }
    // 初始化节点
    void initNode()
    {
        // 获取皮肤数据
        SkinData = DataManager.Instance.gameInfo.collections.bodyParts;
        Debug.Log("皮肤数量：" + SkinData.Count);
        //数据排序 解锁的皮肤排在前面然后在根据sortNum大小排序
        SkinData.Sort((a, b) =>
        {
            // 优先比较解锁状态（已解锁排在前面）
            int lockCompare = a.isLocked.CompareTo(b.isLocked);

            // 如果解锁状态不同
            if (lockCompare != 0)
            {
                // 反转比较结果（因为true的CompareTo false会返回1，我们需要-1）
                return -lockCompare;
            }

            // 解锁状态相同时比较sortNum
            return a.sortNum.CompareTo(b.sortNum);
        });

        // 如果皮肤数据为空或未初始化，则输出警告信息并返回
        if (SkinData == null || SkinData.Count == 0)
        {
            Debug.LogWarning("SkinData 为空或未初始化");
            return;
        }

        // 遍历皮肤数据
        foreach (SkinItem item in SkinData)
        {
            if (item.id == DataManager.Instance.gameInfo.player.artId)
            {
                ViewSkinItem = item;
            }
            // 实例化皮肤项
            GameObject skinItem = Instantiate(SkinItemPrefab, SkinContent.transform);
            skinItem.SetActive(true);
            skinItem.name = item.id.ToString();
            skinItem.transform.Find("LockImage").gameObject.SetActive(!item.isLocked && item.unlockCondition != 0);
            skinItem.transform.Find("PickImage").gameObject.SetActive(false);

            skinItem.GetComponent<Button>().onClick.AddListener(() => { OnPrivewClick(item.name); });
            // 添加点击事件监听器
            SetSkin(item.id, skinItem.transform.Find("Art").gameObject);
        }
        ;
        //设置默认预览皮肤数据
        if (DataManager.Instance.gameInfo.player.artId == 0)
        {
            ViewSkinItem = SkinData[0];
        }
        // 设置玩家皮肤
        if (ViewSkinItem != null)
        {
            OnPrivewClick(ViewSkinItem.name);
        }
        ChangeButtonInfo();
        UpDateCoin();
        SetPickBorder();

    }
    void UpDateCoin()
    {
        coinNum.text = DataManager.Instance.gameInfo.player.coin.ToString();
    }
    void SetSkin(int id, GameObject Target)
    {
        SkinItem skinInfo = SkinData.Find(x =>
        {
            return x.id == id;
        });
        if (skinInfo != null)
        {
            // 加载预制体
            GameObject skinPrefab = Resources.Load<GameObject>($"{artSSkinPath}{skinInfo.name}");
            if (skinPrefab != null)
            {
                // 实例化预制体
                GameObject instantiatedSkin = Instantiate(skinPrefab, Target.transform); // parentTransform 是您希望将预制体放置的父物体
                                                                                         // 设置层级为 Shop
                SetLayerRecursively(instantiatedSkin, LayerMask.NameToLayer("Shop"));
                // 进行其他操作，例如设置位置、旋转等
                instantiatedSkin.transform.localPosition = Vector3.zero; // 根据需要设置位置
                if (SkinViewPrefab == Target)
                {
                    instantiatedSkin.transform.localScale = instantiatedSkin.transform.localScale * 111;
                }
                else
                {
                    instantiatedSkin.transform.localScale = instantiatedSkin.transform.localScale * 56;
                }
            }
            else
            {
                Debug.LogWarning($"未找到预制体: {artSSkinPath}{skinInfo.name}");
            }
        }
        else
        {
            Debug.Log("没有找到皮肤");
        }
    }
    // 递归设置层级的方法
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer; // 设置当前对象的层级

        // 遍历所有子对象并递归设置层级
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    // 点击预览按钮时调用
    public void OnPrivewClick(string skinName)
    {
        Debug.Log("点击预览按钮");
        // 根据皮肤名称查找皮肤信息
        SkinItem skinInfo = SkinData.Find(x =>
       {
           return x.name == skinName;
       });
        ViewSkinItem = skinInfo; // 保存当前预览的皮肤信息
                                 // 设置皮肤
                                 // 清空 SkinViewPrefab 的子物体
        foreach (Transform child in SkinViewPrefab.transform)
        {
            GameObject.Destroy(child.gameObject); // 删除子物体
        }
        SetSkin(skinInfo.id, SkinViewPrefab);
        ChangeButtonInfo();
        SetPickBorder();
    }
    void SetPickBorder()
    {
        SkinContent.transform.Find(ViewSkinItem.id.ToString()).Find("PickImage").gameObject.SetActive(true);
        if (SkinContent.transform.Find(lastPickId.ToString()) != null)
        {
            SkinContent.transform.Find(lastPickId.ToString()).Find("PickImage").gameObject.SetActive(false);

        }
        lastPickId = ViewSkinItem.id;
    }
    void ChangeButtonInfo()
    {
        Transform ButtonUnlockAD = ButtonListPrefab.transform.Find("ButtonUnlockAD");
        Transform ButtonUnlockCoin = ButtonListPrefab.transform.Find("ButtonUnlockCoin");
        Transform useButton = ButtonListPrefab.transform.Find("ButtonUse");
        if (ButtonUnlockAD == null || useButton == null || ButtonUnlockCoin == null)
        {
            Debug.Log("没有找到按钮");
            return;
        }
        ButtonUnlockAD.gameObject.SetActive(false);
        ButtonUnlockCoin.gameObject.SetActive(false);
        useButton.gameObject.SetActive(false);
        if (ViewSkinItem.unlockCondition == 0 || ViewSkinItem.isLocked == true)
        {
            useButton.gameObject.SetActive(true);
            if (DataManager.Instance.gameInfo.player.artId == ViewSkinItem.id)
            {
                useButton.GetComponentInChildren<Text>().text = "已配置";
                useButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                useButton.GetComponentInChildren<Text>().text = "使用";
                useButton.GetComponent<Button>().interactable = true;
            }
        }
        if (!ViewSkinItem.isLocked && ViewSkinItem.unlockCondition != 0)
        {
            switch (ViewSkinItem.unlockCondition)
            {

                case 1:
                    ButtonUnlockCoin.gameObject.SetActive(true);
                    ButtonUnlockCoin.GetComponentInChildren<Text>().text = ViewSkinItem.unlockValue.ToString();
                    break;
                case 2:
                    ButtonUnlockAD.gameObject.SetActive(true);
                    ButtonUnlockAD.GetComponentInChildren<Text>().text = "累计" + ViewSkinItem.unlockValue + "次广告解锁";
                    break;
                default:
                    break;
            }
        }

    }
    public void OnBuyClick()
    {
        if (ViewSkinItem.unlockCondition == 1)
        {
            if (DataManager.Instance.gameInfo.player.coin >= ViewSkinItem.unlockValue)
            {
                PlayerInfo.Instance.SaveCoin(-ViewSkinItem.unlockValue, () =>
                {
                    panelManager.ShowTips("解锁成功");
                    unlockSkin();
                });
            }
            else
            {
                panelManager.ShowTips("金币不足");
            }
            Debug.Log("购买");
        }
        else if (ViewSkinItem.unlockCondition == 2)
        {
            //观看广告
            ADManager.Instance.OnADShow(() =>
            {
                if (DataManager.Instance.gameInfo.player.adWatchTime >= ViewSkinItem.unlockValue)
                {
                    unlockSkin();
                }
                else
                {
                    panelManager.ShowTips("广告次数不够");
                }
            });
            Debug.Log("观看广告");
        }
    }
    public void OnUseClick()
    {
        PlayerInfo.Instance.rSetDefaultBodyArt(ViewSkinItem.id, () =>
        {

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<player>().InitDefaultArt();
        });
    }
    public void OnClickBack()
    {
        gameObject.SetActive(false);
        panelManager.showHome();
    }
    void unlockSkin()
    {
        CollectionInfo.Instance.rUnlockBodyArt(ViewSkinItem.id, () =>
        {
            PlayerInfo.Instance.rSetDefaultBodyArt(ViewSkinItem.id, () =>
        {
            Debug.Log("使用成功");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<player>().InitDefaultArt();
        });
            SkinContent.transform.Find(ViewSkinItem.id.ToString()).Find("LockImage").gameObject.SetActive(false);
            ChangeButtonInfo();
            Debug.Log("解锁成功");
        });
    }
}

