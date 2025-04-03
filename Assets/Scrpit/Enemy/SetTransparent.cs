using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//空气墙
public class SetTransparent : MonoBehaviour
{
    public float transparency = 0.5f; // 透明度值，范围从0到1，0表示完全透明，1表示完全不透明

    void Start()
    {

        // 获取 Renderer 组件
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            // 获取材质
            Material material = renderer.material;

            // 确保材质使用的是支持透明度的着色器
            material.shader = Shader.Find("Standard");
            material.SetFloat("_Mode", 3); // 设置为透明模式
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;

            // 设置颜色和透明度
            Color color = material.color;
            color.a = transparency;
            material.color = color;
        }
        else
        {
            Debug.LogError("Renderer component not found on the plane object.");
        }
    }


  
}