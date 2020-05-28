using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Collections;
using BBCommon;

public class SocialIntegration : Singleton<SocialIntegration> {


	public string m_leaderboardID;
	// Use this for initialization
	void Start () {
		Social.localUser.Authenticate(onUserAuthenticate);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void onUserAuthenticate(bool success){
		if(success){
			Debug.Log("User authentication success");
		}
		else{
			Debug.Log("Cannot authenticate user");

		}

	}


	public void ReportScore(int score){
		try {
			Social.ReportScore(score, m_leaderboardID,null);

			
		} catch (System.Exception ex) {
			Debug.LogError("cannot report issue")	;
		}
	}

	public void ShowLeaderBoardUI(){
		Social.ShowLeaderboardUI();
	}
}
