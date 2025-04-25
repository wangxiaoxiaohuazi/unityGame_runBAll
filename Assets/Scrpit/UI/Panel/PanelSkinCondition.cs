using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelSkinCondition : MonoBehaviour
{
    public GameObject SkinContent;
    public GameObject SkinItemPrefab;
    public GameObject SkinViewPrefab;
    public GameObject ButtonListPrefab;
    public Text TipsText;
    public Text coinNum;
    private PanelManager panelManager;
    private SkinItem ViewSkinItem;
    private List<SkinItem> SkinData;
    private string skinImagePath = "Materials/PlayerSkin/";
    private int lastPickId = 0;
    // Start is called before the first frame update
    void Start()
    {
        panelManager = FindObjectOfType<PanelManager>();
        initNode();
    }
    void OnEnable()
    {
        initNode();
    }
    // 初始化节点
    void initNode()
    {
        // 获取皮肤数据
        SkinData = DataManager.Instance.gameInfo.collections.skins;
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
        SkinData.ForEach((SkinItem item) =>
        {
            //设置默认预览皮肤数据
            if (item.id == DataManager.Instance.gameInfo.player.skinId)
            {
                ViewSkinItem = item;
            }
            // 实例化皮肤项
            GameObject skinItem = Instantiate(SkinItemPrefab, SkinContent.transform);
            skinItem.SetActive(true);
            skinItem.transform.Find("LockImage").gameObject.SetActive(!item.isLocked && item.unlockCondition != 0);
            skinItem.transform.Find("PickImage").gameObject.SetActive(false);
            // 设置皮肤项的皮肤
            SetSkin(item.id, skinItem.transform.Find("Player").gameObject);
            skinItem.name = item.id.ToString();
            // 添加点击事件监听器
            skinItem.GetComponent<Button>().onClick.AddListener(() => { OnPrivewClick(item.name); });
        });
        // 设置玩家皮肤
        SetSkin(ViewSkinItem.id, SkinViewPrefab);
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
            // 加载图片
            Texture2D newTexture = Resources.Load<Texture2D>($"{skinImagePath}{skinInfo.name}");
            if (newTexture != null)
            {
                // 如果需要将图片应用到材质上，可以创建一个新的材质并设置其主纹理
                Material newMat = new Material(Shader.Find("Standard")); // 使用标准着色器
                newMat.mainTexture = newTexture;

                Renderer renderer = Target.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // 创建材质实例避免修改原始材质
                    renderer.material = newMat;
                }
                else
                {
                    Debug.Log("没有找到材质");
                }
            }
            else
            {
                Debug.LogWarning($"未找到图片: Materials/PlayerSkin/{skinInfo.name}");
            }
        }
        else
        {
            Debug.Log("没有找到皮肤");
        }
    }
    // 点击预览按钮时调用
    public void OnPrivewClick(string skinName)
    {
        // 根据皮肤名称查找皮肤信息
        SkinItem skinInfo = SkinData.Find(x =>
       {
           return x.name == skinName;
       });
        ViewSkinItem = skinInfo; // 保存当前预览的皮肤信息
        // 设置皮肤
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
                    panelManager.ShowTips("解锁成功");
                    unlockSkin();

                }
                else
                {
                    panelManager.ShowTips("广告次数不足");

                }
            });
            Debug.Log("观看广告");
        }
    }
    public void OnUseClick()
    {
        PlayerInfo.Instance.rSetDefaultSkin(ViewSkinItem.id, () =>
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<player>().InitDefaultPlayer();
        });
    }
    public void OnClickBack()
    {
        gameObject.SetActive(false);
        panelManager.showHome();
    }
    void unlockSkin()
    {
        CollectionInfo.Instance.rUnlockSkin(ViewSkinItem.id, () =>
        {
            PlayerInfo.Instance.rSetDefaultSkin(ViewSkinItem.id, () =>
         {
             GameObject player = GameObject.FindGameObjectWithTag("Player");
             player.GetComponent<player>().InitDefaultPlayer();
         });
            SkinContent.transform.Find(ViewSkinItem.id.ToString()).Find("LockImage").gameObject.SetActive(false);
            ChangeButtonInfo();
            Debug.Log("解锁成功");
        });
    }
}
