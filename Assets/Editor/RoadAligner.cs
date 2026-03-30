using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// ==============================================================
//  RoadBorderLine — FINAL VERSION
//
//  Actual Structure (confirmed):
//    BG_Group
//      Ring        → children mein kahin bhi 'LeftBorder' ya 'Cube' hai
//      Ring (1)    → same
//      Ring (2)    → same
//      ...
//
//  Fix: Find() ki jagay loop se pehla matching child dhundta hai
// ==============================================================

public class RoadBorderLine : EditorWindow
{
    private float lineWidth = 0.3f;
    private Color lineColor = new Color(1f, 0.4f, 0f, 1f);
    private float yOffset = 0f;
    private bool smoothCurve = true;
    private int smoothSteps = 5;

    // Aap yahan jo bhi child name use karna chahein
    // "LeftBorder" ya "Cube" dono kaam karenge
    private string targetName = "LeftBorder";

    [MenuItem("Tools/Road Border Line")]
    public static void ShowWindow()
    {
        GetWindow<RoadBorderLine>("Road Border Line");
    }

    void OnGUI()
    {
        GUILayout.Label("Road Border — Final Version", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "BG_Group select karein.\n" +
            "Target Child = jis object ki position chahiye har Ring mein.",
            MessageType.Info);

        EditorGUILayout.Space();
        targetName = EditorGUILayout.TextField("Target Child Name", targetName);
        EditorGUILayout.Space();

        lineWidth = EditorGUILayout.Slider("Line Width", lineWidth, 0.01f, 3f);
        lineColor = EditorGUILayout.ColorField("Line Color", lineColor);
        yOffset = EditorGUILayout.FloatField("Y Offset", yOffset);
        smoothCurve = EditorGUILayout.Toggle("Smooth Curve", smoothCurve);
        if (smoothCurve)
            smoothSteps = EditorGUILayout.IntSlider("Smooth Steps", smoothSteps, 1, 20);

        EditorGUILayout.Space();

        GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);
        if (GUILayout.Button("Generate Line Border", GUILayout.Height(45)))
            GenerateLine();

        GUI.backgroundColor = Color.white;
        EditorGUILayout.Space();

        GUI.backgroundColor = new Color(0.9f, 0.4f, 0.4f);
        if (GUILayout.Button("Delete Existing Border", GUILayout.Height(30)))
            DeleteExisting();

        GUI.backgroundColor = Color.white;
    }

    void GenerateLine()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("Road Border Line", "BG_Group select karein!", "OK");
            return;
        }

        Transform parent = selected.transform;
        List<Vector3> points = new List<Vector3>();
        int missed = 0;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform ring = parent.GetChild(i);

            // Loop se pehla matching child dhundho — index se nahi
            Transform target = FindFirstChildByName(ring, targetName);

            if (target == null)
            {
                missed++;
                Debug.LogWarning($"[Road Border] {ring.name}: '{targetName}' nahi mila — skip");
                continue;
            }

            points.Add(target.position + Vector3.up * yOffset);
        }

        if (points.Count < 2)
        {
            EditorUtility.DisplayDialog("Road Border Line",
                $"Sirf {points.Count} point(s) mile!\n\n" +
                $"• BG_Group select kiya?\n" +
                $"• Target name '{targetName}' sahi hai?\n\n" +
                $"Rings checked: {parent.childCount}\n" +
                $"Missed: {missed}", "OK");
            return;
        }

        Vector3[] raw = points.ToArray();
        Vector3[] final = smoothCurve ? CatmullRomSmooth(raw, smoothSteps) : raw;

        DeleteExisting();

        GameObject obj = new GameObject("RoadBorder_Line");
        Undo.RegisterCreatedObjectUndo(obj, "Create Road Border");

        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.useWorldSpace = true;
        lr.positionCount = final.Length;
        lr.numCornerVertices = 8;
        lr.numCapVertices = 8;
        lr.SetPositions(final);

        Selection.activeGameObject = obj;

        string msg = $"Done!\n{points.Count} Rings → {final.Length} points banay.";
        if (missed > 0) msg += $"\n({missed} rings skip hue — Console mein dekho)";

        Debug.Log($"[Road Border] {msg}");
        EditorUtility.DisplayDialog("Road Border Line", msg, "OK");
    }

    // KEY FIX: Index ki parwah nahi — naam se dhundho
    Transform FindFirstChildByName(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).name == name)
                return parent.GetChild(i);
        }
        return null;
    }

    Vector3[] CatmullRomSmooth(Vector3[] pts, int steps)
    {
        var result = new List<Vector3>();
        for (int i = 0; i < pts.Length - 1; i++)
        {
            Vector3 p0 = i == 0 ? pts[i] : pts[i - 1];
            Vector3 p1 = pts[i];
            Vector3 p2 = pts[i + 1];
            Vector3 p3 = i + 2 >= pts.Length ? pts[i + 1] : pts[i + 2];
            for (int s = 0; s <= steps; s++)
            {
                float t = s / (float)steps;
                result.Add(CatmullRom(p0, p1, p2, p3, t));
            }
        }
        result.Add(pts[pts.Length - 1]);
        return result.ToArray();
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t, t3 = t2 * t;
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    void DeleteExisting()
    {
        var old = GameObject.Find("RoadBorder_Line");
        if (old != null) Undo.DestroyObjectImmediate(old);
    }
}