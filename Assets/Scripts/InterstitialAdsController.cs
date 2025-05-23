using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class InterstitialAdsController : MonoBehaviour
{
    private string _adUnitId = "ca-app-pub-3940256099942544/1033173712";
    private InterstitialAd _interstitialAd;
    public static InterstitialAdsController Instance;
    private Action OnComplete;

    public void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    public InterstitialAdsController LoadInterstitialAd(bool show = true)
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    OnComplete?.Invoke();
                    OnComplete = null;
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
                if (show)
                {
                    RegisterListener();
                    ShowInterstitialAd();
                }
            });
        return this;
    }
    public void SetOnComplete(Action OnComplete)
    {
        this.OnComplete = OnComplete;
    }

    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    private void RegisterListener()
    {
        // Raised when an ad opened full screen content.
        _interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
            OnComplete?.Invoke();
            OnComplete = null;
        };
        // Raised when the ad closed full screen content.
        _interstitialAd.OnAdFullScreenContentClosed += () =>
    {
            Debug.Log("Interstitial Ad full screen content closed.");
        OnComplete?.Invoke();
        OnComplete = null;

        // Reload the ad so that we can show another as soon as possible.
        LoadInterstitialAd(false);
        };
        // Raised when the ad failed to open full screen content.
        _interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            OnComplete?.Invoke();
            OnComplete = null;

            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd(false);
        };
    }

    private void OnDestroy()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
        }
    }
}
