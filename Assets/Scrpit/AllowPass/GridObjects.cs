using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridObjects : MonoBehaviour
{
    public Vector2 spacing;
    public enum Constraint { FixedColumnCount, FixedRowCount };
    public Constraint constraint;
    public int constraintCount = 2;

    public bool adjustLocalScale = true;
    public Vector3 localScale = Vector3.one;

    void OnValidate()
    {
        constraintCount = Mathf.Max(constraintCount, 1);
        localScale = new Vector3(Mathf.Max(0, localScale.x), Mathf.Max(0, localScale.y), Mathf.Max(0, localScale.z));
        spacing = new Vector2(Mathf.Max(0, spacing.x), Mathf.Max(0, spacing.y));
    }

    void Update()
    {
        if (Application.isPlaying) return;

        int i = 0;
        float minX = int.MaxValue, minZ = int.MaxValue, maxX = int.MinValue, maxZ = int.MinValue;
        foreach(Transform item in transform)
        {
            int x, y;
            if (constraint == Constraint.FixedColumnCount)
            {
                x = i % constraintCount;
                y = i / constraintCount;
            }
            else
            {
                x = i / constraintCount;
                y = i % constraintCount;
            }

            item.localPosition = new Vector3(x * spacing.x, 0, spacing.y * y);
            minX = Mathf.Min(minX, x * spacing.x);
            minZ = Mathf.Min(minZ, y * spacing.y);
            maxX = Mathf.Max(maxX, x * spacing.x);
            maxZ = Mathf.Max(maxZ, y * spacing.y);

            if (adjustLocalScale)
            {
                item.localScale = localScale;
            }
            i++;
        }

        Vector3 center = new Vector3((minX + maxX) / 2f, 0, (minZ + maxZ) / 2f);

        foreach (Transform item in transform)
        {
            item.localPosition -= center;
        }
    }
}
