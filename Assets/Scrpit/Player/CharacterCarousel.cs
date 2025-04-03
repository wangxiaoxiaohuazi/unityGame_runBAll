using System.Collections;
using UnityEngine;

public class CharacterCarousel : MonoBehaviour
{
    [Header("角色设置")]
    public GameObject[] characterPrefabs;  // 角色预制体数组
    public float spacing = 2f;             // 角色间距
    public float switchDuration = 0.5f;    // 切换动画时长

    private GameObject[] characterInstances; // 已实例化的角色数组
    private int currentIndex = 0;          // 当前显示的角色索引
    private bool isSwitching = false;      // 切换状态锁

    void Start()
    {
        InitializeCharacters();
    }

    // 初始化所有角色实例
    void InitializeCharacters()
    {
        characterInstances = new GameObject[characterPrefabs.Length];

        for (int i = 0; i < characterPrefabs.Length; i++)
        {
            // 实例化角色并设置初始位置
            characterInstances[i] = Instantiate(
                characterPrefabs[i],
                new Vector3(i * spacing, 0, 0),
                Quaternion.identity,
                transform
            );

            // 默认隐藏非当前角色
            characterInstances[i].SetActive(i == currentIndex);
        }
    }

    public void ShowPrevious()
    {
        if (isSwitching) return;
        int newIndex = (currentIndex - 1 + characterPrefabs.Length) % characterPrefabs.Length;
        StartCoroutine(SwitchCharacters(newIndex, Vector3.right));
    }

    public void ShowNext()
    {
        if (isSwitching) return;
        int newIndex = (currentIndex + 1) % characterPrefabs.Length;
        StartCoroutine(SwitchCharacters(newIndex, Vector3.left));
    }

    IEnumerator SwitchCharacters(int targetIndex, Vector3 direction)
    {
        isSwitching = true;

        // 显示新角色并设置初始位置
        GameObject incomingChar = characterInstances[targetIndex];
        incomingChar.transform.localPosition = direction * spacing;
        incomingChar.SetActive(true);

        Vector3 startPosCurrent = characterInstances[currentIndex].transform.localPosition;
        Vector3 endPosCurrent = -direction * spacing;

        Vector3 startPosNew = incomingChar.transform.localPosition;
        Vector3 endPosNew = Vector3.zero;

        float elapsed = 0f;

        while (elapsed < switchDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / switchDuration);

            // 当前角色移出
            characterInstances[currentIndex].transform.localPosition =
                Vector3.Lerp(startPosCurrent, endPosCurrent, t);

            // 新角色移入
            incomingChar.transform.localPosition =
                Vector3.Lerp(startPosNew, endPosNew, t);

            yield return null;
        }

        // 隐藏旧角色
        characterInstances[currentIndex].SetActive(false);
        currentIndex = targetIndex;
        isSwitching = false;
    }
}
