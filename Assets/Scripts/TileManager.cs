using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TileManager : MonoBehaviour {

	public GameObject[] m_rightTiles;
	public GameObject[] m_upTiles;
	public GameObject m_Coin;
	public GameObject m_startTile;
	public GameObject m_startPrefab;

	public GameObject[] m_envPrefabs;

	private GameObject m_lastAddedTile;
	private Queue<GameObject> m_tilePool;
	private Queue<GameObject> m_envPool;

	private int m_lastUpdatedDistance;
	private int m_conRoad;


	//private List<GameObject> m_rightTilePool;
	//private List<GameObject> m_upTilePool;


	void Awake(){
		m_lastAddedTile = m_startTile;
		m_tilePool=new Queue<GameObject>();
		m_envPool=new Queue<GameObject>();

	}

	// Use this for initialization
	void Start () {
	}

	void OnEnable(){
		//GameManager.OnGameMainMenu += Reset;
	}
	
	void OnDisable(){
		//GameManager.OnGameMainMenu -= Reset;
	}

	// Update is called once per frame
	void Update () {
		Vector3 playerPos = GameManager.Instance.player.transform.position;
		if (GameManager.Instance.m_GameState == "run") {
			float distance = Vector3.Distance (playerPos, m_lastAddedTile.transform.position);
			if (distance < 8) {
				InfiniteScroll ();
			}
		}

	}

	void InfiniteScroll(){
		for(int i=0;i<10;++i){
			//Destroy(m_tilePool.Dequeue());	
			//Destroy(m_envPool.Dequeue());	
			Destroy( m_startTile.transform.GetChild(i).gameObject);
			if(i%2==1){
				CreateRandomTile();
			}
		}
		//StaticBatchingUtility.Combine (m_startTile.transform.parent.gameObject);
	}

	void InitializeScene(){
		m_lastAddedTile = Instantiate(m_startPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		m_lastAddedTile.transform.parent = m_startTile.transform;
		for(int i=0;i<20;i++){
			CreateRandomTile();
		}
		//StaticBatchingUtility.Combine (m_startTile.transform.parent.gameObject);
	}




	void Reset(){
		foreach (Transform child in m_startTile.transform) {
			Destroy(child.gameObject);
		}

		m_tilePool.Clear();
		m_envPool.Clear();

		InitializeScene ();
	}

	void CreateRandomCoin(GameObject parent){
		if (Random.Range (0, 5) == 0) {
			//Vector3 coinPosition = position - new Vector3(1.5f, 0.8f , 1);
			Vector3 coinPosition = parent.transform.position +new Vector3( Random.Range (1.0f,1.5f), 0.89f , Random.Range (1.0f,1.5f) );
			GameObject coin = Instantiate((GameObject)m_Coin,coinPosition, m_Coin.transform.rotation) as GameObject;
			coin.transform.parent = parent.transform;
		}
	}

	void CreateRandomTile(){
		//int previousUpIndex=0, previousRightIndex=0;
		Transform anchor = m_lastAddedTile.transform.GetChild(0);
		if(Mathf.FloorToInt(anchor.localRotation.eulerAngles.y) == 180){
			int ndx = 0;
			ndx = Random.Range(0,m_rightTiles.Length);
			if (m_conRoad == 2) {
				ndx = 0;
			}	
			m_lastAddedTile = Instantiate((GameObject)m_rightTiles.GetValue(ndx),anchor.position,Quaternion.identity) as GameObject;

		}
		else{
			int ndx = 0;
			//while(ndx==previousUpIndex)
			ndx = Random.Range(0,m_upTiles.Length);
			if (m_conRoad == 2) {
				ndx = 0;
			}	
			m_lastAddedTile = Instantiate((GameObject)m_upTiles.GetValue(ndx),anchor.position,Quaternion.identity) as GameObject;
		}
		CreateRandomCoin (m_lastAddedTile);
		//m_tilePool.Enqueue(m_lastAddedTile);
		//m_lastAddedTile.isStatic = true;

		if (m_lastAddedTile.name.Contains ("Road")) {
			m_conRoad++;
		} else {
			m_conRoad = 0;
		}

		m_lastAddedTile.transform.parent = m_startTile.transform;

		CreateEnvironment(anchor);

	}

	void CreateEnvironment(Transform anchor){
		//Create env
		if(m_envPrefabs.Length == 0)
			return;
		int envNdx = Random.Range (0, m_envPrefabs.Length);
		GameObject g = Instantiate (m_envPrefabs [envNdx], Vector3.zero, Quaternion.identity) as GameObject;
		Vector3 anchorPos = anchor.transform.position;


		float r = Random.Range(-4.0f,4.0f);
		r = r + 3f*Mathf.Sign(r);
		Vector3 newPos = anchorPos + r * anchor.transform.right;//+ anchor.transform.localRotation * new Vector3 ( x,0,z);
		//newPos = Vector3.Project(newPos,anchor.transform.forward*anchorPos);
		g.transform.position = new Vector3 (newPos.x, -3.5f, newPos.z); 
		float scale = Random.Range (.6f, .8f);
		g.transform.localScale = new Vector3 (scale, scale, scale);
		g.transform.parent =  m_startTile.transform;
	}
}
