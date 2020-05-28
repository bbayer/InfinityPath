using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/**
 * Class to do a character selection on Unity3D
 * 
 * @author Jefferson Henrique
 * */
public class SelectChar : MonoBehaviour {
	
	// The left marker out of visible scence
	public Transform markerLeft2;
	// The left marker of visible scence
	public Transform markerLeft;
	// The middle marker of visible scence
	public Transform markerMiddle;
	// The right marker of visible scence
	public Transform markerRight;
	// The right marker out of visible scence
	public Transform markerRight2;
	
	// The characters prefabs to pick
	public GameObject[] charsPrefabs;
	
	// The game objects created to be showed on screen
	private GameObject[] chars;

	// The index of the current character
	private int currentChar = 0;
	// The index of Line1,Line2
	public Text m_txtPriceLine;

	public Text m_lblCarName;
	

	// The index of Line1,Line2
	public Text GemScore;
	public Slider m_SpeedSlider;
	public Slider m_TurnSlider;	
	public Button m_btnUnlock;
	public Button m_btnSelect;

	
	struct CarPrefs{
		public float speed;
		public float turnSpeed;
		public bool isUnlocked;
	};

	private List<CarPrefs> carPrefs;
	private float m_SpeedMin;
	private float m_SpeedMax;

	private float m_TurnSpeedMin;
	private float m_TurnSpeedMax;

	void Awake(){
		//PlayerPrefs.SetInt("TotalCoins",200);

	}
	void Start() {

		// We initialize the chars array
		chars = new GameObject[charsPrefabs.Length];
		carPrefs = new List<CarPrefs>();
		// We create game objects based on characters prefabs
		int index = 0;
		foreach (GameObject t in charsPrefabs) {
			chars[index++] = Instantiate(t, markerRight2.position, Quaternion.identity) as GameObject;
		}
		m_SpeedMin = 10000;
		m_TurnSpeedMin = 1000;
		foreach (GameObject c in chars) {
			CarPrefs info;
			Player p = c.GetComponent<Player>();
			p.enabled = false;

			try{
				BoxCollider collider = c.GetComponent<BoxCollider> ();
				collider.enabled = false;
			}catch(System.Exception ex){
				
			}

			c.name = p.m_PlayerName;
			info.speed = p.m_Speed;
			info.turnSpeed = 1/p.m_TurnDuration;
			info.isUnlocked = true;
			Destroy(c.transform.GetChild(0).gameObject);
			Destroy(c.transform.GetChild(1).gameObject);
			c.transform.GetChild(2).gameObject.transform.position = markerRight2.position;
			Destroy(p);
			c.transform.Rotate(new Vector3(0,90,0));
			carPrefs.Add(info);

			if(info.speed> m_SpeedMax){
				m_SpeedMax = info.speed;
			}

			if(info.speed<m_SpeedMin){
				m_SpeedMin = info.speed;
			}


			if(info.turnSpeed> m_TurnSpeedMax){
				m_TurnSpeedMax = info.turnSpeed;
			}
			
			if(info.turnSpeed<m_TurnSpeedMin){
				m_TurnSpeedMin = info.turnSpeed;
			}
		}
		//Debug.Log ("MinSpeed:" + m_SpeedMin + " Max:" + m_SpeedMax);
		AdjustUI ();
	}

	void AdjustUI(){
		m_lblCarName.text = chars [currentChar].name;
		GemScore.text = "" + PlayerPrefs.GetInt ("TotalCoins");
		m_SpeedSlider.value = (carPrefs [currentChar].speed - m_SpeedMin) / (m_SpeedMax - m_SpeedMin);
		m_TurnSlider.value = (carPrefs [currentChar].turnSpeed - m_TurnSpeedMin) / (m_TurnSpeedMax - m_TurnSpeedMin);
		if(PlayerPrefs.GetInt("IsPlayerUnlocked"+currentChar) == 0){
			m_btnSelect.gameObject.SetActive(false);
			m_btnUnlock.gameObject.SetActive(true);
			m_txtPriceLine.text = "UNLOCK WITH 199 COINS";

		}
		else{
			m_btnSelect.gameObject.SetActive(true);
			m_btnUnlock.gameObject.SetActive(false);
			m_txtPriceLine.text = "SELECT YOUR RIDE";
		}
	}
	public void onRightClick(){
		currentChar++;
		if (currentChar == chars.Length) {
			currentChar--;
		}
		AdjustUI ();
	}

	public void onLeftClick(){
		currentChar--;
		if (currentChar < 0) {
			currentChar = 0;
		}
		AdjustUI ();
	}

	public void Unlock(){
		int totalCoins = PlayerPrefs.GetInt("TotalCoins",0);

		if(totalCoins<199){
			m_txtPriceLine.text = "NOT ENOUGH COINS";
		}
		else{
			totalCoins-=199;
			PlayerPrefs.SetInt("TotalCoins",totalCoins);
			PlayerPrefs.SetInt("IsPlayerUnlocked"+currentChar,1);
			AdjustUI();
		}
	}

	public void Select(){
		PlayerPrefs.SetInt("SelectedPlayer",currentChar);
		SceneManager.LoadScene("Main");

	}

	void Update() {
	
		// Shows a label with the name of the selected character
		GameObject selectedChar = chars[currentChar];
		// The index of the middle character
		int middleIndex = currentChar;	
		// The index of the left character
		int leftIndex = currentChar - 1;
		// The index of the right character
		int rightIndex = currentChar + 1;

		// For each character we set the position based on the current index

		for (int index = 0; index < chars.Length; index++) {
			Transform transf = chars[index].transform;

			// If the index is less than left index, the character will dissapear in the left side
			if (index < leftIndex) {
				transf.position = Vector3.Lerp(transf.position, markerLeft2.position, Time.deltaTime);
				transf.localScale= Vector3.Lerp(transf.localScale, markerLeft2.localScale, Time.deltaTime);

				// If the index is less than right index, the character will dissapear in the right side
			} else if (index > rightIndex) {
				transf.position = Vector3.Lerp(transf.position, markerRight2.position, Time.deltaTime);
				transf.localScale= Vector3.Lerp(transf.localScale, markerRight2.localScale, Time.deltaTime);
				
				// If the index is equals to left index, the character will move to the left visible marker
			} else if (index == leftIndex) {
				transf.position = Vector3.Lerp(transf.position, markerLeft.position, Time.deltaTime);
				transf.localScale= Vector3.Lerp(transf.localScale, markerLeft.localScale, Time.deltaTime);
				transf.rotation = Quaternion.Lerp(transf.rotation,  Quaternion.Euler(new Vector3(0,135,0)), Time.deltaTime);
				
				// If the index is equals to middle index, the character will move to the middle visible marker
			} else if (index == middleIndex) {
				transf.position = Vector3.Lerp(transf.position, markerMiddle.position, Time.deltaTime);
				transf.localScale= Vector3.Lerp(transf.localScale, markerMiddle.localScale, Time.deltaTime);
				transf.rotation = Quaternion.Lerp(transf.rotation, transf.rotation * Quaternion.Euler(new Vector3(0,60,0)), Time.deltaTime);
				// If the index is equals to right index, the character will move to the right visible marker
			} else if (index == rightIndex) {
				transf.position = Vector3.Lerp(transf.position, markerRight.position, Time.deltaTime);
				transf.localScale= Vector3.Lerp(transf.localScale, markerRight.localScale, Time.deltaTime);
				transf.rotation = Quaternion.Lerp(transf.rotation,  Quaternion.Euler(new Vector3(0,135,0)), Time.deltaTime);
			}
		}


	
	}
	
}