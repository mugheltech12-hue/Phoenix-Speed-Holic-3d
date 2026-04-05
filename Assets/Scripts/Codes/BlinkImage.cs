using UnityEngine;

public class BlinkImage : MonoBehaviour
{
    public static BlinkImage Instance;

    public BlinkStarIndexSO blinkStarIndexSO;
    public Color[] starBlinkColors;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer nahi mila!");
            return;
        }

        // Start pe current index wala color laga do
        ApplyCurrentColor();
    }

    // ✅ Har baar play press ho tab yeh call hoga
    public void ChangeColor()
    {
        blinkStarIndexSO.currentIndex++;

        if (blinkStarIndexSO.currentIndex >= starBlinkColors.Length)
            blinkStarIndexSO.currentIndex = 0;

        ApplyCurrentColor();
    }

    private void ApplyCurrentColor()
    {
        if (spriteRenderer != null && starBlinkColors.Length > 0)
            spriteRenderer.color = starBlinkColors[blinkStarIndexSO.currentIndex];
    }

    private void OnApplicationQuit()
    {
        blinkStarIndexSO.currentIndex = 0;
    }
}