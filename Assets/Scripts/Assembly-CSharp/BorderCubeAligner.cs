using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BorderCubeAligner : MonoBehaviour
{
    [Header("Sab Border Cubes IN ORDER drag karo yahan")]
    public List<Transform> borderCubes;

    [Header("Settings")]
    public float cubeOriginalLength = 3.034238f;

    void Awake()
    {
        AlignAllBorderCubes();
    }

#if UNITY_EDITOR
    // Inspector mein button banana ke liye
    [ContextMenu("ALIGN NOW (Editor mein apply karo)")]
    void AlignInEditor()
    {
        AlignAllBorderCubes();
        // Permanently save karo
        foreach (var cube in borderCubes)
        {
            if (cube != null)
                EditorUtility.SetDirty(cube);
        }
        Debug.Log("Editor mein permanently aligned!");
    }
#endif

    void AlignAllBorderCubes()
    {
        if (borderCubes == null || borderCubes.Count < 2)
        {
            Debug.LogError("[BorderCubeAligner] Kam se kam 2 cubes assign karo!");
            return;
        }

        for (int i = 0; i < borderCubes.Count - 1; i++)
        {
            Transform current = borderCubes[i];
            Transform next = borderCubes[i + 1];

            if (current == null || next == null) continue;

            Vector3 direction = next.position - current.position;
            float distance = direction.magnitude;

            if (distance < 0.01f) continue;

            current.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up)
                               * Quaternion.Euler(90, 0, 0);

            Vector3 scale = current.localScale;
            scale.y = distance / cubeOriginalLength;
            current.localScale = scale;
        }

        Debug.Log($"[BorderCubeAligner] Done! {borderCubes.Count - 1} cubes aligned.");
    }
}