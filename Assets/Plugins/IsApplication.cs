using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
public class IsApplication : MonoBehaviour {

	public static bool Installed(string app_bundle){
		#if UNITY_ANDROID
		AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
		Debug.Log(" ********LaunchOtherApp ");
		AndroidJavaObject launchIntent = null;
		//if the app is installed, no errors. Else, doesn't get past next line
		try{
			launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage",app_bundle);
			//        
			//        ca.Call("startActivity",launchIntent);
		}catch(Exception ex){
			Debug.Log("exception"+ex.Message);
		}
		if(launchIntent == null)
			return false;
		return true;
		#else
		return canOpenUrl(app_bundle);
		#endif

	}


	#if UNITY_IOS

	[DllImport ("__Internal")] private static extern bool canOpenUrl(string url);

	#endif
}
