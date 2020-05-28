using UnityEngine;
using System.Collections;
using System;
public class ReviewDisplay : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		string lastDateShown = PlayerPrefs.GetString("ReviewBoxDisplayDate", DateTime.Now.AddDays(.5).ToShortTimeString());
		DateTime lastDateShownParsed= DateTime.Parse(lastDateShown);
		TimeSpan offset = DateTime.Now-lastDateShownParsed;
		if(offset.TotalDays>1){
			IOSReviewRequest.RequestReview();
			PlayerPrefs.SetString("ReviewBoxDisplayDate",DateTime.Now.ToShortTimeString());

		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

