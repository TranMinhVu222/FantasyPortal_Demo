using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    string _gameId;
    [SerializeField] bool _testMode = true;
    public string typeOfReward;
    string _androidAdUnitId = "Rewarded_Android";
    string _iOSAdUnitId = "Rewarded_iOS";
    string _adUnitId = null;

    private void Awake()
    {
        if (Advertisement.isInitialized)
        {
            Debug.Log("Advertisement is Initialized");
            LoadRewardedAd();
        }
        else
        {
            InitializeAds();
        }
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif
        // Disable the button until the ad is ready to show:
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
    }
    public static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSGameId : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        LoadInerstitialAd();
        // LoadBannerAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void LoadInerstitialAd()
    {
        Advertisement.Load("Interstitial_Android", this);
    }

    //Được gọi trong sự kiện OnClick trong button để load Ads
    public void LoadRewardedAd()
    {
        Advertisement.Load("Rewarded_Android", this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        // Debug.Log("OnUnityAdsAdLoaded");
        // Advertisement.Show(placementId,this);
    }

    //Được gọi trong sự kiện OnClick trong button đồng ý xem để show Ads
    public void PlayRewardAds()
    {
        Advertisement.Show("Rewarded_Android",this);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {placementId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log("OnUnityAdsShowFailure");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("OnUnityAdsShowStart");
        Time.timeScale = 0;
        // Advertisement.Banner.Hide();
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("OnUnityAdsShowClick");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("OnUnityAdsShowComplete "+showCompletionState);
        if (placementId.Equals("Rewarded_Android") && UnityAdsShowCompletionState.COMPLETED.Equals(showCompletionState))
        {
            Debug.Log(typeOfReward);
            RewardPlayer(typeOfReward);
        }
        Time.timeScale = 1;
        // Advertisement.Banner.Show("Banner_Android");
    }
    
    public void LoadBannerAd()
    {
        // Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        // Advertisement.Banner.Load("Banner_Android",
        //     new BannerLoadOptions
        //     {
        //         loadCallback = OnBannerLoaded,
        //         errorCallback = OnBannerError
        //     }
        //     );
    }

    void OnBannerLoaded()
    {
        Advertisement.Banner.Show("Banner_Android");
    }

    void OnBannerError(string message)
    {

    }

    //
    public void RewardPlayer(string type)
    {
        switch (type)
        {
            case "starReward":
                WatchAdEvent.Instance.PressWatchAdStarButton();
                break;
            case "booster":
                WatchAdEvent.Instance.PressWatchAdButton();
                break;
            case "duplicateStar":
                WatchAdEvent.Instance.PressWatchAdsDoubleStars();
                break;
        }
    }
    
    //Tên của loại thưởng set trong nút button
    public void NameTypeReward(string name)
    {
        typeOfReward = name;
    }
}