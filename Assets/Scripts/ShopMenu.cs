using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour {

	void Awake(){
		GameManager.OnEvent+= GameManager_OnEvent;


	}

	void GameManager_OnEvent (string name, object value)
	{
		if(name == "gamestate"){
			bool cond= (name == "gamestate" && value.ToString()=="shopmenu");
			gameObject.SetActive (cond);
		}
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
		
	public void OnUnlock(int id){
		Debug.Log("Unlock clicked");
	}

	public void OnClose(){
		GameManager.Instance.MainMenu();
	}

	public void OnRestorePurchases(){
		
	}
}
