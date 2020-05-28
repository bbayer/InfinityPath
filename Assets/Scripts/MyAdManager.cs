using UnityEngine;

//using System.Collections;
//using Heyzap;

public class MyAdManager : MonoBehaviour
{/*
	public static string kHeyzapId = "8e27867dc132df9524bab1151ce61fc0";//ketchapp: ff4d63e68db9518cacc9a4b5d8e375d8
	object lastData;
	public float lastTimeInterstitialPlayed;
	public float interstitialInterval;
	// Use this for initialization
	void Start ()
	{
		GameManager.OnEvent += GameManager_OnEvent;

		HeyzapAds.Start(kHeyzapId, HeyzapAds.FLAG_NO_OPTIONS);
		//Fetch all 
		HZIncentivizedAd.Fetch();
		HZInterstitialAd.Fetch();
		HZVideoAd.Fetch();

		HZIncentivizedAd.SetDisplayListener(OnVideoEvent);
		HZInterstitialAd.SetDisplayListener(OnVideoEvent);
		HZVideoAd.SetDisplayListener(OnVideoEvent);
		HZBannerAd.SetDisplayListener(BannerDisplayCallback);

		HZBannerShowOptions opts = new HZBannerShowOptions();
		opts.Position = HZBannerShowOptions.POSITION_BOTTOM;
		HZBannerAd.ShowWithOptions(opts);
		//HeyzapAds.ShowMediationTestSuite();
		lastTimeInterstitialPlayed=Time.realtimeSinceStartup;
	}
	
	void GameManager_OnEvent (string name, object value)
	{
		if (name == "show_incentivized_ad"){
			lastData=value;
			if(GameManager.Instance.adDebugEnabled){
				Invoke("IncentivizedSuccess",1);
			}
			else{
				if(HZIncentivizedAd.IsAvailable()){
					HZIncentivizedAd.Show();
				}
				else{
					GameManager.Instance.PublishEvent("incentivized_ad_not_ready");

				}
			}
		}
		else if(name=="show_interstitial_ad"){
			if(Time.realtimeSinceStartup - lastTimeInterstitialPlayed > interstitialInterval){
				if(HZInterstitialAd.IsAvailable()){
					HZInterstitialAd.Show();
					GameManager.Instance.PublishEvent("interstitial_ad_success");
				}
				else{
					GameManager.Instance.PublishEvent("interstitial_ad_not_ready");
				}
			}
			else{
				GameManager.Instance.PublishEvent("interstitial_ad_not_timedout");

			}
			
		}
		else if(name=="refresh_banner"){
			
		}
		else if (name=="show_hz_test_suite"){
			HeyzapAds.ShowMediationTestSuite();
		}
	}

	void OnDestroy(){
		GameManager.OnEvent -= GameManager_OnEvent;
	}

	void BannerDisplayCallback(string adState, string adTag){
		if (adState == "loaded") {
			// Do something when the banner ad is loaded
			GameManager.Instance.PublishEvent("banner_shown");
		}
		if (adState == "error") {
			// Do something when the banner ad fails to load (they can fail when refreshing after successfully loading)
		}
		if (adState == "click") {
			// Do something when the banner ad is clicked, like pause your game
		}	
	}

	void OnVideoEvent(string adState, string adTag) {
		if ( adState.Equals("incentivized_result_complete") ) {
			// The user has watched the entire video and should be given a reward.
			IncentivizedSuccess();
		}
		if ( adState.Equals("incentivized_result_incomplete") ) {
			// The user did not watch the entire video and should not be given a   reward.
			IncentivizedFail();
		}

		if ( adState.Equals("audio_starting") ) {
			// The ad about to be shown will need audio.
			// Mute any background music.
			SoundManager.Instance.MuteMusic();
		}
		if ( adState.Equals("audio_finished") ) {
			// The ad being shown no longer needs audio.
			// Any background music can be resumed.
			SoundManager.Instance.UnmuteMusic();

		}
	}

	void IncentivizedSuccess(){
		GameManager.Instance.PublishEvent("incentivized_video_completed", lastData);

	}

	void IncentivizedFail(){
		GameManager.Instance.PublishEvent("incentivized_video_failed", lastData);
	}
	*/
}

