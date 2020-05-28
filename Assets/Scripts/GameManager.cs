using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BBCommon;
using UnityEngine.SceneManagement;

using UnityEngine.SocialPlatforms;
using Facebook.Unity;
using GameAnalyticsSDK;
public class GameManager : BBCommon.Singleton<GameManager> {


	public static string kAppStoreURL = "https://appstor.es/infinity.path";
	public static string kGooglePlayURL = "https://play.google.com/store/apps/details?id=io.voodoo.paperio&hl=fr";
	public static string kShareText = "OMG!! I have scored {0} in {1}. Can you beat my score?";
	public static string kAppStoreId = "1138102771";
	public static string kGooglePlayId = "io.voodoo.paperio";
	public static string kFBAppLink = "fb://page/527358433996753";
	public static string kFBPageLink = "https://www.facebook.com/voodoogames/";
	public static string kLeaderboardName = "com.bbayer.leaderboard.infinitypath";


	public string m_GameState;
	public PlayerCubes player;
	public GameObject selectedPlayer;
	public bool adDebugEnabled;
	//Events and delegates


	public delegate void OnEventAction(string name, object value);
	public static event OnEventAction OnEvent;

	public Vector3 startPosition;
	public GameObject[] m_Players;
	private PathGenerator pathGen;
	public TileManagerCubes tileManager;
	int captureNdx;

	// Use this for initialization
	void Awake(){
		//GameObject ply = Instantiate(m_Players[PlayerPrefs.GetInt("SelectedPlayer",0)]);
		//player = ply.GetComponent<PlayerCubes>();
		InitializeUserPrefs();
		Application.targetFrameRate=60;
		QualitySettings.vSyncCount = 0;
		//Initialize Heyzap
		captureNdx = 0;
		//UnlockAll();
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init(InitCallback,OnHideUnity);
		} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
		}
		GameAnalytics.Initialize();
		Cursor.visible = false;

	}

	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	void Start () {	
		MainMenu ();


		Social.localUser.Authenticate(OnUserAuthenticate);
		//HeyzapAds.ShowDebugLogs();
		// Your Publisher ID is: 8e27867dc132df9524bab1151ce61fc0

	}

	public void SendFBEvent(string event_name){
		FB.LogAppEvent(event_name);
	}


	// Update is called once per frame
	void Update () {
		/*
		if (Input.GetMouseButtonDown (0)) {
			if (m_GameState == GameState.GameStateMainMenu) {
				Input.ResetInputAxes();
				StartGame();
			}
			else if(m_GameState == GameState.GameStateOver){
				Input.ResetInputAxes();
				MainMenu();
			}
		}
		*/

		if(Input.GetKeyDown(KeyCode.P)){
			ScreenCapture.CaptureScreenshot("capture"+captureNdx+".png");
			captureNdx++;
		}


		if(Input.GetKeyDown(KeyCode.C)){
			ColorManager.Instance.ColorTransition();
		}
		if(Application.targetFrameRate!=60){
			Application.targetFrameRate=60;
		}
		if(Input.GetKeyDown(KeyCode.Q)){
			if(Time.timeScale > 0 ){
				Time.timeScale=0f;
			}
			else{
				Time.timeScale=1f;
			}
		}

		selectedPlayer.transform.position = player.transform.position;
	}

	public void StartGame(){
		////Debug.Log ("Game started");
		PublishEvent("gamestate","run");
	}

	public void FinishGame(){
		////Debug.Log ("Game Over");

		PublishEvent("gamestate","over");

	}

	public void ContinueGame(){
		player.ContinueGame();
		PublishEvent("gamestate","run");
	}
	public void MainMenu(){
		PublishEvent("gamestate","mainmenu");
	}

	public void BestScore(int score){
		PublishEvent("bestscore",score);
	}

	public void ShopMenu(){
		PublishEvent("gamestate","shopmenu");
	}

	public void OnSelectPlayer(){
		ShopMenu();
	}

	void InitializeUserPrefs(){
		//TODO:Remove
		//PlayerPrefs.DeleteAll();

		for (int i=0; i<20; i++) {
			if(!PlayerPrefs.HasKey("IsPlayerUnlocked"+i)){
				PlayerPrefs.SetInt("IsPlayerUnlocked"+i,0);
			}

			if(!PlayerPrefs.HasKey("PlayerBestScore"+i)){
				PlayerPrefs.SetInt("PlayerBestScore"+i,0);
			}

			//TODO:Remove
			//PlayerPrefs.SetInt("IsPlayerUnlocked"+i,1);
		}

		if(!PlayerPrefs.HasKey("BestScore")){
			PlayerPrefs.SetInt("BestScore",0);
		}

		PlayerPrefs.SetInt("IsPlayerUnlocked0",1);

		if(!PlayerPrefs.HasKey("SelectedPlayer")){
			PlayerPrefs.SetInt("SelectedPlayer",0);
		}

		if(!PlayerPrefs.HasKey("TotalCoins")){
			PlayerPrefs.SetInt("TotalCoins",0);
		}

		if(!PlayerPrefs.HasKey("IsAdsRemoved")){
			PlayerPrefs.SetInt("IsAdsRemoved",0);
		}



		//PlayerPrefs.SetInt("TotalCoins",201);

	}

	public void UnlockAll(){
		for (int i=0; i<m_Players.Length; i++) {
			PlayerPrefs.SetInt("IsPlayerUnlocked"+i,1);
		}
	}

	public void LockAll(){
		for (int i=0; i<m_Players.Length; i++) {
			PlayerPrefs.SetInt("IsPlayerUnlocked"+i,0);
		}
		
	}
	//Sharing

	public void OnFacebook(){
		#if UNITY_ANDROID
		if(IsApplication.Installed("com.facebook.katana")){
			Application.OpenURL(kFBAppLink);
		}
		else{
			Application.OpenURL(kFBPageLink);

		}
		#elif UNITY_IOS
		if(IsApplication.Installed(kFBAppLink)){
			Application.OpenURL(kFBAppLink);
		}
		else{
			Application.OpenURL(kFBPageLink);

		}
		#endif
	}

	public void OnRate(){
		#if UNITY_ANDROID
		Application.OpenURL("market://details?id="+kGooglePlayId);
		#elif UNITY_IOS
		Application.OpenURL("itms-apps://itunes.apple.com/app/id"+kAppStoreId);
		#endif	
	}

	public void OnRemoveAds(){
		Debug.Log("onRemoveAds.");
		//Purchaser.Instance.BuyNonConsumable ();

	}

	public void OnRestorePurchases(){
		Debug.Log("OnRestorePurchases.");
		//Purchaser.Instance.RestorePurchases ();

	}

	public void OnShare(){
		NativeShare ns = new NativeShare ();
		string storeUrl = "http://www.burakbayer.com";
		#if UNITY_IOS
		storeUrl = kAppStoreURL;
		#elif UNITY_ANDROID
		storeUrl = kGooglePlayURL;
		#endif
		kShareText = string.Format(kShareText,GameManager.Instance.player.m_Score, Application.productName);
		Debug.Log(kShareText);
		try {
			ns.Share (kShareText, Application.persistentDataPath + "/gameover.png", storeUrl, Application.productName);

		} catch (System.Exception ex) {
			
		}
	}

	public void OnLeaderboard(){
		Social.ShowLeaderboardUI ();
	}

	public void OnAchievements(){
		Social.ShowAchievementsUI();

	}

	public void ReportScore(int score){
		try {
			Social.ReportScore(score, kLeaderboardName,null);

		} catch (System.Exception ex) {
			Debug.LogError("cannot report issue")	;
		}
	}

	void OnUserAuthenticate(bool success){
		if(success){
			Debug.Log("User authentication success");
		}
		else{
			Debug.Log("Cannot authenticate user");

		}

	}

	public void OnSuccessfullIAP(string productID){
		PlayerPrefs.SetInt ("IsAdsRemoved", 1);
		PublishEvent("hide_banner_ad");
	}

	public void OnFailedIAP(string productID, string reason=""){
		Debug.Log ("GameManager: " + productID + " failed with reason:" + reason);
	}

	public void OnHeyzapTestSuit(){
		PublishEvent("show_hz_test_suite");
	}

	public void PublishEvent(string name , object value=null){

		if(name=="gamestate"){
			m_GameState=value.ToString();
			if(m_GameState=="run"){
			}
			else if(m_GameState=="over"){
				player.SaveData();
				PublishEvent("show_interstitial_ad");
			}
			else if(m_GameState=="mainmenu"){
				if(selectedPlayer){
					Destroy(selectedPlayer.gameObject);
				}
				selectedPlayer = Instantiate(m_Players[PlayerPrefs.GetInt("SelectedPlayer",0)]);
				//player = ply.GetComponent<PlayerCubes>();
				ColorManager.Instance.ColorTransition ();
			}
		}

		if(OnEvent!=null){
			OnEvent(name,value);
		}

		Debug.Log("New Event:["+name+"] with param:"+value);
	}


	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
			Debug.Log("The Facebook SDK Initialized");
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	void OnApplicationPause (bool pauseStatus)
	{
		// Check the pauseStatus to see if we are in the foreground
		// or background
		if (!pauseStatus) {
			//app resume
			if (FB.IsInitialized) {
				FB.ActivateApp();
			} else {
				//Handle FB.Init
				FB.Init(InitCallback);
			}
		}
	}
		
}

