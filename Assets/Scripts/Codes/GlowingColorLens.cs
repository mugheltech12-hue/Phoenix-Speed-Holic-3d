using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowingColorLens : MonoBehaviour
{
    public SpriteRenderer glowingPoint;

    public SpriteRenderer glowPlayPoint;

    public Color[] glowColors;


    private void OnEnable()
    {
        var maxColrs = glowColors.Length;

        var randColorNumber = Random.Range(0, maxColrs);

        glowingPoint.color = glowColors[randColorNumber];
    }


    private void OnDisable()
    {
        if (glowPlayPoint != null)
        {
            glowPlayPoint.gameObject.SetActive(true);
            glowPlayPoint.color = glowingPoint.color;
        }
        
    }

    void Start()
    {
        
    }
}
