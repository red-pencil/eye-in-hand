using UnityEngine;
using System.Collections;

public class ObjectDisabler : MonoBehaviour {

	public Camera TargetCam;
	public KeyCode TargetKey;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (TargetKey))
			TargetCam.enabled = !TargetCam.enabled;
	}
}
