using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Player : MonoBehaviour {
	enum Direction{
		FORWARD,
		RIGHT
	}
	private Rigidbody m_RigidBody;
	public float m_Speed;
	public float m_TurnDuration;
	public int m_NumberOfTurns;
	public int m_GatheredCoins;
	public Vector3 m_StartPosition;
	public ParticleSystem m_ExhaustParticles;
	public ParticleSystem m_ExplosionParticles;
	public string m_PlayerName;
	public bool m_IsRotating;

	[HideInInspector]
	public float m_Distance;

	private Direction direction;
	private float m_TurnStep;
	private Coroutine m_TurnCoroutine;
	private Transform m_Sphere;
	/*

	void Awake(){
		m_RigidBody = GetComponent<Rigidbody>();
		m_Sphere = transform.FindChild ("Sphere");

	}
	// Use this for initialization
	void Start () {
		Reset ();
	}

	void OnEnable(){
		GameManager.OnGameMainMenu += Reset;
		GameManager.OnGameRun += onGameStart;
	}

	void OnDisable(){
		GameManager.OnGameMainMenu -= Reset;
		GameManager.OnGameRun -= onGameStart;
	}

	void Reset(){
		m_RigidBody.MovePosition(m_StartPosition);

		direction = Direction.FORWARD;
		m_Distance = 0;
		//m_RigidBody.MoveRotation (Quaternion.Euler (new Vector3 (0, 90, 0)));
		m_TurnCoroutine = StartCoroutine(TurnMe(new Vector3(0,90,0), m_TurnDuration ) );
		////Debug.Log ("Reseting position");
		m_ExhaustParticles.gameObject.SetActive(false);

		m_NumberOfTurns = 0;
		GameManager.Instance.lblScore.text = "0";
		m_GatheredCoins = PlayerPrefs.GetInt("TotalCoins");
		GameManager.Instance.lblCoins.text = "" + m_GatheredCoins;
		m_ExplosionParticles.Stop ();
		m_ExplosionParticles.gameObject.SetActive (false);
			

	}
	// Update is called once per frame
	void Update () {
		if( GameManager.Instance.m_GameState == GameManager.GameState.GameStateRun){
			if(Input.GetMouseButtonDown(0) ){
				if(m_TurnCoroutine!=null)
					StopCoroutine(m_TurnCoroutine);
				if(direction == Direction.FORWARD){
					direction = Direction.RIGHT;
					m_TurnCoroutine = StartCoroutine(TurnMe(new Vector3(0,180,0),m_TurnDuration));
				}
				else{
					direction = Direction.FORWARD;
					m_TurnCoroutine = StartCoroutine(TurnMe(new Vector3(0,90,0), m_TurnDuration ) );
					
				}

			}

		}
	}

	void FixedUpdate(){
		Move();
	}

	void Move(){
		if (GameManager.Instance.m_GameState == GameManager.GameState.GameStateRun) {
			Vector3 movement = transform.forward * m_Speed * Time.deltaTime ;
			m_RigidBody.MovePosition (m_RigidBody.position + movement);
			m_Distance+=movement.magnitude*5.0f;

			if(m_IsRotating){
				Vector3 rot = new Vector3 (m_Speed * 100 * Time.deltaTime,0, 0  );
				m_Sphere.Rotate (rot);

			}
		}

	}


	IEnumerator TurnMe(Vector3 toAngle, float inTime) {
		var fromAngle = m_RigidBody.transform.rotation;
		float t = 0;
		while (t!=1) {
			t += Time.deltaTime/inTime;
			if(t>1)
				t=1;
			m_RigidBody.MoveRotation(Quaternion.Lerp(fromAngle, Quaternion.Euler(toAngle), t));
			yield return null;
		}
	}

	void OnTriggerEnter(Collider other){
		if (GameManager.Instance.m_GameState != GameManager.GameState.GameStateRun)
			return;
		////Debug.Log ("On Trigger Enter :" + other.ToString());

		if (other.tag == "Coin") {
			Destroy (other.gameObject);
			m_GatheredCoins++;
			GameManager.Instance.lblCoins.text = ""+m_GatheredCoins;
			SoundManager.Instance.Coin();

		} else 
		if (other.tag == "Score") {
			//m_NumberOfTurns++;
			//lblScore.text = ""+m_NumberOfTurns;
		}
		else
		{
			//Hit to obstacle
			if (m_TurnCoroutine != null){
				StopCoroutine (m_TurnCoroutine);
			}
			SaveData();

			GameManager.Instance.FinishGame ();
			m_ExhaustParticles.gameObject.SetActive(false);
			m_ExplosionParticles.Play();
			m_ExplosionParticles.gameObject.SetActive (true);
			SoundManager.Instance.Crash();

		}
	}

	void SaveData(){
		PlayerPrefs.SetInt("TotalCoins", m_GatheredCoins);
		PlayerPrefs.SetFloat("TotalDistanceTravelled", PlayerPrefs.GetFloat("TotalDistanceTravelled",0) + m_Distance);
		int highScore = PlayerPrefs.GetInt ("BestScore", 0);
		int selected = PlayerPrefs.GetInt ("SelectedPlayer", 0);
		int playerHighScore = PlayerPrefs.GetInt ("PlayerBestScore" + selected, 0);
		if (highScore < m_NumberOfTurns) {
			PlayerPrefs.SetInt("BestScore", m_NumberOfTurns);
		}

		if (playerHighScore < m_NumberOfTurns) {
			PlayerPrefs.SetInt("PlayerBestScore" + selected, m_NumberOfTurns);
		}

		PlayerPrefs.Save ();
	}
	void OnTriggerExit(Collider other){
		if (GameManager.Instance.m_GameState != GameManager.GameState.GameStateRun)
			return;
		////Debug.Log ("On Trigger Exit :" + other.ToString());
		if (other.tag == "Score") {
			m_NumberOfTurns++;
			GameManager.Instance.lblScore.text = ""+m_NumberOfTurns;
			SoundManager.Instance.Score();

			if(m_NumberOfTurns > 10){
				float t = (m_NumberOfTurns - 10) *0.1f;
				m_Speed = Mathf.Lerp(4f,6f,t);
				m_TurnDuration = Mathf.Lerp(0.5f,0.3f,t);
			}
		}
	}

	void onGameStart(){
//		ParticleSystem.EmissionModule em = m_ExhaustParticles.emission;
//		em.enabled = true;
//		m_ExhaustParticles.gameObject.SetActive(true);
//		m_ExhaustParticles.loop=true;
		m_Speed = 4;
		m_TurnDuration = 0.5f;
		m_ExhaustParticles.gameObject.SetActive(true);

	}*/
}
