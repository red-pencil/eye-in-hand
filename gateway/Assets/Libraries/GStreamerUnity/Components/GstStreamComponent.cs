using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GstStreamComponent : MonoBehaviour {


	public IGstStreamer Streamer;
	// Use this for initialization
	void Start () {
		
	}

	void OnDestroy()
	{
		if (Streamer != null) {
			Streamer.Destroy ();
			Streamer = null;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
