using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TTSDK;
public class player : MonoBehaviour
{
    private Rigidbody rb;
    public float forceMagnitude = 10f; // 调整这个值来改变击飞的力度

    public float blood = 4; //血量
    public bool isInvincible = false; //无敌状态
    public float power = 0; //能量
    public int levelNumber = 0; //等级
    public float scoreNumber = 0; //分数
    public int attack = 1; //攻击力
    [Header("默认角色")]
    public GameObject _defaultPlayer;
    public GameObject _defaultTrailing;
    private int priveAttactNodeID;

    public float defaultBlood = 4;
    void Start()
    {
        //初始化基础赋值
        rb = GetComponent<Rigidbody>();
        // vibrationManager = gameObject.AddComponent<VibrationManager>();
        InitDefaultPlayer(); //初始化默认角色
        InitDefaultTraling(); //初始化默认拖尾
        InitDefaultArt(); //初始化默认艺术
        Debug.Log("当前总获得金币：" + DataManager.Instance.gameInfo.player.coin);
        GameDataManager.Instance.isPause = false;
        defaultBlood = blood;
        GameDataManager.Instance.ChangePlayerLives(blood);//初始化血量
        GameDataManager.Instance.goldenCoin = 0; //重置金币
    }

    void Update()
    {


    }


    //初始化玩家皮肤
    public void InitDefaultPlayer()
    {

        // 获取body对象
        Transform bodyComponent = gameObject.transform.Find("Body");
        if (bodyComponent != null)
        {
            //移除所有子对象
            foreach (Transform child in bodyComponent)
            {
                Destroy(child.gameObject);
            }
            // 添加默认皮肤
            GameObject NewPlayer = Instantiate(
                _defaultPlayer,
                bodyComponent.transform.position,
                _defaultPlayer.transform.rotation,
                bodyComponent
            );
            List<SkinItem> SkinData = DataManager.Instance.gameInfo.collections.skins;
            // 如果皮肤数据为空或未初始化，则输出警告信息并返回
            if (SkinData == null || SkinData.Count == 0)
            {
                Debug.LogWarning("SkinData 为空或未初始化");
                return;
            }
            SkinItem ViewSkinItem = null;
            // 遍历皮肤数据
            SkinData.ForEach((SkinItem item) =>
            {
                //设置默认预览皮肤数据
                if (item.id == DataManager.Instance.gameInfo.player.skinId)
                {
                    ViewSkinItem = item;
                }
            });
            if (ViewSkinItem != null)
            {
                Texture2D newTexture = Resources.Load<Texture2D>($"Materials/PlayerSkin/{ViewSkinItem.name}");
                if (newTexture != null)
                {
                    // 如果需要将图片应用到材质上，可以创建一个新的材质并设置其主纹理
                    Material newMat = new Material(Shader.Find("Standard")); // 使用标准着色器
                    newMat.mainTexture = newTexture;

                    Renderer renderer = NewPlayer.GetComponent<Renderer>();
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
            }
        }
        setLevelNode(0);

    }
    public void InitDefaultArt()
    {
        // 获取body对象
        Transform bodyComponent = gameObject.transform.Find("BodyArt");
        if (bodyComponent != null)
        {
            //移除所有子对象
            foreach (Transform child in bodyComponent)
            {
                Destroy(child.gameObject);
            }
            List<SkinItem> SkinData = DataManager.Instance.gameInfo.collections.bodyParts;
            // 如果皮肤数据为空或未初始化，则输出警告信息并返回
            if (SkinData == null || SkinData.Count == 0)
            {
                Debug.LogWarning("SkinData 为空或未初始化");
                return;
            }
            SkinItem ViewSkinItem = null;
            // 遍历皮肤数据
            SkinData.ForEach((SkinItem item) =>
            {
                //设置默认预览皮肤数据
                if (item.id == DataManager.Instance.gameInfo.player.artId)
                {
                    ViewSkinItem = item;
                }
            });
            if (ViewSkinItem != null)
            {
                // 加载预制体
                GameObject skinPrefab = Resources.Load<GameObject>($"VFX/{ViewSkinItem.name}");
                if (skinPrefab != null)
                {
                    // 实例化预制体
                    GameObject instantiatedSkin = Instantiate(skinPrefab, bodyComponent.transform); // parentTransform 是您希望将预制体放置的父物体
                }
            }
        }
    }
    public void setLevelNode(int num, Collider _Collider = null)
    {
        //发生碰撞增加等级
        if (
            num != 0
            && _Collider == null
            && priveAttactNodeID == _Collider.gameObject.GetInstanceID()
        )
        {
            return;
        }
        //获取gameObject 下面的TextMeshPro 物体。并设置文本
        levelNumber += num;
        Transform levelNumberTransform = transform.Find("levelNumber"); // 在当前物体下查找名为 levelNumber 的物体
        if (levelNumberTransform != null)
        {
            GameObject gameObject = levelNumberTransform.gameObject; // 获取 GameObject
            gameObject.GetComponent<TextMeshPro>().text = "等级:" + levelNumber;
        }
        else
        {
            Debug.LogError("未找到名为 levelNumber 的物体");
        }
    }

    public void InitDefaultTraling()
    {
        // 获取当前player对象
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;
        // 获取Trailing对象
        Transform trailingComponent = player.transform.Find("Trailing");
        if (trailingComponent != null)
        {
            foreach (Transform child in trailingComponent)
            {
                Destroy(child.gameObject);
            }
            // 添加默认拖尾
            Instantiate(
                _defaultTrailing,
                trailingComponent.transform.position,
                _defaultTrailing.transform.rotation,
                trailingComponent
            );
        }
    }

    void OnTriggerEnter(Collider other) { }

    void OnCollisionEnter(Collision collision)
    {
        // 检查碰撞的对象是否有刚体组件
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
        string[] tagsToIgnore = { "Wall", "StopWall" };
        if (System.Array.Exists(tagsToIgnore, tag => collision.gameObject.CompareTag(tag)))
            return;
        if (rb != null)
        {
            // 启用重力并取消 Kinematic
            rb.isKinematic = false;
            rb.useGravity = true;
            // Debug.Log("Player:" + collision.gameObject.name);
            // 计算击飞方向
            Vector3 forceDirection = collision.transform.position - transform.position;
            forceDirection.y = 1; // 确保力有一个向上的分量
            forceDirection.Normalize();

            // 应用力
            // 力的大小可以根据需要调整
            rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);

        }

    }

    public void Quit()
    {
        Application.Quit();
    }
}
