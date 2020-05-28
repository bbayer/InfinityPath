using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public class ScrollView : MonoBehaviour {

	// Use this for initialization
	[Serializable]
	public struct UnlockableButtons{
		public string name;
		public int price;
		public Sprite sprite;
	}

	public UnlockableButtons[] m_ButtonList;

	public GameObject m_ButtonPrefab;
	public Text m_CoinText;

	private GameObject m_Content;
		
	void OnEnable () {
		m_Content = transform.GetChild(0).gameObject;
		foreach(Transform t in m_Content.transform){
			Destroy(t.gameObject);
		}

		Debug.Log("Shop menu start");
		int ndx=0;
		int selectedPlayerNdx = PlayerPrefs.GetInt("SelectedPlayer",0);

		foreach (UnlockableButtons item in m_ButtonList) {
			GameObject g = Instantiate( m_ButtonPrefab);
			g.transform.SetParent (m_Content.transform, false);
			RectTransform rectTransform = g.GetComponent<RectTransform>();
			Button button = g.GetComponent<Button>();
			Text[] texts = g.GetComponentsInChildren<Text>();


			GameObject imageg = g.transform.Find("Image").gameObject;
			Image image=imageg.GetComponent<Image>();
			rectTransform.localScale = Vector3.one;
			rectTransform.offsetMin = new Vector2(0,0);
			rectTransform.offsetMax = new Vector2(0,250);
			rectTransform.anchoredPosition = new Vector2(0, -250 * ndx);


			int isUnlocked = PlayerPrefs.GetInt("IsPlayerUnlocked"+ndx,0);

			texts[0].text = item.name;

			if(isUnlocked == 1||item.price==0){
				texts[1].text = "TAP TO\nSELECT";
			}
			else{
				texts[1].text = "UNLOCK\n" + item.price + "C";
			}

			int captured=ndx;
			button.onClick.AddListener( ()=> {OnButtonClick(captured);} );
			image.sprite = item.sprite;
			SetIsSelected(g,false);

			if(selectedPlayerNdx == ndx){
				SetIsSelected(g,true);

			}

			ndx++;


		}

		m_Content.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		m_Content.GetComponent<RectTransform>().sizeDelta = new Vector2(0,250*m_ButtonList.Length);

		UpdateTotalCoinText ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnButtonClick(int id){

		int isUnlocked = PlayerPrefs.GetInt("IsPlayerUnlocked"+id,0);
		SoundManager.Instance.Click();
		if(isUnlocked == 1){
			SelectPlayer(id);
		}
		else{
			UnlockPlayer(id);
		}

	}

	void SelectPlayer(int id){
		PlayerPrefs.SetInt("SelectedPlayer",id);
		//PlayerPrefs.SetInt("SelectedPlayer",0);
		UnselectAll();
		SetIsSelected(m_Content.transform.GetChild(id).gameObject,true);
		GameManager.Instance.MainMenu();


	}

	void UnselectAll(){
		foreach (Transform btn in m_Content.transform) {
			SetIsSelected(btn.gameObject,false);
		}
	}

	void SetIsSelected(GameObject btn, bool selected){
		GameObject isselected = btn.transform.Find("isSelected").gameObject;
		Color c;
		if(selected){
			c = new Color(1,1,1,.5f);
		}
		else{
			c = new Color(1,1,1,0);

		}
		isselected.GetComponent<Image>().color = c;
	}


	void UnlockPlayer(int id){
		GameObject selected = m_Content.transform.GetChild(id).gameObject;
		int price = m_ButtonList[id].price;
		int totalCoins = PlayerPrefs.GetInt("TotalCoins",0);
		if(totalCoins<price){
			//TODO: Not enough coins
			Text[] texts = selected.GetComponentsInChildren<Text>();
			texts[1].text = "NOT ENOUGH\nCOINS";
			SoundManager.Instance.Error();
		}
		else{
			totalCoins-=price;
			PlayerPrefs.SetInt("TotalCoins",totalCoins);
			PlayerPrefs.SetInt("IsPlayerUnlocked"+id,1);
			SelectPlayer(id);

		}
		UpdateTotalCoinText ();
	}

	void UpdateTotalCoinText(){
		int totalCoins = PlayerPrefs.GetInt("TotalCoins",0);
		m_CoinText.text = string.Format ("YOU HAVE {0} COINS LEFT", totalCoins);
	}
}
