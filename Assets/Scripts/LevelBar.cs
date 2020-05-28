using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBar : MonoBehaviour {

	public struct LevelBarValues{
		public int minVal;
		public int maxVal;
		public int val;
		public int level;
	}

	public Text minText;
	public Text maxText;
	public Text levelText;
	public Image progressBar;
	public Image tinyBar;
	public Text tinyBarText;

	void Awake(){
		GameManager.OnEvent += GameManager_OnEvent;

	}

	void GameManager_OnEvent (string name, object value){
		if(name=="level_bar_update"){
			LevelBarValues values=(LevelBarValues)value;
			UpdateUI(values);
		}
	}
	public void UpdateUI(LevelBarValues values){
		if(minText!=null){
			minText.text=values.minVal.ToString();
		}
		if(maxText!=null){
			maxText.text=values.maxVal.ToString();
		}

		if(levelText!=null){
			levelText.text="Level "+(values.level+1).ToString();
		}
		if(progressBar!=null){
			progressBar.fillAmount= ((float)(values.val - values.minVal))/(values.maxVal-values.minVal);
		}
		if(tinyBarText!=null){
			tinyBarText.text=values.val.ToString();

		}
		if(tinyBar!=null){
			Vector2 pos=tinyBar.rectTransform.anchoredPosition;
			tinyBar.rectTransform.anchoredPosition=new Vector2(
				progressBar.fillAmount*400f,
				pos.y);
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
