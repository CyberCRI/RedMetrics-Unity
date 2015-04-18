using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RedMetricsManager : MonoBehaviour
{
		
	void Start ()
	{
	}
		
	public IEnumerator GET (string url, System.Action<WWW> callback)
	{
		
		WWW www = new WWW (url);
		yield return www;
		callback(www);
	}
		
	public IEnumerator POST (string url, Dictionary<string,string> post, System.Action<WWW> callback)
	{
		WWWForm form = new WWWForm ();
		foreach (KeyValuePair<string,string> post_arg in post) {
			form.AddField (post_arg.Key, post_arg.Value);
		}
		
		WWW www = new WWW (url, form);
		yield return www;
		callback(www);
	}
	
	public IEnumerator POST (string url, byte[] post, Dictionary<string, string> headers, System.Action<WWW> callback)
	{
		WWW www = new WWW (url, post, headers);
		yield return www;
		callback(www);
	}
}
