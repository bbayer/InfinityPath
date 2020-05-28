using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {
	int bestScore,totalCoins,totalScore;
	public override void  OnInspectorGUI () {
		//Called whenever the inspector is drawn for this object.
		DrawDefaultInspector();
		EditorGUILayout.BeginHorizontal(GUIStyle.none);
		bestScore = EditorGUILayout.IntSlider(bestScore,0,300);
		if(GUILayout.Button("set best score")){
			PlayerPrefs.SetInt("BestScore",bestScore);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal(GUIStyle.none);
		totalCoins = EditorGUILayout.IntSlider(totalCoins,0,300);
		if(GUILayout.Button("set coin")){
			PlayerPrefs.SetInt("TotalCoins",totalCoins);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal(GUIStyle.none);
		totalScore = EditorGUILayout.IntSlider(totalScore,0,60000);
		if(GUILayout.Button("total score")){
			PlayerPrefs.SetInt("TotalScore",totalScore);
		}
		EditorGUILayout.EndHorizontal();
		//This draws the default screen.  You don't need this if you want
		//to start from scratch, but I use this when I'm just adding a button or
		//some small addition and don't feel like recreating the whole inspector.
		if(GUILayout.Button("clear playerprefs")) {
			//add everthing the button would do.
			PlayerPrefs.DeleteAll();
		}
		else if(GUILayout.Button("unlock all")) {
			//add everthing the button would do.
			GameManager.Instance.UnlockAll();
		}
		else if(GUILayout.Button("lock all")) {
			GameManager.Instance.LockAll();

		}

	}
}

