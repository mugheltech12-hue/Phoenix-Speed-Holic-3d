using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAlignment : MonoBehaviour
{
    public GameObject sun;
    public GameObject backgroundImage;
    public GameObject sunInThePath;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sun.transform.position = sunInThePath.transform.position;
        backgroundImage.transform.position = sunInThePath.transform.position;
    }
}
