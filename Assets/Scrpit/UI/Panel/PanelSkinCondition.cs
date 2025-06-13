using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
    private string skinImagePath = "Assets/ResourcesMove/Materials/PlayerSkin/";
    private int lastPickId = 0;
    //资源类型不统一导致
    private static readonly HashSet<string> jpgSkinNames = new HashSet<string> {
                "b-skin7",
                "Y-ball-05",
                "Y-ball-06",
                "Y-ball-07",
                "Y-ball-16",
                "Y-ball-19",
                "Y-ball-21",
                "Y-ball-22"
            };
    // 添加纹理缓存字典
    private Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

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
            if (item.id != 401)
            {


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
            }
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
        SkinItem skinInfo = SkinData.Find(x => x.id == id);
        if (skinInfo != null)
        {


            string extension = jpgSkinNames.Contains(skinInfo.name) ? ".jpg" : ".png";
            string textureKey = $"{skinImagePath}{skinInfo.name}{extension}";

            // 检查缓存
            if (textureCache.TryGetValue(textureKey, out Texture2D cachedTexture))
            {
                ApplyTextureToMaterial(cachedTexture, Target);
                return;
            }

            // 使用 Addressable 加载
            Addressables.LoadAssetAsync<Texture2D>(textureKey).Completed += (AsyncOperationHandle<Texture2D> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Texture2D newTexture = handle.Result;
                    textureCache[textureKey] = newTexture;
                    ApplyTextureToMaterial(newTexture, Target);
                }
                else
                {
                    Debug.LogWarning($"加载图片失败: {textureKey}");
                }
            };
        }
        else
        {
            Debug.Log("没有找到皮肤");
        }
    }

    private void ApplyTextureToMaterial(Texture2D texture, GameObject target)
    {
        Material newMat = new Material(Shader.Find("Standard"));
        newMat.mainTexture = texture;

        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = newMat;
        }
        else
        {
            Debug.Log("没有找到材质");
        }
    }

    private void OnDestroy()
    {
        // 清理缓存的纹理
        foreach (var texture in textureCache.Values)
        {
            if (texture != null)
            {
                Addressables.Release(texture);
            }
        }
        textureCache.Clear();
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
            if (DataManager.Instance.gameInfo.player.skinId == ViewSkinItem.id)
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

