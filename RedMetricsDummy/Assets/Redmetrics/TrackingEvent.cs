using UnityEngine;
using System.Collections;

public enum TrackingEvent {
	//standard events
	DEFAULT,
	START,
	END,
	WIN,
	FAIL,
	RESTART,
	GAIN,
	LOSE,

	//other examples of events
	CREATEPLAYER,
	CHANGEPLAYER,
	JUMP,
	BOUNCE
}