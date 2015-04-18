using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class RedMetricsUser : MonoBehaviour {

	public RedMetricsManager rmm; 

	private string redMetricsURL = "https://api.redmetrics.io/v1/";
	private string redMetricsEvent = "event";
	private string redMetricsPlayer = "player";
	private string gameVersion = "99a00e65-6039-41a3-a85b-360c4b30a466";
	private string playerID = "b5ab445a-56c9-4c5b-a6d0-86e8a286cd81";

	void Start () 
	{
		createEvent();
	}

	void setPlayerID(string pID)
	{
		playerID = pID;
	}
	
	private void createEvent()
	{
		string url = redMetricsURL+redMetricsEvent;
		Dictionary<string, string> headers = new Dictionary<string, string>();
		headers.Add("Content-Type", "application/json");

		string ourPostData = "{\"gameVersion\":\""+gameVersion+"\"," +
			"\"player\":\""+playerID+"\"," +
				"\"type\":\"todelete\"}";
		byte[] pData = System.Text.Encoding.ASCII.GetBytes(ourPostData.ToCharArray());
		WWW www;
		StartCoroutine(rmm.POST(url, pData, headers, value => www = value));
	}
	
	private void createPlayer()
	{
		string url = redMetricsURL+redMetricsPlayer;
		Dictionary<string, string> headers = new Dictionary<string, string>();
		headers.Add("Content-Type", "application/json");
		string ourPostData = "{\"type\":todelete}";
		byte[] pData = System.Text.Encoding.ASCII.GetBytes(ourPostData.ToCharArray());
		WWW www;
		StartCoroutine(rmm.POST(url, pData, headers, value => www = value));

	}
}
