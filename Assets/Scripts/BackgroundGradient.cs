using UnityEngine;
using System.Collections;

public class BackgroundGradient : MonoBehaviour {

	// Use this for initialization
	public Texture2D m_mainTexture;
	public Color m_topColor;
	public Color m_bottomColor;

	private Mesh m_Mesh;
	private Color[] m_colors;

	void Awake () {
		m_mainTexture = new Texture2D (1, 2, TextureFormat.ARGB32, false);
		m_Mesh = GetComponent<MeshFilter> ().mesh;
		m_colors = new Color[4]{ Color.red,Color.blue, Color.red, Color.blue };
		m_Mesh.colors = m_colors;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetColors(Color top, Color bottom){
		if (m_colors == null)
			return;
		m_topColor = top;
		m_bottomColor = bottom;
		m_colors [0] = top;
		m_colors [1] = bottom;
		m_colors [2] = top;
		m_colors [3] = bottom;
		m_Mesh.colors = m_colors;
	}
}
