using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PathGenerator   {

	private string currentString;
	private int currentRule;
	private List<Dictionary<string, string[]>> rules;

	public PathGenerator(){
		currentRule = -1;
		currentString = "";
		rules = new List<Dictionary<string, string[]>> ();
	}

	public void Reset(string start){
		rules.Clear ();
		currentString = start;
	}

	public void Flush(){
		currentString = currentString.Substring(currentString.Length-2);
	}

	public int AddRule( Dictionary<string, string[]> rule){
		rules.Add (rule);
		return rules.Count - 1;
	}

	public void UseRule(int ruleNdx ){
		currentRule = ruleNdx;
	}

	public char Next(){
		if (currentString.Length < 3) {			
			//for (int i = 0; i < 2; i++) {
				currentString = Iterate (currentString, rules [currentRule]);
				Debug.Log (currentString);
			//}
		}
		char retval = currentString [0];
		currentString = currentString.Substring (1);
		return retval;
	}

	public void IterateSpecific(int ndx){
		Debug.Log("Iterate specific " + ndx + " curstr:"+currentString);
		currentString = Iterate (currentString, rules [ndx]);
		Debug.Log("curstr after:" +currentString);
	}
	private object RandomChoice( IList arr){
		return arr[Random.Range(0,arr.Count)];
	}

	string Iterate (string text, Dictionary<string,string[]> rules) {
		List<string> eligible_keys = new List<string> ();
		foreach (KeyValuePair<string,string[]> item in rules) {
			//if (text.LastIndexOf (item.Key) != -1 && text.LastIndexOf (item.Key) + item.Key.Length == text.Length) {
			if(text.EndsWith(item.Key)){
				eligible_keys.Add (item.Key);
			}
		}
		string selected="", selectedStr=""; 
		if(eligible_keys.Count == 0){
			Debug.LogWarning("No key for " + text + " current rule:" + currentRule);
			if(text.EndsWith("y")) selectedStr="u";
			else if(text.EndsWith("e")) selectedStr="r";
		}
		else{
			selected = RandomChoice (eligible_keys) as string;
			selectedStr = RandomChoice (rules [selected]) as string;
		}
		return text + selectedStr;
	}

	///
	public int Count(){
		return rules.Count ;
	}

	public bool NextRule(){
		if (currentRule + 1 != rules.Count) {
			UseRule (currentRule + 1);
			return true;
		}
		return false;
	}

	public bool PreviousRule(){
		if (currentRule > 0) {
			UseRule (currentRule - 1);
			return true;
		}
		return false;
	}

	public void Append(string str){
		currentString+=str;
	}

	public void AppendStraightRoad(int count){
		if(currentString.EndsWith("u") || currentString.EndsWith("y")){
			currentString+=new string('u',count);
		}
		else if(currentString.EndsWith("r") || currentString.EndsWith("e")){
			currentString+=new string('r',count);
		}
	}

	public void ResetString(string str){
		currentString=str;
	}
}
