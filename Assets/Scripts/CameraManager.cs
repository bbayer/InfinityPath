using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	// Use this for initialization

	private Camera m_Camera;
	public float m_DampTime;
	private Vector3 m_MoveVel;
	public float m_FOVUpdateTime;
	public float m_InGameFOV;
	public float m_GameOverFOV;
	[HideInInspector]
	public float m_InMenuFOV;
	private Coroutine m_AdjustCoroutine;

	void Awake(){
		m_Camera = GetComponentInChildren<Camera>();
		transform.position = Vector3.zero;
		m_InMenuFOV = m_Camera.fieldOfView;
	}

	void Start () {
	
	}

	void OnEnable(){	
		GameManager.OnEvent += GameManager_OnEvent;

	}

	void GameManager_OnEvent (string name, object value)
	{
		string val="";
		if(value!=null)
			val=value.ToString();
		if(name == "gamestate"){
			switch (val) {
			case "run":
				GameStart();

				break;
			case "over":
				GameOver();

				break;
			case "mainmenu":
				GameMainMenu();
				break;
			default:
				break;
			}
		}
	}
	
	void OnDestroy(){
		GameManager.OnEvent -= GameManager_OnEvent;
	}

	// Update is called once per frame
	void FixedUpdate () {

		Vector3 playerPos = GameManager.Instance.player.transform.position;
		//transform.position = playerPos;
		//transform.position = new Vector3 (playerPos.x, 0.82f, playerPos.z);
		transform.position = Vector3.SmoothDamp(transform.position, new Vector3 (playerPos.x, 0.82f, playerPos.z), ref m_MoveVel, .2f);

	}

	void GameStart(){
		//Debug.Log("CAMERA: Game Started");
		if(m_AdjustCoroutine!=null)
			StopCoroutine ( m_AdjustCoroutine );
		m_AdjustCoroutine = StartCoroutine(AdjustFOV (m_InGameFOV, m_FOVUpdateTime));
	}

	void GameOver(){
		if(m_AdjustCoroutine != null)
			StopCoroutine (m_AdjustCoroutine);
		m_AdjustCoroutine = StartCoroutine(AdjustFOV (m_GameOverFOV, m_FOVUpdateTime));
	}

	void GameMainMenu(){
		if(m_AdjustCoroutine != null)
			StopCoroutine (m_AdjustCoroutine);
		m_AdjustCoroutine = StartCoroutine(AdjustFOV (m_InMenuFOV, m_FOVUpdateTime));
	}

	IEnumerator AdjustFOV(float toFOV, float inTime) {
		float fromFOV = m_Camera.fieldOfView;
		//float velocity=0;
		float t = 0f;
		while (t!=1) {
			t += Time.deltaTime/inTime;
			if(t>1)
				t=1;
			m_Camera.fieldOfView = Mathf.Lerp(fromFOV,toFOV,t*t);
			yield return null;
		}
	}
}
