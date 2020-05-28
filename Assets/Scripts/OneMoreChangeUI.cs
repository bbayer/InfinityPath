using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OneMoreChangeUI : MonoBehaviour {

	public Image progress;
	public RectTransform btnOneMoreChange;
	public float timeout;
	// Use this for initialization
	void Awake () {
		
		GameManager.OnEvent += GameManager_OnEvent;

	}
	
	// Update is called once per frame
	void Update () {
	}

	void StartTimer(){
		LeanTween.value( gameObject, updateProgress, 1,0, timeout).setOnComplete(OnTimeout);
		LeanTween.cancel(btnOneMoreChange);
		btnOneMoreChange.localScale=Vector3.one;
		LeanTween.scale(btnOneMoreChange,new Vector3(1.2f,1.2f,1.2f),.5f).setEaseInBounce().setLoopPingPong();
	}

	void updateProgress( float val ){
		progress.fillAmount=val;
	}

	void OnTimeout(){
		GameManager.Instance.FinishGame();

	}
	public void OnHeartClicked(){
		LeanTween.cancel(gameObject);
		GameManager.Instance.PublishEvent("show_incentivized_ad","omc");

	}

	public void OnCancelClicked(){
		LeanTween.cancel(gameObject);
		GameManager.Instance.FinishGame();
	}

	void GameManager_OnEvent (string name, object value)
	{
		if(name=="gamestate"){
			Debug.LogWarning("one more chance!!!!!!!!!!!!!!!!");

			if(value.ToString()=="show_omc"){
				gameObject.SetActive (true);

			}
			else{
				gameObject.SetActive(false);
			}
		}
		else if (name=="start_omc"){
			Debug.Log("Starting OMC timer");
			StartTimer();
		}
		else if(name=="incentivized_video_completed" && value.ToString()=="omc"){
			Debug.Log("One more chance successfull");
			GameManager.Instance.ContinueGame();
			
		}
		else if(name=="incentivized_video_failed"){
			Debug.Log("One more chance successfull");
			GameManager.Instance.FinishGame();
		}

	}
}
