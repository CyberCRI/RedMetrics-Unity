using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class RedMetricsUser : MonoBehaviour
{

	public RedMetricsManager rmm;
	private string redMetricsURL = "https://api.redmetrics.io/v1/";
	private string redMetricsEvent = "event";
	private string redMetricsPlayer = "player";
	private string gameVersion = "\"99a00e65-6039-41a3-a85b-360c4b30a466\"";
	private string playerID = "\"b5ab445a-56c9-4c5b-a6d0-86e8a286cd81\"";

	private string createPlayerEventType = "\"newPlayer\"";
	private string startEventType = "\"start\"";

	void Start ()
	{
		createPlayer (www => trackStart(www));
	}

	void setPlayerID (string pID)
	{
		Debug.Log("setPlayerID("+pID+")");
		playerID = pID;
	}
	
	private void createEvent (string eventType)
	{
		string url = redMetricsURL + redMetricsEvent;
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");

		string ourPostData = "{\"gameVersion\":" + gameVersion + "," +
			"\"player\":" + playerID + "," +
			"\"type\":"+eventType+"}";
		byte[] pData = System.Text.Encoding.ASCII.GetBytes (ourPostData.ToCharArray ());
		Debug.Log("StartCoroutine...");
		StartCoroutine (rmm.POST (url, pData, headers, value => wwwLogger(value)));
	}
	
	private void createPlayer (System.Action<WWW> callback)
	{
		string url = redMetricsURL + redMetricsPlayer;
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");
		string ourPostData = "{\"type\":"+createPlayerEventType+"}";
		byte[] pData = System.Text.Encoding.ASCII.GetBytes (ourPostData.ToCharArray ());
		Debug.Log("StartCoroutine...");
		StartCoroutine (rmm.POST (url, pData, headers, callback));
	}

	private void wwwLogger (WWW www)
	{
		if (www.error == null) {
			Debug.Log ("Success: " + www.text);
		} else {
			Debug.Log ("Error: " + www.error);
		} 
	}

	private string extractPID(WWW www)
	{
		string result = null;
		wwwLogger(www);
		string trimmed = www.text.Trim ();
		string[] split1 = trimmed.Split('\n');
		foreach(string s1 in split1)
		{
			Debug.Log(s1);
			if(s1.Length > 5)
			{
				string[] split2 = s1.Trim ().Split(':');
				foreach(string s2 in split2)
				{
					if(!s2.Equals("id")){
						Debug.Log ("id =? "+s2);
						result = s2;
					}
				}
			}
		}
		return result;
	}

	private void trackStart(WWW www)
	{
		string pID = extractPID(www);
		setPlayerID(pID);
		createEvent(startEventType);
	}
}
