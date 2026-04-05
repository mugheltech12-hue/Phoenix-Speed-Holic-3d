using UnityEngine;

public class ShowBanner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnEnable()
    {
        if(AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowBanner();
        }
       
    }

    private void OnDisable()
    {
        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.HideBanner();
        }
    }


}
