using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	public float m_RotationSpeed;
	private Transform m_coin;

	void Awake(){
		m_coin = transform.GetChild (0);
	}
	// Use this for initialization
	void Start () {
		iTween.RotateBy (m_coin.gameObject, iTween.Hash ( 
			"z", 5,
			"time",10,
			"looptype", iTween.LoopType.loop,
			"easetype",iTween.EaseType.linear
			));
	
	}
	
	// Update is called once per frame
	void Update () {


	}

	public void Collect(){
		iTween.MoveTo (m_coin.gameObject, iTween.Hash ( 
			"position", new Vector3(m_coin.position.x, 4f, m_coin.position.z), 
			"time",1,
			"oncomplete", "DestroyAll", 
			"oncompletetarget",gameObject,
			"easetype",iTween.EaseType.easeOutBounce
			));

		iTween.ScaleTo (m_coin.gameObject, iTween.Hash ( 
			"scale", Vector3.zero, 
			"time",1,
			"easetype",iTween.EaseType.easeInCirc
		));
	}

	public void DestroyAll(){
		Destroy (gameObject);
	}
}
