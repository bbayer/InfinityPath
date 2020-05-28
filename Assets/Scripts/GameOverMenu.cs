using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
 * TODO:
 * context aware earn coin button 
 * new best
 * share to earn
 * shop button animation after 200C
 * share button animation
 * 
 * */
public class GameOverMenu : MonoBehaviour {


	public Button m_btnWatchVideo;
	public Button m_btnShop;
	public int m_currentReward;
	private int m_gameCount;
	private int m_videoButtonVisibility;

	public GameObject lblCoins;
	public Text lblBestScore;
	public Text lblScore;
	public enum RewardedAction{
		SHARE,
		WATCH_VIDEO,
	}

	public RewardedAction m_currentRewardedAction;

	void Awake(){
		GameManager.OnEvent += GameManager_OnEvent;


		m_gameCount = 0;

	}

	void GameManager_OnEvent (string name, object value)
	{
		if(name=="gamestate")
		{
			if(value.ToString()=="over"){
				gameObject.SetActive (true);
			}else{
				gameObject.SetActive (false);

			}
		}
		else if(name=="incentivized_video_completed" && value.ToString()=="double_coins"){
			ApplyCurrentReward();
		}
	}

	void OnDestroy(){
		GameManager.OnEvent -= GameManager_OnEvent;
	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnVideoButtonClicked(){
		m_currentReward=GameManager.Instance.player.m_GatheredCoins;
		Debug.Log("GAT:"+GameManager.Instance.player.m_GatheredCoins);
		GameManager.Instance.PublishEvent("show_incentivized_ad","double_coins");
		m_btnWatchVideo.gameObject.SetActive(false);

	}

	public void OnEnable(){
		//When game over menu enabled.
		if(GameManager.Instance.player == null)
			return;

		//Don't show video after getting reward
		//m_videoButtonVisibility = Mathf.Max(--m_videoButtonVisibility,0);
		InitializeUserInterface();
		CheckForShopButtonAnimation ();
		//TODO
		//CheckToShowInterstitialAds ();

//		if(m_videoButtonVisibility == 0 && Random.Range(0,3) == 0){
//			iTween.ScaleTo(m_btnWatchVideo.gameObject,iTween.Hash("x",1.1f,"y",1.1f,"easetype",iTween.EaseType.easeOutElastic, "looptype",iTween.LoopType.pingPong,"time",1));
//		}
	}

	void InitializeUserInterface(){

		int currentScore = GameManager.Instance.player.m_Score;
		int bestScore = PlayerPrefs.GetInt ("BestScore", 0);
		
		lblScore.text = "" + currentScore;

		
		lblCoins.GetComponent<Text> ().text = "" + PlayerPrefs.GetInt("TotalCoins",0);
		lblCoins.transform.Rotate(Vector3.zero);
		//TODO:
		//m_btnWatchVideo.gameObject.SetActive(true);

		if( currentScore == bestScore && bestScore !=0 ){
			lblBestScore.text = "NEW BEST " + bestScore;
//			OnNewBestScore();
		}
		else{
			lblBestScore.text = "BEST " + bestScore;
//			if (Random.Range(0,15)==0 ) {
//				OnNewBestScore ();
//			}
		}
	}

	void InitializeVideoReward(){
		iTween.Stop(m_btnWatchVideo.gameObject);
		m_btnWatchVideo.transform.localScale = Vector3.one;
		if(m_videoButtonVisibility == 0){
			m_btnWatchVideo.gameObject.SetActive(true);
		}
		m_currentRewardedAction = RewardedAction.WATCH_VIDEO;
		m_currentReward = 50;
		SetVideoButtonText ("\u25B6 EARN 50C");
	}

	public void OnNewBestScore(){
		m_currentReward = Random.Range (2, 5) * 10;
		m_currentRewardedAction = RewardedAction.SHARE;
		SetVideoButtonText("SHARE TO EARN " + m_currentReward+"C");
		//Make video button visible for sake of best score
		m_videoButtonVisibility = 0;
		m_btnWatchVideo.gameObject.SetActive(true);

	}

	IEnumerator InterstitialDelayedLaunch(){
		yield return new WaitForSeconds(1.0f);
		//HZInterstitialAd.Show();

	}

	public void CheckToShowInterstitialAds(){/*
		if (m_gameCount++ % 4 == 3) {
			if (HZInterstitialAd.IsAvailable()) {
				Debug.Log("showing interstitialad");
				StartCoroutine(InterstitialDelayedLaunch());
			}
			else{
				Debug.Log("InterstitialAd not available");
			}
		}*/
	}

	public void ApplyCurrentReward(){
		int totalCoins = PlayerPrefs.GetInt("TotalCoins",0);

		PlayerPrefs.SetInt ("TotalCoins", totalCoins + m_currentReward);
		GameObject coins = transform.Find ("lblCoins").gameObject;
		coins.GetComponent<Text> ().text = "" + PlayerPrefs.GetInt("TotalCoins",0);

		iTween.PunchRotation(coins,iTween.Hash("z",5f,"looptype","none","time",5));
		SoundManager.Instance.Reward();
		//Setup wathch video visibitity
		m_btnWatchVideo.gameObject.SetActive(false);
		m_videoButtonVisibility = 3;
	}


	private void SetVideoButtonText(string text){
		Text t = m_btnWatchVideo.GetComponentInChildren<Text> ();
		t.text = text;
		
	}


	void CheckForShopButtonAnimation(){
		iTween.Stop(m_btnShop.gameObject);
		m_btnShop.transform.localScale = new Vector3(.2f,.2f,1f);

		int totalCoins = PlayerPrefs.GetInt("TotalCoins",0);
		bool isTherePurchaseable = false;
		for(int i=0;i<GameManager.Instance.m_Players.Length;i++){
			if( PlayerPrefs.GetInt("IsPlayerUnlocked"+i,0) == 0){
				isTherePurchaseable = true;
				break;
			}

		}

		if (totalCoins > 200 && isTherePurchaseable) {
			iTween.ScaleTo(m_btnShop.gameObject,iTween.Hash("x",.25f,"y",.25f,"easetype",iTween.EaseType.easeOutElastic, "looptype","loop","time",1));
		}
	}



}
