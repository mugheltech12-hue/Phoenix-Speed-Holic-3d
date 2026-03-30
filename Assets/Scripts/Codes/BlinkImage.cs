using UnityEngine;
using UnityEngine.UI;

public class BlinkImage : MonoBehaviour
{
    public static BlinkImage Instance;

    public BlinkStarIndexSO blinkStarIndexSO;

    public float blinkSpeed = 1f; // Speed of the blinking effect
    private Image image;
    private bool increasingAlpha = false;
    private float alpha = 1f;

    public Color[] starBlinkColors;

    private Color currentColor;
    private int colorNumber=0;

    private int randNumber = 0;

    

    //red, orange, yellow, green, blue, purple & pink

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeColor()
    {
        //var currentRandomNumber = randNumber;
        //randNumber = Random.Range(0, 6);
        

    }

    void Start()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("BlinkImage script requires an Image component on the same GameObject.");
        }
        var blinkStarNumber = blinkStarIndexSO.currentIndex;
        if (blinkStarNumber < 7)
        {
            image.color = starBlinkColors[blinkStarNumber];
            blinkStarIndexSO.currentIndex += 1;
        }
        else {
            image.color = starBlinkColors[0];

            blinkStarIndexSO.currentIndex = 0;
        }

    }

    //red, orange, yellow, green, blue, purple & pink

    void Update()
    {
        if (image != null)
        {
            // Adjust alpha value over time
            if (increasingAlpha)
            {
                alpha += Time.deltaTime * blinkSpeed;
                if (alpha >= 1f)
                {
                    alpha = 1f;
                    increasingAlpha = false;
                }
            }
            else
            {
                alpha -= Time.deltaTime * blinkSpeed;
                if (alpha <= 0.3f)
                {
                    alpha = 0f;
                    increasingAlpha = true;
                }
            }

            // Apply the alpha change
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;
        }
    }
    private void OnApplicationQuit()
    {
        blinkStarIndexSO.currentIndex = 0;
    }
}
