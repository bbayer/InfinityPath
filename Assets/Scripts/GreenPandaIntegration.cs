using UnityEngine;
using System.Collections;
using Facebook.Unity;
public class GreenPandaIntegration : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		GameManager.OnEvent += GameManager_OnEvent;

	}
	
	void GameManager_OnEvent (string name, object value)
	{
		if(name=="gamestate")
		{
			if(value.ToString()=="run"){
				FB.LogAppEvent("game_start");
			}
			
		}
		else if(name=="incentivized_video_completed" && value.ToString()=="double_coins"){
			FB.LogAppEvent("double_coins_shown");

		}
		else if(name=="incentivized_video_completed" && value.ToString()=="omc"){
			FB.LogAppEvent("omc_shown");

		}
		else if(name=="banner_shown"){
			FB.LogAppEvent("banner_shown");

		}
		else if(name=="level_achieved"){
			FB.LogAppEvent("level_achieved");

		}else if(name=="interstitial_ad_success"){
			FB.LogAppEvent("ad_shown_interstitial");
		}
	}

	void OnDestroy(){
		GameManager.OnEvent -= GameManager_OnEvent;
	}
}

