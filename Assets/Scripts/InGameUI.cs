using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour {
	public Text lblScore;
	public GameObject lblMessage;
	public Image imgBestScore;
	public Text lblCoin;
	public GameObject bonusUI;
	Image[] imgs;
	void Awake(){
		GameManager.OnEvent += GameManager_OnEvent;
		imgs = bonusUI.GetComponentsInChildren<Image>();
	}
		
	void GameManager_OnEvent (string name, object value)
	{
		if(name == "gamestate"){
			bool cond= (name == "gamestate" && value.ToString()=="run");
			gameObject.SetActive (cond);
			OnBonusCollected(0);
		}
		else if(name == "bestscore"){
			OnBestScore();
		}
		else if(name == "score"){
			OnScore((int)value);
		}
		else if(name=="message_start"){
			OnMessage(value.ToString(),true);
		}
		else if(name=="message_end"){
			OnMessage(value.ToString(), false);
		}
		else if(name=="coin"){
			OnCoin((int)value);
		}
		else if(name=="bonus_collected_count"){
			OnBonusCollected((int)value);
		}else if(name == "bonus_amount"){
			OnBonusAmount((int)value);
		}
			
	}

	void OnBonusCollected(int value){
		
		for(int i=0;i<imgs.Length;i++){
			imgs[i].color = new Color(1,1,1,.2f);
		}
		value = value % imgs.Length;
		for(int i=0;i<value;i++){
			imgs[i].color = new Color(1,1,1,1f);
		}
	}

	void OnBonusAmount(int value){
		Text txt = bonusUI.GetComponentInChildren<Text>();
		if(txt != null)
			txt.text = "+"+value.ToString();
	}

	void OnBestScore(){
		if(imgBestScore)
			StartCoroutine( AnimateBestScore() );
	}

	void OnScore(int score){
		if(lblScore)
			lblScore.text = score.ToString();
	}

	void OnMessage(string msg, bool start){
		if(lblMessage){
			Debug.Log("OnMessage;"+msg);
			lblMessage.GetComponent<Text>().text = msg;
			RectTransform r = lblMessage.GetComponent<RectTransform>();
			LeanTween.cancel(r);
			lblMessage.GetComponent<Text>().color = new Color(1,1,1,0);
	
			LeanTween.rotateZ(lblMessage.gameObject,0,.3f).setFrom(Random.Range(-40,40));
			LeanTween.scale(r,Vector3.one,.3f).setFrom(new Vector3(1.2f,1.2f,1f));
			LeanTween.alphaText(r,1,.3f).setEaseInCubic();
			LeanTween.alphaText(r,0,.3f).setEaseOutCubic().setDelay(.6f);
		}
	}

	void OnCoin(int val){
		if(lblCoin)
			lblCoin.text = val.ToString();
	}
	void OnDestroy(){
		GameManager.OnEvent -= GameManager_OnEvent;
	}
	// Use this for initialization
	void Start () {
		//lblCoins = transform.Find ("lblCoins").GetComponent<Text>();
		//lblScore = transform.Find ("lblScore").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	}


	void OnEnable(){
		Debug.Log("enabled");
		imgBestScore.gameObject.SetActive(false);
		StopAllCoroutines();
	}

	IEnumerator AnimateBestScore(){
		GameObject g = imgBestScore.gameObject;
		bool toggle = true;
		while (true) {
			g.SetActive (toggle);
			toggle = !toggle;
			yield return new WaitForSeconds (.3f);
		}
	}
}
