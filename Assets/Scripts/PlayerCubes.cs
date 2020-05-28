using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class PlayerCubes : MonoBehaviour {
	enum Direction{
		FORWARD,
		RIGHT
	}

	enum Action{
		JUMP,
		TURN,
		DONOTHING
	}

	private Rigidbody m_RigidBody;
	public float m_Speed;
	public float m_StartSpeed;
	public float m_TurnDuration;
	public int m_Score;
	public int m_GatheredCoins;
	public Vector3 m_StartPosition;
	public ParticleSystem m_ExplosionParticles;
	public ParticleSystem m_LevelExplosion;
	public string m_PlayerName;
	public bool m_IsRotating;

	public int currentBestScore;
	public int currentTotalScore;
	public int currentGamesSinceBestScore;
	public int currentLevelComplexity;
	public int startLevelComplexity;
	public int totalBonusCollected = 0;
	public int currentLevel;

	private Direction direction;
	private float m_TurnStep;
	private Coroutine m_TurnCoroutine;
	private Coroutine m_ClickCoroutine;
	private Transform m_Model;
	private bool isJumping;
	private float startTime;
	private int touchCount;
	private int totalCoins;
	private bool isTurning;
	private bool shouldJump;
	private Transform shadow;
	private float jumpForce;
	private Vector3 jumpVector;
	public float m_ReduceTime=0.3f;
	struct LevelComplexity{
		public float speedOffset;
		public float jumpForce;
		public float turnDuration;
		public int suggestedLevel;
	}
	bool firstTimeOMC;
	LevelComplexity[] levelComplexities = new LevelComplexity[12];
	public int[] tileSwitchArray;
	void Awake(){
		m_RigidBody = GetComponent<Rigidbody>();
		m_Model = transform.Find ("model");
		shadow = transform.Find ("shadow");
		shadow.transform.position = new Vector3(transform.position.x,transform.position.y-0.15f,transform.position.z);

		//Score 0
		levelComplexities[0].speedOffset = .3f;
		levelComplexities[0].jumpForce = 500;
		levelComplexities[0].turnDuration = 0.3f;
		levelComplexities[0].suggestedLevel = 0;
		//5
		levelComplexities[1].speedOffset = .6f;
		levelComplexities[1].jumpForce = 500;
		levelComplexities[1].turnDuration = 0.3f;
		levelComplexities[1].suggestedLevel = 1;
		//Score 15
		levelComplexities[2].speedOffset = .8f;
		levelComplexities[2].jumpForce = 500;
		levelComplexities[2].turnDuration = 0.25f;
		levelComplexities[2].suggestedLevel = 2;
		//25
		levelComplexities[3].speedOffset = 1f;
		levelComplexities[3].jumpForce = 500;
		levelComplexities[3].turnDuration = 0.20f;
		levelComplexities[3].suggestedLevel = 3;
		//35
		levelComplexities[4].speedOffset = 1.3f;
		levelComplexities[4].jumpForce = 500;
		levelComplexities[4].turnDuration = 0.15f;
		levelComplexities[4].suggestedLevel = 4;

		//50
		levelComplexities[5].speedOffset = 1.6f;
		levelComplexities[5].jumpForce = 450;
		levelComplexities[5].turnDuration = 0.18f;
		levelComplexities[5].suggestedLevel = 5;
		for(int i=6;i<12;i++){
			levelComplexities[i].speedOffset = 1.6f + 0.3f *(i-5);;
			levelComplexities[i].jumpForce = 450;
			levelComplexities[i].turnDuration = 0.18f - 0.02f * (i-5);
			levelComplexities[i].suggestedLevel = i;
		}

	}
	// Use this for initialization
	void Start () {
		Reset ();
	}

	void OnEnable(){
		GameManager.OnEvent += GameManager_OnEvent;
	}

	void GameManager_OnEvent (string name, object value)
	{
		if(name == "gamestate"){
			string state = value.ToString();
			if(state == "mainmenu")
				Reset();
			if(state == "run")
				onGameStart();
		}
	}

	void OnDisable(){
		GameManager.OnEvent -= GameManager_OnEvent;
	}

	void Reset(){
		transform.position = GameManager.Instance.startPosition;
		direction = Direction.FORWARD;
		m_Score = 0;
		m_StartSpeed = 5f;//8f
		m_GatheredCoins = 0;
		GameManager.Instance.PublishEvent("score",0);
		//m_GatheredCoins = PlayerPrefs.GetInt("TotalCoins");
		m_RigidBody.isKinematic = false;
		m_RigidBody.mass = 1;
		totalCoins=PlayerPrefs.GetInt("TotalCoins");
		GameManager.Instance.PublishEvent("coin",totalCoins);
		currentBestScore = bestScore;
		currentTotalScore = totalScore;
		Debug.Log("Total score:"+currentTotalScore);
		currentGamesSinceBestScore = gamesSinceBestScore;
	
		m_Model.gameObject.SetActive (true);
		transform.rotation=Quaternion.identity;
		if(gameObject.GetComponentInChildren<ParticleSystem>()){
			Destroy(gameObject.GetComponentInChildren<ParticleSystem>().gameObject);

		}
		m_RigidBody.MoveRotation(Quaternion.identity);
		totalBonusCollected=0;
		CancelInvoke("ReduceBonusCollected");
		CancelInvoke("ScoreCounter");
		firstTimeOMC=false;
		currentLevel=CalculateCurrentLevel(currentTotalScore);
		CalculateLevelData();
	}

	void ReduceBonusCollected(){
		updateTotalBonus(-1);
	}

	void updateTotalBonus(int offset){
		totalBonusCollected = Mathf.Max(0,totalBonusCollected+offset);
		GameManager.Instance.PublishEvent("bonus_collected_count",totalBonusCollected);
		//int amount = 2+ (totalBonusCollected/5) * 2;
		int amount=1;
		GameManager.Instance.PublishEvent("bonus_amount",amount);
		if(totalBonusCollected > 0 && totalBonusCollected % 5 == 0){
			totalBonusCollected=0;
			GameManager.Instance.PublishEvent("message_start","BONUS +1");
			ScoreCounter();
		}
		if(offset > 0){
			CancelInvoke("ReduceBonusCollected");
			InvokeRepeating("ReduceBonusCollected",m_ReduceTime,m_ReduceTime);
		}
	}
	// Update is called once per frame
	void Update () {

		if (GameManager.Instance.m_GameState == "run") {
			if (Input.GetMouseButtonDown (0) || Input.GetKeyUp(KeyCode.Space)) {
				//TileBasedImplementation2 ();
				Turn2();
			}

			if(isJumping){
				float sc = Mathf.Lerp(.25f,0f,(transform.position.y - .3f)*1.33f);
				shadow.localScale = new Vector3(sc,sc,sc);

			}
			else{
				shadow.localScale = new Vector3(.25f,.25f,.25f);
			}

		}
	}

	void UpdateShadow(){
		RaycastHit hit;
		if(Time.renderedFrameCount % 10 != 0 ){
			return;
		}
		if(Physics.Raycast(transform.position, -Vector3.up, out hit, 2.0f)){
			shadow.transform.position = hit.point;
		}
	}

	void FixedUpdate(){
		if( GameManager.Instance.m_GameState == "run"){
			Move2();
			CheckIfGameOver ();
		}

	}

	bool ShouldJump(){
		RaycastHit hit;
		if(Physics.Raycast(transform.position, -Vector3.up, out hit, 5.0f)){

			Transform collidedTile = hit.collider.transform.parent;
			int collidedNdx = collidedTile.GetSiblingIndex();
			//print("found:" + collidedTile.name + " with ndx "+collidedNdx);
			//hit.collider.GetComponent<Renderer>().material.SetColor("_MainColor",Color.red);
			Transform startTile = collidedTile.parent;
			if(collidedTile.name != "corner"){
				for(int i=collidedNdx;i<startTile.childCount;i++){
					Transform t = startTile.GetChild(i);
					if(t.name == "corner"){
						return false;
					}
					else if(t.name == "empty"){
						if(direction == Direction.RIGHT && collidedTile.name == "right" ){
							return false;
						}
						if(direction == Direction.FORWARD && collidedTile.name == "up"){
							return false;
						}
						return true;
					}
				}
			}else{
				return false;
			}
		}
		return false;
	}


	bool ShouldJump2(){

		Transform collidedTile = TileManagerCubes.Instance.ClosestTile(m_RigidBody.transform.position);
		int collidedNdx = collidedTile.GetSiblingIndex();
		//print("found:" + collidedTile.name + " with ndx "+collidedNdx);
		//collidedTile.GetChild(0).GetComponent<Renderer>().material.SetColor("_MainColor",Color.red);
		Transform startTile = collidedTile.parent;
		if(collidedTile.name != "c"){
			if(collidedTile.name == "e"){

			}
			for(int i=collidedNdx+1;i<startTile.childCount;i++){
				Transform t = startTile.GetChild(i);
				if(t.name == "c"){
					return false;
				}
				else if(t.name == "e"){
					if(direction == Direction.RIGHT && collidedTile.name == "r" ){
						return false;
					}
					if(direction == Direction.FORWARD && collidedTile.name == "u"){
						return false;
					}
					return true;
				}
			}
		}

		return false;

	}


	Action DecideAction(){

		Transform collidedTile = TileManagerCubes.Instance.ClosestTile(m_RigidBody.transform.position);
		int collidedNdx = collidedTile.GetSiblingIndex();
		//print("found:" + collidedTile.name + " with ndx "+collidedNdx);
		//collidedTile.GetChild(0).GetComponent<Renderer>().material.SetColor("_MainColor",Color.red);
		Transform startTile = collidedTile.parent;
		if(collidedTile.name != "c"){
			if(collidedTile.name == "e"){
				if(direction == Direction.RIGHT && m_RigidBody.transform.position.z>collidedTile.position.z){
					goto label1;
				}

				if(direction == Direction.FORWARD && m_RigidBody.transform.position.x<collidedTile.position.x){
					goto label1;
				}
				return Action.DONOTHING;
			}
			label1:
			for(int i=collidedNdx+1;i<startTile.childCount;i++){
				Transform t = startTile.GetChild(i);
				if(t.name == "c"){
					return Action.TURN;
				}
				else if(t.name == "e"){
					if(direction == Direction.RIGHT && collidedTile.name == "r" ){
						return Action.TURN;
					}
					if(direction == Direction.FORWARD && collidedTile.name == "u"){
						return Action.TURN;
					}
					return Action.JUMP;
				}
			}
		}

		return Action.TURN;

	}

	void TileBasedImplementation2(){
		Action action = DecideAction ();

		switch (action) {
		case Action.JUMP:
			Jump2 ();
			break;
		case Action.TURN:
			Turn2 ();
			break;
		default:
			break;
		}

		//Score3();
	}

	void Move2(){
		Vector3 pos = m_RigidBody.position + ( m_RigidBody.transform.right * m_Speed * Time.deltaTime *-1);
		m_RigidBody.MovePosition ( pos);
		//Score2 ();		
	}

	private void Jump2(){
		m_RigidBody.velocity = Vector3.zero;
		m_RigidBody.angularVelocity = Vector3.zero;
		m_RigidBody.AddForce (Vector3.up * jumpForce,ForceMode.Force);
		isJumping=true;
		SoundManager.Instance.Jump ();
		//StartCoroutine(UpdateJumpVector());


	}

	IEnumerator UpdateJumpVector(){
		//jumpVector = new Vector3(0,9f,0);
		//yield return new WaitForSeconds(.15f);
		//jumpVector = Vector3.zero;
		float t = 0;
		float inTime = .3f;
		Vector3 start = new Vector3(0,m_RigidBody.transform.position.y,0);
		Vector3 maxPos = new Vector3(0,m_RigidBody.transform.position.y+1.5f,0);

		while (t<=1.0f) {
			t += Time.deltaTime/inTime;
			jumpVector = Vector3.Lerp(start, maxPos, Mathf.Sin(t*Mathf.PI/2f));
			//jumpVector = Vector3.Lerp(start, maxPos, Mathf.SmoothStep(0,1,t));
			//jumpVector = jumpVector * Time.fixedDeltaTime;
			yield return null;
		}
		t = 0;
		while (t<=1.0f) {
			t += Time.deltaTime/inTime;
			jumpVector = Vector3.Lerp( maxPos ,Vector3.zero, 1f - Mathf.Cos(t*Mathf.PI/2f));
			//jumpVector = Vector3.Lerp( maxPos ,start, Mathf.SmoothStep(0,1,t));
			//jumpVector = jumpVector * Time.fixedDeltaTime;

			yield return null;
		}
		jumpVector = Vector3.zero;

	}


	private void Turn2(){
		if(m_TurnCoroutine!=null)
			StopCoroutine(m_TurnCoroutine);
		if(direction == Direction.FORWARD){
			direction = Direction.RIGHT;
			m_TurnCoroutine = StartCoroutine(TurnMe(new Vector3(0,90,0),m_TurnDuration));

		}
		else{
			direction = Direction.FORWARD;
			m_TurnCoroutine = StartCoroutine(TurnMe(new Vector3(0,0,0),m_TurnDuration));
		}
		SoundManager.Instance.Turn();
	}

	void Score2(){
		//float magnitude = m_RigidBody.transform.position.magnitude * 0.3f; 
		float magnitude = Time.time - startTime;																																																			
		if (m_Score != (int)magnitude) {
			m_Score = (int)magnitude;
			GameManager.Instance.PublishEvent("score",m_Score);
			CheckSpeedUp();
			if(m_Score % 10 == 0)
				ColorManager.Instance.ColorTransition();
		}

	}

	void OnCollisionEnter(Collision collision) {
		isJumping=false;
	}

	void onGameStart(){
		SetLevelComplexity (0);
		startTime = Time.time;
		SetupLevelComplexityWithBestScore();
		InvokeRepeating("ReduceBonusCollected",0,m_ReduceTime);
		InvokeRepeating("ScoreCounter",0,1f);
	}

	void ScoreCounter(){
		m_Score+=1;
		GameManager.Instance.PublishEvent("score",m_Score);
		CalculateLevelData();

		CheckSpeedUp();
		if(m_Score % 10 == 0)
			ColorManager.Instance.ColorTransition();
	}
	void SetupLevelComplexityWithBestScore(){
		int highScore = bestScore;
		if( highScore > 110){
			SetLevelComplexity (6);		
			startLevelComplexity=6;

		}
		else if(highScore > 80){
			SetLevelComplexity (5);		
			startLevelComplexity=5;


		}else if (highScore > 60) {
			SetLevelComplexity (4);	
			startLevelComplexity=4;

		} else if (highScore > 40) {
			SetLevelComplexity (3);
			startLevelComplexity=3;

		} else if (highScore > 20) {
			SetLevelComplexity (2);
			startLevelComplexity=2;
		} else if (highScore > 10) {
			SetLevelComplexity (1);
			startLevelComplexity=1;

		}else{
		startLevelComplexity=0;
		}
	}

	int gamesSinceBestScore{
		get{ 
			return PlayerPrefs.GetInt ("GamesSinceBestScore", 0);
		}
		set{
			PlayerPrefs.SetInt ("GamesSinceBestScore", value);	
		}
	}	

	int bestScore{
		get{ //BestScore
			return PlayerPrefs.GetInt ("BestScore", 0);
		}
		set{ 
			PlayerPrefs.SetInt ("BestScore", value);
		}
	}

	int totalScore{
		get{
			return PlayerPrefs.GetInt("TotalScore",0);
		}
		set{
			PlayerPrefs.SetInt ("TotalScore", value);

		}
	}

	void IncreaseLevelComplexity(){
		SetLevelComplexity (currentLevelComplexity + 1);
	}

	void DecreaseLevelComplexity(){
		SetLevelComplexity (currentLevelComplexity - 1);
	}

	void ActivateRelaxPath(){
		TileManagerCubes.Instance.RelaxMode (10);
	}

	void SetLevelComplexity(int ndx){
		if (ndx < levelComplexities.Length && ndx > -1) {
			m_Speed = m_StartSpeed + levelComplexities [ndx].speedOffset;
			m_TurnDuration = levelComplexities [ndx].turnDuration;
			jumpForce = levelComplexities [ndx].jumpForce;
			TileManagerCubes.Instance.UseRule (levelComplexities [ndx].suggestedLevel);
			currentLevelComplexity = ndx;
		}
	}

	void CheckSpeedUp(){
		bool isInscreased=false;
		string log="";
		if (m_Score == currentBestScore+1 && currentBestScore!=0) {
			//Best score occured during gameplay
			//TODO: play anim
			ActivateRelaxPath();
			GameManager.Instance.BestScore(m_Score);
			SoundManager.Instance.BestScore();
		}
		int multiplier = (currentBestScore/110)+1;
		//int multiplier = 1;
//		for(int i=10*multiplier;i<110*multiplier;i+=10*multiplier){
//			if(i>m_Score)
//				break;
//			if (i == m_Score){
//				IncreaseLevelComplexity();
//				isInscreased=true;
//				log="Inscrease+";
//				break;
//
//			}
//		}
		int lastComplexity=currentLevelComplexity;
		SetLevelComplexity(m_Score/(10*multiplier)+startLevelComplexity);
		isInscreased = lastComplexity<currentLevelComplexity;
		if(isInscreased){
			if(currentBestScore > 40 && currentBestScore - m_Score<15 && m_Score<currentBestScore){
				DecreaseLevelComplexity();
				log+=" ======> decrease close to best ";
			}
			if(currentGamesSinceBestScore == 10 && currentBestScore/10 < 6){
				DecreaseLevelComplexity();
				log+=" =======> decrease no best score";
			}
		}

		if(m_Score==5){
			if( Random.Range(1,3) == 1){
				TileManagerCubes.Instance.UseRuleOnes(12);
			}
		}
		if(currentBestScore < 20 && m_Score==20){
			if( Random.Range(1,3) == 1){
				TileManagerCubes.Instance.UseRuleOnes(12);
			}
		}
		else if(currentBestScore < 30 && m_Score == 30){
			if( Random.Range(1,3) == 1){
				TileManagerCubes.Instance.UseRuleOnes(12);
			}
		} 
		else if(currentBestScore < 50 && m_Score == 50){
			if( Random.Range(1,3) == 1){
				TileManagerCubes.Instance.UseRuleOnes(12);
			}
		} 
		else if(currentBestScore < 70 && m_Score == 70){
			if( Random.Range(1,3) == 1){
				TileManagerCubes.Instance.UseRuleOnes(12);
			}
		} 
		else if(currentBestScore < 90 && m_Score == 90){
			if( Random.Range(1,3) == 1){
				TileManagerCubes.Instance.UseRuleOnes(12);
			}
		} 
		else{
			if(Random.Range(1,10)==1){
				TileManagerCubes.Instance.UseRuleOnes(12);

			}
		}

		Debug.LogWarning("Current level comp:"+currentLevelComplexity + " "+log);
	}



	IEnumerator TurnMe(Vector3 toAngle, float inTime) {
		var fromAngle = m_RigidBody.transform.rotation;
		isTurning = true;
		float t = 0;
		while (t!=1) {
			t += Time.deltaTime/inTime;
			if(t>1)
				t=1;
			m_RigidBody.MoveRotation(Quaternion.Lerp(fromAngle, Quaternion.Euler(toAngle), t));
			yield return null;
		}
		isTurning = false;
	}


	//Service functions
	void OnTriggerEnter(Collider other){
		if (GameManager.Instance.m_GameState != "run")
			return;
		////Debug.Log ("On Trigger Enter :" + other.ToString());

		if (other.tag == "Coin") {
			Coin coin = other.GetComponent<Coin>();
			coin.Collect ();
			m_GatheredCoins++;
			GameManager.Instance.PublishEvent("coin",m_GatheredCoins+totalCoins);
			SoundManager.Instance.Coin();

		} else if (other.tag== "Jump"){
			Jump2();
		}else if(other.tag == "Bonus"){
			Destroy(other.gameObject);
			updateTotalBonus(+1);
		}
	}

	IEnumerator CaptureScreenAndFinishGame(){
		//yield return new WaitForEndOfFrame ();
		//		Texture2D text = new Texture2D (Screen.width, Screen.height,TextureFormat.RGB24,false);
		//		text.ReadPixels (new Rect (0, 0, Screen.width, Screen.height) ,0,0);
		//		text.Apply ();
		//		yield return 0;
		//		byte[] data = text.EncodeToPNG ();
		//		yield return 0;
		//		File.WriteAllBytes (Application.persistentDataPath + "/gameover.png", data);
		//		Debug.Log (Application.persistentDataPath + "/gameover.png");
		//		Object.Destroy (text);
		ScreenCapture.CaptureScreenshot("gameover.png");
	
		yield return 0;
	}

	void CheckIfGameOver(){

		if (m_RigidBody.transform.position.y < 0 /*|| m_RigidBody.transform.position.y > 3*/ && GameManager.Instance.m_GameState == "run") {
			DecideForOneMoreChance();
		}	
	}

	void DecideForOneMoreChance(){
		StopPlayer ();
		//TODO: One more chance

		StartCoroutine (CaptureScreenAndFinishGame ());
		FinishGame ();
		/*
		if (m_Score<bestScore&& !firstTimeOMC){

			GameManager.Instance.PublishEvent("gamestate","show_omc");
			GameManager.Instance.PublishEvent("start_omc");
			firstTimeOMC=true;
		}
		else{
			StartCoroutine (CaptureScreenAndFinishGame());
			FinishGame();
		}
		*/
	}
	void FinishGame(){
		GameManager.Instance.FinishGame ();
	}

	public void ContinueGame(){
		m_Model.gameObject.SetActive (true);
		m_RigidBody.isKinematic = false;
		transform.position=GameManager.Instance.tileManager.m_lastAddedPos+
			GameManager.Instance.startPosition+new Vector3 (-2, 0, 0);
		Debug.Log("start:"+transform.position);
		GameManager.Instance.tileManager.SoftReset();
		m_RigidBody.MoveRotation(Quaternion.identity);
		transform.rotation=Quaternion.identity;
		direction=Direction.FORWARD;

	}

	void StopPlayer(){
		if (m_TurnCoroutine != null){
			StopCoroutine (m_TurnCoroutine);
		}
		m_Model.gameObject.SetActive (false);
		SoundManager.Instance.Crash();
		CancelInvoke("ReduceBonusCollected");
		CancelInvoke("ScoreCounter");
		m_RigidBody.isKinematic = true;
		ParticleSystem s = Instantiate (m_ExplosionParticles, m_RigidBody.transform.position, Quaternion.identity) as ParticleSystem;
		s.transform.parent = transform;
	}

	public void SaveData(){
		PlayerPrefs.SetInt("TotalCoins", totalCoins+m_GatheredCoins);
		int selected = PlayerPrefs.GetInt ("SelectedPlayer", 0);
		int playerHighScore = PlayerPrefs.GetInt ("PlayerBestScore" + selected, 0);
		if (bestScore < m_Score) {
			bestScore =  m_Score;
			gamesSinceBestScore = 0;
		} else {
			gamesSinceBestScore++;
		}

		if (playerHighScore < m_Score) {
			PlayerPrefs.SetInt("PlayerBestScore" + selected, m_Score);
		}
		totalScore=currentTotalScore+m_Score;
		GameManager.Instance.ReportScore (m_Score);
		PlayerPrefs.Save ();
	}

	int CalculateCurrentLevel(int score){
		return Mathf.Max(0,Mathf.CeilToInt(Mathf.Log(score/100.0f,2)));
	}

	void CalculateLevelData(){
		int score = currentTotalScore+m_Score;
		LevelBar.LevelBarValues data=new LevelBar.LevelBarValues();
		data.level = CalculateCurrentLevel(score);
		if(currentLevel!=data.level){
			GameManager.Instance.PublishEvent("level_achieved");
			SoundManager.Instance.Reward();
			ParticleSystem s = Instantiate (m_LevelExplosion, m_RigidBody.transform.position, Quaternion.identity) as ParticleSystem;
			s.transform.parent = transform;

		}
		currentLevel =data.level;
		if(data.level==0){
			data.minVal = 0;
			data.maxVal = 100;
		}
		else{
			data.minVal = (int)Mathf.Pow(2,Mathf.Max(0,data.level-1))*100;
			data.maxVal = (int)Mathf.Pow(2,data.level)*100;
		}
		data.val=score;

		GameManager.Instance.tileManager.SetSelectedTile(data.level);
		GameManager.Instance.PublishEvent("level_bar_update",data);
	}
}