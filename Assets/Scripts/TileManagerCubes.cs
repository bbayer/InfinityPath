using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BBCommon;

public class TileManagerCubes : Singleton<TileManagerCubes> {

	public GameObject m_TilePrefab;
	public GameObject m_RampPrefab;

	public GameObject m_Coin;
	public GameObject m_startTile;
	public GameObject[] m_envPrefabs;

	private GameObject m_lastAddedTile;
	public Vector3 m_lastAddedPos;
	public float m_DistanceToLastAdded;
	private float m_lastUpdatedDistance;
	private int m_ColorNdx;
	private PathGenerator pathGenerator;


	private float m_RampHeight=1;
	private float m_TileHeight=0;

	public bool m_ScreenshotMode;

	public enum MoveType{
		Jump,
		Turn,
	}

	public int m_relaxMode=0;
	public int currentSelectedTile;
	//private List<GameObject> m_rightTilePool; _MainColor
	//private List<GameObject> m_upTilePool;

	private enum TileDirection
	{
		RightDirection,
		UpDirection,
	}

	void Awake(){
		m_lastAddedTile = m_startTile;
		pathGenerator = new PathGenerator ();
	}

	// Use this for initialization
	void Start () {
	}

	void OnEnable(){
		GameManager.OnEvent += GameManager_OnEvent;

	}

	void GameManager_OnEvent (string name, object value)
	{
		if(name == "gamestate"){
			if(value.ToString() == "mainmenu"){
				Reset();
			}
		}
	}

	void OnDisable(){
		GameManager.OnEvent -= GameManager_OnEvent;

	}

	// Update is called once per frame
	void Update () {
		
		if (GameManager.Instance.m_GameState == "run") {
			InfiniteScroll2 ();
		}
		Transform g = m_startTile.transform.GetChild (0);
		if ( /*g.gameObject.name == "e"||*/g.transform.position.y< -10) {
			Destroy (g.gameObject);	
		}

	}

	void InitializePath(){
		pathGenerator.Reset ("rrrrr");


		//0
		pathGenerator.AddRule (
			new Dictionary<string, string[]> {
				{"r", new string[]{ "uuu", "uuuu"}},
				{"u", new string[]{ "rrr", "rrrr"}}
			}
		);
		//1
		pathGenerator.AddRule (
			new Dictionary<string, string[]> {
				{"r", new string[] { "uu", "uuuyuuu", "uuu"} },
				{"u", new string[] { "rr", "rrrerrr","rrr"} }
			}
		);
		//2
		pathGenerator.AddRule (
			new Dictionary<string, string[]> {
				{"r", new string[] { "uuuu", "uyuu","uuyuu", "uutuu"} },
				{"u", new string[] { "rrrr", "rerrr","rrerrr","rrwrr"} }
			}
		);
		//3
		pathGenerator.AddRule (
			new Dictionary<string, string[]> {
				{"r", new string[] { "uuu", "uuyyuu", "uutuu"} },
				{"u", new string[] { "rrr", "rerrr","rrerrr","rrwrr"} }
			}
		);
		//4
		pathGenerator.AddRule (
			new Dictionary<string, string[]> {
				{"r", new string[] { "uuu", "uyyuu","uuyuu", "uutuu"} },
				{"u", new string[] { "rrr", "rreerr","rrwrr"} }
			}
		);

		//5
		pathGenerator.AddRule (
			new Dictionary<string, string[]> {
				{"r", new string[] { "uuu", "uyyuu","uuyuu", "uutuu","uuruuu"} },
				{"u", new string[] { "rrr", "reerr","rrerr","rrwrr","ruurrr"} }
			}
		);
		//6
		pathGenerator.AddRule (
			new Dictionary<string, string[]> {
				{"r", new string[] { "uu", "uuu", "uyyuu","utu","uutuu"}},
				{"u", new string[] { "rr", "rrr" ,"reerr","rwr","rrwrr"}}
			}
		);
		//7
		pathGenerator.AddRule (
			new Dictionary<string, string[]> {
				{"r", new string[] { "u", "uurrrrr","uu","uyyuu","utuu"} },
				{"u", new string[] { "r", "rruuuuu","rr","reerr","rwrr"} }
			}
		);
		//8
		pathGenerator.AddRule (
			new Dictionary<string, string[]> {
				{"r", new string[] { "u", "uu","uyyu","utuu"} },
				{"u", new string[] { "r", "rr","reer","rwrr"} }
			}
		);
		//9
		pathGenerator.AddRule (
			new Dictionary<string, string[]> {
				{"r", new string[] { "u", "uu","uyy","utuu"} },
				{"u", new string[] { "r", "rr","ree","rwrr"} },
				{"y", new string[] { "u" ,"uyy"} },
				{"e", new string[] { "r" ,"ree"} }
			}
		);
		//10
		pathGenerator.AddRule (
			new Dictionary<string, string[]> {
				{"r", new string[] { "utuu","uyy","u"} },
				{"u", new string[] { "rwrr","ree","r"} },
				{"y", new string[] { "u" ,"uyy","tu"} },
				{"e", new string[] { "r" ,"ree","wr"} }
			}
		);
		//11
		pathGenerator.AddRule (
			new Dictionary<string, string[]> {
				{"r", new string[] { "utuu","uyy","u","ururu"} },
				{"u", new string[] { "rwrr","ree","r","rurur"} },
				{"y", new string[] { "u" ,"uyy","tu"} },
				{"e", new string[] { "r" ,"ree","wr"} }
			}
		);
		//temp12
		pathGenerator.AddRule (
			new Dictionary<string, string[]> {
				{"r", new string[] { "ururu","uurruurr","urrurr","ruuyuur"} },
				{"u", new string[] { "rururu","rruurruu","ruuruu","urrerru"} },
				{"y", new string[] { "uurr","ururu"} },
				{"e", new string[] { "rruu","rurur"} },
			}
		);

		pathGenerator.UseRule (0);

	}


	public void UseRule(int rule){
		//pathGenerator.Flush ();
		pathGenerator.UseRule( rule );
	}

	public void IncreaseComplexity(){
		pathGenerator.NextRule ();
	}

	public void DecreaseComplexity(){
		pathGenerator.PreviousRule ();
	}

	public void RelaxMode(int count=5){
		m_relaxMode = count;
		pathGenerator.AppendStraightRoad(5);
	}

	public void UseRuleOnes(int rule){
		Debug.Log("Iterate rule ones");
		pathGenerator.IterateSpecific(rule);
	}

	void InfiniteScroll2(){
		Vector3 playerPos = GameManager.Instance.player.transform.position;
		//Debug.Log(Vector3.Distance(playerPos,m_lastAddedPos));
		m_DistanceToLastAdded = Vector3.Distance(playerPos,m_lastAddedPos);
		if ( m_DistanceToLastAdded< 10) {//45
			CreateNextTile ();
			if(m_relaxMode > 0){
				CreateCoin (m_lastAddedTile);
				m_relaxMode--;
			}
			else
			{
				CreateRandomCoin (m_lastAddedTile);
			}
			m_lastUpdatedDistance = playerPos.magnitude;


			GameObject destroyedTile = m_startTile.transform.GetChild (0).gameObject;
			int i = 1;
			while (!destroyedTile.GetComponent<Rigidbody>().isKinematic) {
				destroyedTile = m_startTile.transform.GetChild (i).gameObject;
				i++;
			}
			destroyedTile.GetComponent<Rigidbody> ().isKinematic = false;
			if(destroyedTile.name == "e")
				Destroy(destroyedTile);
			else if (destroyedTile.name == "u") {
				destroyedTile.GetComponent<Rigidbody> ().AddTorque (Vector3.right * Random.Range(30,70));
			} else {
				destroyedTile.GetComponent<Rigidbody> ().AddTorque (Vector3.forward * Random.Range(30,70));
			}

		}

	}
		

	void DestroyFirstTile(object tile){
		Debug.Log ("Deleting tile");
		Destroy ((GameObject) tile);
	}


	void InitializeScene2(){
		m_lastAddedPos = new Vector3 (2, 0, 0);
		m_TileHeight = 0;
		InitializePath ();		
		for(int i=0;i<12;i++){//35
			CreateNextTile();
		}

	}
		
	IEnumerator InitializeScene3(){
		InitializeScene2();
		yield return new WaitForEndOfFrame();
		for(int i=0;i<5;i++){
			m_startTile.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
		}
	}
	void Reset(){
		//iTween.Stop ();
		Debug.Log("reseting tiles");
		foreach (Transform child in m_startTile.transform) {
			Destroy(child.gameObject);
		}
		StartCoroutine(InitializeScene3());
		m_lastUpdatedDistance = 0;
	}

	public void SoftReset(){
		foreach (Transform child in m_startTile.transform) {
			Destroy(child.gameObject);
		}
		pathGenerator.ResetString("rrrrrrr");
		for(int i=0;i<12;i++){//35
			CreateNextTile();
		}
	}

	void CreateRandomCoin(GameObject parent){

		if (Random.Range (0, 5) == 0) {
			CreateCoin (parent);
		}
	}
		
	void CreateCoin(GameObject parent){
		if (parent.transform.childCount == 0 || m_ScreenshotMode)
			return;
		//Vector3 coinPosition = parent.transform.position + new Vector3(0f,.4f,0f);
		GameObject coin = Instantiate((GameObject)m_Coin,Vector3.zero, m_Coin.transform.rotation) as GameObject;
		coin.transform.parent = parent.transform;
		coin.transform.localPosition = new Vector3(.8f,.4f,Random.Range(0f,0.4f));
	}

	GameObject CreateNextTile(){
		char tileCode = 'r';

		tileCode = pathGenerator.Next ();	


		Vector3 position = m_lastAddedPos;
		GameObject newTile = Instantiate((GameObject)m_envPrefabs[currentSelectedTile],position,Quaternion.identity) as GameObject;
		newTile.GetComponent<Rigidbody> ().isKinematic = true;
		newTile.transform.parent = m_startTile.transform;
		Vector3 upShift = new Vector3 (0, 0, 2);
		Vector3 rightShift = new Vector3 (-2, 0, 0);

		Transform previous = null, previousBefore = null;
		if (newTile.transform.GetSiblingIndex () > 1) {
			previous = m_startTile.transform.GetChild (newTile.transform.GetSiblingIndex () - 1);
			previousBefore = m_startTile.transform.GetChild (previous.GetSiblingIndex () - 1);
		}

		if(m_ScreenshotMode){
			tileCode = 'r';
		}

		switch (tileCode) {
			case 'e':
				{
					//Empty right
					position = position + rightShift;
					newTile.name = "e";
					newTile.SetActive (false);
					break;
				}
			case 'r':
				{
					position = position + rightShift;
					newTile.name = "r";
					break;
				}
			
			case 'y':
				{
					//Empty up
					position = position + upShift;
					newTile.name = "e";
					newTile.SetActive (false);
					break;
				}

			case 'u':
				{	
					position = position + upShift;
					newTile.name = "u";
					newTile.transform.localRotation = Quaternion.Euler(new Vector3(0,90,0));
					break;
				}

			case 't':
				{
					//Ramp up
					position = new Vector3(position.x,0,position.z) + upShift;
					Destroy (newTile);
					Quaternion rot;
					if (m_TileHeight != 0) {
						rot = Quaternion.Euler (new Vector3 (270, 180, 0));
						m_TileHeight = 0;
					} else {
						rot = Quaternion.Euler (new Vector3 (270, 0, 0));
						m_TileHeight = m_RampHeight;
					}
					newTile = Instantiate((GameObject)m_RampPrefab , new Vector3(position.x, 0, position.z), rot) as GameObject;
					newTile.GetComponent<Rigidbody> ().isKinematic = true;
					newTile.transform.parent = m_startTile.transform;	
					newTile.name = "u";

					break;
					
				}
			
			case 'w':
				{
					//Ramp right
					position = new Vector3(position.x,0,position.z) + rightShift;
					Quaternion rot;

					if (m_TileHeight != 0) {
						rot = Quaternion.Euler (new Vector3 (270, 90, 0));
						m_TileHeight = 0;
					} else {
						rot = Quaternion.Euler (new Vector3 (270, -90, 0));
						m_TileHeight = m_RampHeight;
					}

					Destroy (newTile);
					newTile = Instantiate((GameObject)m_RampPrefab , new Vector3(position.x, 0, position.z), rot) as GameObject;
					newTile.GetComponent<Rigidbody> ().isKinematic = true;
					newTile.transform.parent = m_startTile.transform;	
					newTile.name = "r";

					break;

				}

			default:
				break;
		}

		if (previous) {
			Vector3 diff = position - previousBefore.position;
			if (diff.x == -2 && diff.z == 2) {
				previous.name = "c";
				if(newTile.name=="u"||newTile.name=="y")
					previous.transform.localRotation = Quaternion.Euler(new Vector3(0,90,0));
				else
					previous.transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
				
			}
			if(newTile.name == "e"){
				//Activate jump trigger
				previous.transform.GetChild(0).gameObject.SetActive(true);
			}
		}
			

		m_lastAddedPos =  new Vector3 (position.x, m_TileHeight, position.z);
//		newTile.transform.position = position;
		newTile.transform.position = new Vector3 (position.x, -1, position.z );
		iTween.MoveTo (newTile, iTween.Hash("position", position, "time",.4f, "easeType", iTween.EaseType.easeOutCubic));

		m_lastAddedTile = newTile;
		return newTile;
	}
		
	public Transform ClosestTile(Vector3 p){
		Transform retval=m_startTile.transform.GetChild(0);
		float distance = 100f;
		p = new Vector3(p.x,0.3f,p.z);
		foreach (Transform tile in m_startTile.transform) {
			if (tile.position.x - 1 < p.x && p.x < tile.position.x + 1 && tile.position.z - 1 < p.z && p.z < tile.position.z + 1) {
				return tile;
			}
		}

		foreach (Transform tile in m_startTile.transform) {
			if( Vector3.Distance(tile.position,p) < distance){
				distance  = Vector3.Distance(tile.position,p);
				retval = tile;
			}

		}
		return retval;
	}

	public Transform GetAvailableTileForSpawn(){
		foreach(Transform tile in m_startTile.transform){
			if (tile.name=="u" || tile.name=="r"){
				return tile;
			}
		}
		return null;
	}
	public Transform GetFirstTile(){
		return m_startTile.transform.GetChild(0);
	}

	public void IncreaseSelectedTile(){
		currentSelectedTile=Mathf.Min(m_envPrefabs.Length-1, currentSelectedTile+1);
	}

	public void DescreaseTile(){
		currentSelectedTile=Mathf.Max(0,currentSelectedTile-1);
	}

	public void ResetSelectedTile(){
		currentSelectedTile=0;
	}

	public void SetSelectedTile(int f){
		currentSelectedTile=Mathf.Max(0, Mathf.Min(m_envPrefabs.Length-1, f));
	}

}
