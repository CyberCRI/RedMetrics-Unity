using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RedMetricsManager : MonoBehaviour
{    
	
	//////////////////////////////// singleton fields & methods ////////////////////////////////
	public static string gameObjectName = "RedMetricsManager";
	private static RedMetricsManager _instance;
	public static RedMetricsManager get() {
		if (_instance == null)
		{
			_instance = GameObject.Find(gameObjectName).GetComponent<RedMetricsManager>();
			if(null != _instance)
			{
				//RedMetricsManager object is not destroyed when game restarts
				DontDestroyOnLoad(_instance.gameObject);
				_instance.initializeIfNecessary();
			}
			else
			{
				Debug.LogError("RedMetricsManager::get couldn't find game object");
			}
		}
		return _instance;
	}
	void Awake()
	{
		Debug.Log("RedMetricsManager::Awake");
		antiDuplicateInitialization();
	}
	
	void antiDuplicateInitialization()
	{
		RedMetricsManager.get ();
		Debug.LogError("RedMetricsManager::antiDuplicateInitialization with hashcode="+this.GetHashCode()+" and _instance.hashcode="+_instance.GetHashCode());
		if(this != _instance) {
			Debug.Log("RedMetricsManager::antiDuplicateInitialization self-destruction");
			Destroy(this.gameObject);
		}
	}
	////////////////////////////////////////////////////////////////////////////////////////////
	
	private void initializeIfNecessary() {}
	
	private string redMetricsURL = "https://api.redmetrics.io/v1/";
	private string redMetricsPlayer = "player";
	private string redMetricsEvent = "event";

	//Hero.Coli's test game version
	private static string defaultGameVersion = "\"83f99dfa-bd87-43e1-940d-f28bbcea5b1d\"";
	private string gameVersion = defaultGameVersion;
	private static string defaultPlayerID = "\"b5ab445a-56c9-4c5b-a6d0-86e8a286cd81\"";
	private string playerID = defaultPlayerID;	    
	
	public void setPlayerID (string pID)
	{
		Debug.Log("setPlayerID("+pID+")");
		playerID = pID;
	}
	
	public void setGameVersion (string gVersion)
	{
		Debug.Log("setGameVersion("+gVersion+")");
		gameVersion = gVersion;
	}
	
	
	//////////////////////////////////////////////////
	/// standalone methods
	
	public static IEnumerator GET (string url, System.Action<WWW> callback)
	{
		Debug.Log("GET");
		WWW www = new WWW (url);
		return waitForWWW (www, callback);
	}
	
	public static IEnumerator POST (string url, Dictionary<string,string> post, System.Action<WWW> callback)
	{
		Debug.Log("POST");
		WWWForm form = new WWWForm ();
		foreach (KeyValuePair<string,string> post_arg in post) {
			form.AddField (post_arg.Key, post_arg.Value);
		}
		
		WWW www = new WWW (url, form);
		return waitForWWW (www, callback);
	}
	
	public static IEnumerator POST (string url, byte[] post, Dictionary<string, string> headers, System.Action<WWW> callback)
	{
		Debug.Log ("POST url:"+url);
		WWW www = new WWW (url, post, headers);
		return waitForWWW (www, callback);
	}
	
	private static IEnumerator waitForWWW (WWW www, System.Action<WWW> callback)
	{
		Debug.Log ("waitForWWW");
		float elapsedTime = 0.0f;
		
		if(null == www)
		{
			Debug.LogError("waitForWWW: null www");
			yield return null;
		}
		
		while (!www.isDone) {
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= 30.0f)
			{
				Debug.LogError("waitForWWW: TimeOut!");
				break;
			}
			yield return null;
		}
		
		if (!www.isDone || !string.IsNullOrEmpty (www.error)) {
			string errmsg = string.IsNullOrEmpty (www.error) ? "timeout" : www.error;
			Debug.LogError("RedMetricsManager::waitForWWW Error: Load Failed: " + errmsg);
			callback (null);    // Pass null result.
			yield break;
		}
		
		Debug.LogError("waitForWWW: www good to ship!");
		callback (www); // Pass retrieved result.
	}
	
	////////////////////////////////////////
	/// helpers
	/// 
	
	private void sendData(string urlSuffix, string pDataString, System.Action<WWW> callback)
	{
		string url = redMetricsURL + urlSuffix;
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");
		byte[] pData = System.Text.Encoding.ASCII.GetBytes (pDataString.ToCharArray ());
		Debug.Log("RedMetricsManager::sendData StartCoroutine POST with data="+pDataString+" ...");
		StartCoroutine (RedMetricsManager.POST (url, pData, headers, callback));
	}
	
	private void createPlayer (System.Action<WWW> callback)
	{
		string ourPostData = "{\"type\":"+TrackingEvent.CREATEPLAYER+"}";
		sendData(redMetricsPlayer, ourPostData, callback);
	}
	
	private void testGet(System.Action<WWW> callback)
	{
		Debug.Log("testGet");
		string url = redMetricsURL + redMetricsPlayer;
		StartCoroutine (RedMetricsManager.GET (url, callback));
	}
	
	private void wwwLogger (WWW www, string origin = "default")
	{
		if(null == www) {
			Debug.LogError("RedMetricsManager::wwwLogger null == www from "+origin);
		} else {
			if (www.error == null) {
				Debug.LogError("RedMetricsManager::wwwLogger Success: " + www.text + " from "+origin);
			} else {
				Debug.LogError("RedMetricsManager::wwwLogger Error: " + www.error + " from "+origin);
			} 
		}
	}
	
	private string extractPID(WWW www)
	{
		string result = null;
		wwwLogger(www, "extractPID");
		string trimmed = www.text.Trim ();
		string[] split1 = trimmed.Split('\n');
		foreach(string s1 in split1)
		{
			//Debug.Log(s1);
			if(s1.Length > 5)
			{
				string[] split2 = s1.Trim ().Split(':');
				foreach(string s2 in split2)
				{
					if(!s2.Equals("id")){
						//Debug.Log ("id =? "+s2);
						result = s2;
					}
				}
			}
		}
		return result;
	}
	
	private void trackStart(WWW www)
	{
		Debug.Log("trackStart: www =? null:"+(null == www));
		string pID = extractPID(www);
		setPlayerID(pID);
		sendEvent(TrackingEvent.START);
	}
	
	public void sendStartEvent(bool switched)
	{
		Debug.Log ("RedMetricsManager::sendStartEvent");
		
		// management of game start for webplayer
		if(!switched) {
			//switch event is sent from somewhere else
			connect ();
			StartCoroutine(waitAndSendStart());
		}
		
		//TODO management of game start for standalone
		//WARNING: switch event is sent from somewhere else
		/*
        string pID = null;
        bool tryGetPID = tryGetData(playerDataKey, out pID);
        if(tryGetPID && !string.IsNullOrEmpty(pID))
        {
            Debug.Log ("RedMetricsManager::sendStartEvent player already identified - pID="+pID);
            string currentLevelName = "?";
            LevelInfo levelInfo;
            bool success = tryGetCurrentLevelInfo(out levelInfo);
            if(success && null != levelInfo)
            {
                currentLevelName = levelInfo.code;
            }
            sendEvent(TrackingEvent.SWITCH,currentLevelName);
        } else {
            createPlayer (www => trackStart(www));
            //testGet (www => trackStart(www));
        }
        */
		
	}
	
	private IEnumerator waitAndSendStart() {
		yield return new WaitForSeconds(5.0f);
		sendEvent(TrackingEvent.START);
	}
	
	public void connect()
	{
		string json = "{\"gameVersionId\": "+gameVersion+"}";
		Debug.LogError ("RedMetricsManager::connect will rmConnect json="+json);
		Application.ExternalCall("rmConnect", json);
	}
	
	
	private string innerCreateJsonForRedMetrics(string eventCode, string customData, string section, string coordinates)
	{
		string eventCodePart = "", customDataPart = "", sectionPart = "", coordinatesPart = "";
		
		eventCodePart = "\"type\":\"";
		if(string.IsNullOrEmpty(eventCode)) {
			eventCodePart+="unknown";
		} else {
			eventCodePart+=eventCode;
		}
		eventCodePart+="\"";
		
		if(!string.IsNullOrEmpty(customData)) {
			customDataPart = ",\"customData\":\""+customData+"\"";
		}
		
		if(!string.IsNullOrEmpty(section)) {
			sectionPart = ",\"section\":\""+section+"\"";
		}
		
		if(!string.IsNullOrEmpty(coordinates)) {
			coordinatesPart = ",\"coordinates\":\""+coordinates+"\"";
		}
		
		return eventCodePart+customDataPart+sectionPart+coordinatesPart+"}";
	}
	
	private string createJsonForRedMetrics(string eventCode, string customData, string section, string coordinates)
	{
		string jsonPrefix = "{\"gameVersion\":" + gameVersion + "," +
			"\"player\":";
		string jsonSuffix = innerCreateJsonForRedMetrics(eventCode, customData, section, coordinates);

		string pID = playerID;
		if(!string.IsNullOrEmpty(pID))
		{
			Debug.Log ("RedMetricsManager::sendEvent player already identified - pID="+pID);            
		} else {
			Debug.LogError ("RedMetricsManager::sendEvent no registered player!");
			pID = defaultPlayerID;
		}
		return jsonPrefix+pID+","+jsonSuffix;
		//sendData(redMetricsEvent, ourPostData, value => wwwLogger(value, "sendEvent("+eventCode+")"));
	}
	
	private string createJsonForRedMetricsJS(string eventCode, string customData, string section, string coordinates)
	{
		return "{"+innerCreateJsonForRedMetrics(eventCode, customData, section, coordinates);
	}
	
	// see github.com/CyberCri/RedMetrics.js
	// with type -> eventCode
	public void sendEvent(TrackingEvent trackingEvent, string customData = null, string section = null, string coordinates = null)
	{
		//TODO test on build type:
		// if webplayer, then use Application.ExternalCall("rmConnect", json);
		// else if standalone, then use WWW
		// else ... ?
		
		string json = createJsonForRedMetricsJS(trackingEvent.ToString().ToLower(), customData, section, coordinates);
		Debug.Log ("RedMetricsManager::sendEvent will rmPostEvent json="+json);
		Application.ExternalCall("rmPostEvent", json);
		
		/*
        //TODO management of game start for standalone
        string pID = null;
        bool tryGetPID = tryGetData(playerDataKey, out pID);
        if(tryGetPID && !string.IsNullOrEmpty(pID))
        {
            Debug.Log ("RedMetricsManager::sendEvent player already identified - pID="+pID);
            string ourPostData = "{\"gameVersion\":" + gameVersion + "," +
                "\"player\":" + playerID + "," +
                    "\"type\":\""+eventCode+"\"}";
            sendData(redMetricsEvent, ourPostData, value => wwwLogger(value, "sendEvent("+eventCode+")"));
        } else {
            Debug.LogError ("RedMetricsManager::sendEvent no registered player!");
        }
        */
	}
	
	public override string ToString ()
	{
		return string.Format ("[RedMetricsManager playerID:{0}, gameVersion:{1}, redMetricsURL:{2}]",
		                      playerID, gameVersion, redMetricsURL);
	}
	
}
