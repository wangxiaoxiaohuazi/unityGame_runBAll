
using System.Collections;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    private Material _instanceMaterial;
    public void SetColor(float duration = 0f)
    {
        StartCoroutine(ChangeColorOverTime(duration));
    }
    public IEnumerator ChangeColorOverTime(float duration)
    {
        Color targetColor = GameDataManager.Instance.BaseColor;
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Color initialColor = renderer.material.color; // 获取初始颜色
            float elapsedTime = 0f;

            while (elapsedTime <= duration)
            {
                if (renderer == null)
                {
                    break;
                }

                // 计算当前的颜色
                _instanceMaterial = Instantiate(renderer.sharedMaterial);
                _instanceMaterial.color = targetColor;
                renderer.sharedMaterial = _instanceMaterial;
                elapsedTime += Time.deltaTime; // 增加经过的时间
                yield return null; // 等待下一帧
            }
            // 确保最终颜色设置为目标颜色
        }
    }
    void OnDestroy()
    {
        // 清理动态材质（重要！）
        if (_instanceMaterial != null)
            Destroy(_instanceMaterial);
    }
}
