using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BBCommon;

public class ColorManager : Singleton<ColorManager>
{
	public  Material m_backgroundMaterial;
	public  Material m_tileMaterial;
	public int lastAssignedAngle;

	public BackgroundGradient m_backgroundGradient;

	public ColorManager ()
	{
		lastAssignedAngle = 0;
	}


	public void FindRandomIndexes( out int top, out int bottom, out int tile){

		bottom = Random.Range(0, 	14) * 7;
		tile = bottom + Random.Range(4, 7);
		top = bottom + 6;

		bottom += Random.Range (0, 2);
		top -= Random.Range (0, 2);
	}


	public  void AssignRandomColors2(){
		int angle = Random.Range (0, 360);

		Debug.Log ("color angle" + angle);
		Color tile = Wheel (angle,saturation:190.0f, brightness:150.0f);
		Color top = Wheel (angle + 120,  saturation:200.0f);
		Color bottom = Wheel (angle + 30, saturation:100.0f, brightness:255.0f);


		m_backgroundMaterial.SetColor ("_BottomColor", bottom);
		m_backgroundMaterial.SetColor ("_TopColor", top);
		m_tileMaterial.SetColor ("_MainColor", tile);
		

	}

	void Start(){
//		m_backgroundMaterial.SetColor ("_TopColor", hexToColor("09B44AFF")); //09B44AFF
//		m_backgroundMaterial.SetColor ("_BottomColor", hexToColor("83F29500"));//83F29500
//		m_tileMaterial.SetColor ("_MainColor", hexToColor("FF8410FF"));//FF8410FF


	}


	public void ColorTransition(){
		if (lastAssignedAngle == 0) {
			m_backgroundMaterial.SetColor ("_TopColor", new Color(.75f,.75f,.75f)); //09B44AFF
			m_backgroundMaterial.SetColor ("_BottomColor", new Color(1f,1f,1f));//83F29500
			m_tileMaterial.SetColor ("_MainColor", new Color(.75f,.75f,.75f));//FF8410FF


			m_backgroundGradient.SetColors (new Color(1f,1f,1f),new Color(.75f,.75f,.75f));
			lastAssignedAngle = Random.Range (0, 360);
		} else {
			lastAssignedAngle += Random.Range(30,90);
		}
		Color top=Color.black, bottom=Color.black;
		Color tile = Wheel (lastAssignedAngle,saturation:150.0f, brightness:165.0f);
		int choice = Random.Range (0, 4);
		Debug.Log ("Color:" + choice);
		//choice = 0;
		switch (choice) {
		case 0:
			{		
				
				//Color top = Wheel (lastAssignedAngle + Random.Range(1,4)*30,  saturation:200.0f);
				//Color bottom = Wheel (lastAssignedAngle + Random.Range(-1,1)*30, saturation:100.0f, brightness:255.0f);			
				
				//top = Wheel (lastAssignedAngle + Random.Range(2,4)*30,  saturation:200.0f);				
				//bottom = Wheel (lastAssignedAngle + Random.Range(-1,2)*30, saturation:110.0f, brightness:255.0f);
				
				top = Wheel (lastAssignedAngle + Random.Range(1,4)*30,  saturation:200.0f);
				bottom = Wheel (lastAssignedAngle + Random.Range(-1,1)*30, saturation:100.0f, brightness:255.0f);
				break;
			}
		case 1:
			{
				
				top = Wheel (lastAssignedAngle - Random.Range(1,4)*30,  saturation:200.0f);
				bottom = Wheel (lastAssignedAngle + Random.Range(1,3)*30, saturation:110.0f, brightness:255.0f);
				break;
			}
		case 2:
			{
				int topC = Random.Range (0,360);
				top = Wheel (topC,  saturation:30.0f, brightness:150.0f);
				bottom = Wheel (topC+60, saturation:60.0f, brightness:255.0f);
				break;
			}

		case 3:
			{
				
				top = Wheel (lastAssignedAngle,  saturation:30.0f, brightness:150.0f);
				bottom = Wheel (lastAssignedAngle+60, saturation:60.0f, brightness:255.0f);
				break;
			}
		}


		StopAllCoroutines();
		StartCoroutine( ColorTo (m_backgroundMaterial, "_TopColor", top, 3));
		StartCoroutine( ColorTo (m_backgroundMaterial, "_BottomColor", bottom, 3));
		StartCoroutine( ColorTo (m_tileMaterial, "_MainColor", tile, 3));

		if (m_backgroundGradient) {
			StartCoroutine (ColorBGGradient (bottom, top, 3));
		}
	}

	IEnumerator ColorBGGradient(Color topColor, Color bottomColor, float inTime){

		float t = 0;
		Color fromTopColor = m_backgroundGradient.m_topColor;
		Color fromBottomColor = m_backgroundGradient.m_bottomColor;
		while (t!=1) {
			t += Time.deltaTime/inTime;
			if(t>1)
				t=1;
			m_backgroundGradient.SetColors ( Color.Lerp (fromTopColor, topColor, t) ,   Color.Lerp (fromBottomColor, bottomColor, t));
			yield return null;
		}


	}

	IEnumerator ColorTo(Material mat, string colorName, Color toColor, float inTime) {
		float t = 0;
		Color fromColor = mat.GetColor (colorName);
		while (t!=1) {
			t += Time.deltaTime/inTime;
			if(t>1)
				t=1;
			mat.SetColor (colorName, Color.Lerp (fromColor, toColor, t));
			yield return null;
		}
	}


	public static Color hexToColor(string hex)
	{
		hex = hex.Replace ("0x", "");//in case the string is formatted 0xFFFFFF
		hex = hex.Replace ("#", "");//in case the string is formatted #FFFFFF
		byte a = 255;//assume fully visible unless specified in hex
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		//Only use alpha if the string has enough characters
		if(hex.Length == 8){
			a = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		}
		return new Color32(r,g,b,a);
	}


	public Color Wheel(int angle, float saturation=150.0f, float brightness=150.0f ){
		float ang = (angle % 360) / 360.0f;
		float s = saturation/ 255.0f;
		float v = brightness / 255.0f;
		return Color.HSVToRGB (ang, s, v);
	}


}


