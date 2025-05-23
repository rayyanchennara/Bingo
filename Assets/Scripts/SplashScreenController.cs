using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenController : MonoBehaviour
{
    public Button playButton;
    public BannerViewController bannerViewController;
    public InterstitialAdsController interstitialAdsController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playButton.onClick.AddListener(() =>
        {
            //  SceneManager.LoadScene("SceneName");
            interstitialAdsController.LoadInterstitialAd().SetOnComplete(() =>
            {
                Debug.Log("Completed");
            });
        });

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            bannerViewController.CreateBannerView();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
