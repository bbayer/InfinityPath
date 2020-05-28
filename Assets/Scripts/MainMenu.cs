using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Awake(){
		GameManager.OnEvent += GameManager_OnEvent;
	}

	void GameManager_OnEvent (string name, object value)
	{
		if(name == "gamestate"){
			bool cond= (name == "gamestate" && value.ToString()=="mainmenu");
			gameObject.SetActive (cond);
		}
	}

	void OnDestroy(){
		GameManager.OnEvent -= GameManager_OnEvent;
	}

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/*
	void Visibility(){

		if(GameManager.Instance.m_GameState == GameManager.GameState.GameStateMainMenu){
			gameObject.SetActive (true);
			//SoundManager.Instance.Gong();
		}
		else{
			gameObject.SetActive (false);
		}


		/*
		Text label = transform.FindChild ("lblBestScore").GetComponent<Text> ();
		float meters = PlayerPrefs.GetFloat ("TotalDistanceTravelled", 0);
		string formattedDistance;

		if (meters < 1000) {
			formattedDistance = string.Format ("{0:F1} m", meters);
		} else {
			formattedDistance = string.Format("{0:F1} km" ,meters/1000.0f);
		}
		formattedDistance += " travelled";

		int selected = PlayerPrefs.GetInt ("SelectedPlayer", 0);
		int playerHighScore = PlayerPrefs.GetInt ("PlayerBestScore" + selected, 0);

		label.text = "All Time Best : " + PlayerPrefs.GetInt ("BestScore", 0) + "\n"+
			GameManager.Instance.player.m_PlayerName + " Best Score : " + playerHighScore + "\n"+
			formattedDistance;

	}
*/

	void OnEnable(){
		
	}
}
