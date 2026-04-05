using UnityEngine;
using System;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;

public enum AdsResult
{
    Finished,
    Skipped,
    Failed
}

public class AdsManager : MonoBehaviour, IRewardedVideoAdListener, IInterstitialAdListener, IBannerAdListener, IAppodealInitializationListener
{
    // =====================
    // SINGLETON
    // =====================
    private static AdsManager instance;
    public static AdsManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("AdsManager");
                instance = obj.AddComponent<AdsManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    // =====================
    // INSPECTOR SETTINGS
    // =====================

    [Header("App Keys")]
    public string appKey = "";

    [Header("Banner Settings")]
    public BannerPosition bannerPosition = BannerPosition.Bottom;
    public bool useSmartBanner = true;

    public enum BannerPosition { Bottom, Top, Left, Right }

    [Header("Ad Type Settings")]
    public bool enableInterstitial = true;
    public bool enableRewardedVideo = true;
    public bool enableBanner = true;

    [Header("Testing")]
    public bool testingMode = true;

    // =====================
    // PRIVATE VARS
    // =====================
    private static bool isInitialized = false;
    private Action<AdsResult> rewardedCallback;
    public bool Showappopenad = true;

    // =====================
    // AWAKE
    // =====================
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    

    void Start()
    {
        InitializeAds();
    }


    // =====================
    // INITIALIZE
    // =====================
    public void InitializeAds()
    {
        if (isInitialized) return;
        isInitialized = true;

        Appodeal.setTesting(testingMode);
        Appodeal.setLogLevel(Appodeal.LogLevel.Verbose);
        Appodeal.setSmartBanners(useSmartBanner);
        Appodeal.setUseSafeArea(true);

        // Callbacks set karo
        Appodeal.setBannerCallbacks(this);
        Appodeal.setInterstitialCallbacks(this);
        Appodeal.setRewardedVideoCallbacks(this);

        int adTypes = 0;
        if (enableInterstitial) adTypes |= Appodeal.INTERSTITIAL;
        if (enableRewardedVideo) adTypes |= Appodeal.REWARDED_VIDEO;
        if (enableBanner) adTypes |= Appodeal.BANNER;

        Appodeal.initialize(appKey, adTypes, this);

        Debug.Log("✅ Appodeal Initialized!");
    }

    // =====================
    // INTERSTITIAL
    // =====================
    public void ShowInterstitial()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("⛔ No internet for Interstitial.");
            return;
        }
        if (PlayerPrefs.GetInt("RemoveAds") != 0) return;

        if (Appodeal.isLoaded(Appodeal.INTERSTITIAL) && Appodeal.canShow(Appodeal.INTERSTITIAL))
        {
            Appodeal.show(Appodeal.INTERSTITIAL);
            Debug.Log("✅ Interstitial Showing.");
        }
        else
        {
            Debug.Log("⚠️ Interstitial not ready yet.");
        }
    }

    // =====================
    // REWARDED VIDEO
    // =====================
    public void ShowRewardedVideo(Action<AdsResult> callback)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            callback?.Invoke(AdsResult.Failed);
            return;
        }
        if (PlayerPrefs.GetInt("RemoveAds") != 0) return;

        if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO) && Appodeal.canShow(Appodeal.REWARDED_VIDEO))
        {
            rewardedCallback = callback;
            Appodeal.show(Appodeal.REWARDED_VIDEO);
            Debug.Log("✅ Rewarded Video Showing.");
        }
        else
        {
            Debug.Log("⚠️ Rewarded Video not ready yet.");
            callback?.Invoke(AdsResult.Failed);
        }
    }

    // =====================
    // BANNER
    // =====================
    public void ShowBanner()
    {
        if (PlayerPrefs.GetInt("RemoveAds") != 0) return;

        switch (bannerPosition)
        {
            case BannerPosition.Bottom: Appodeal.show(Appodeal.BANNER_BOTTOM); break;
            case BannerPosition.Top: Appodeal.show(Appodeal.BANNER_TOP); break;
            case BannerPosition.Left: Appodeal.show(Appodeal.BANNER_LEFT); break;
            case BannerPosition.Right: Appodeal.show(Appodeal.BANNER_RIGHT); break;
        }
        Debug.Log("✅ Banner Showing.");
    }

    public void HideBanner()
    {
        Appodeal.hide(Appodeal.BANNER);
        Debug.Log("Banner Hidden.");
    }

    public void HideAllBanners()
    {
        Appodeal.hide(Appodeal.BANNER);
    }

    // =====================
    // REMOVE ADS
    // =====================
    public bool IsAdsRemoved()
    {
        return PlayerPrefs.GetInt("RemoveAds") != 0;
    }

    public void RemoveAds()
    {
        PlayerPrefs.SetInt("RemoveAds", 1);
        HideAllBanners();
        Debug.Log("✅ Ads Removed!");
    }

    // =====================
    // INITIALIZATION CALLBACK
    // =====================
    public void onInitializationFinished(System.Collections.Generic.List<string> errors)
    {
        if (errors == null || errors.Count == 0)
            Debug.Log("✅ Appodeal Init Finished - No errors!");
        else
            Debug.LogWarning("⚠️ Appodeal Init errors: " + string.Join(", ", errors));
    }

    // =====================
    // REWARDED VIDEO CALLBACKS
    // =====================
    public void onRewardedVideoLoaded(bool isPrecache) => Debug.Log("✅ Rewarded Video Loaded.");
    public void onRewardedVideoFailedToLoad() => Debug.Log("❌ Rewarded Video Failed to Load.");
    public void onRewardedVideoShowFailed() => rewardedCallback?.Invoke(AdsResult.Failed);
    public void onRewardedVideoShown() => Debug.Log("Rewarded Video Shown.");
    public void onRewardedVideoExpired() => Debug.Log("Rewarded Video Expired.");
    public void onRewardedVideoClicked() => Debug.Log("Rewarded Video Clicked.");

    public void onRewardedVideoFinished(double amount, string name)
    {
        Debug.Log($"✅ Rewarded! Amount: {amount} {name}");
        rewardedCallback?.Invoke(AdsResult.Finished);
        rewardedCallback = null;
    }

    public void onRewardedVideoClosed(bool finished)
    {
        if (!finished)
        {
            rewardedCallback?.Invoke(AdsResult.Skipped);
            rewardedCallback = null;
        }
        Invoke("ResetAppOpenBool", 3f);
    }

    // =====================
    // INTERSTITIAL CALLBACKS
    // =====================
    public void onInterstitialLoaded(bool isPrecache) => Debug.Log("✅ Interstitial Loaded.");
    public void onInterstitialFailedToLoad() => Debug.Log("❌ Interstitial Failed to Load.");
    public void onInterstitialShowFailed() => Debug.Log("❌ Interstitial Show Failed.");
    public void onInterstitialShown() => Debug.Log("Interstitial Shown.");
    public void onInterstitialClicked() => Debug.Log("Interstitial Clicked.");
    public void onInterstitialExpired() => Debug.Log("Interstitial Expired.");
    public void onInterstitialClosed()
    {
        Debug.Log("Interstitial Closed.");
        Invoke("ResetAppOpenBool", 3f);
    }

    // =====================
    // BANNER CALLBACKS
    // =====================
    public void onBannerLoaded(int height, bool isPrecache) => Debug.Log("✅ Banner Loaded.");
    public void onBannerFailedToLoad() => Debug.Log("❌ Banner Failed to Load.");
    public void onBannerShown() => Debug.Log("Banner Shown.");
    public void onBannerShowFailed() => Debug.Log("❌ Banner Show Failed.");
    public void onBannerClicked() => Debug.Log("Banner Clicked.");
    public void onBannerExpired() => Debug.Log("Banner Expired.");

    // =====================
    // HELPERS
    // =====================
    void ResetAppOpenBool()
    {
        Showappopenad = true;
    }
}