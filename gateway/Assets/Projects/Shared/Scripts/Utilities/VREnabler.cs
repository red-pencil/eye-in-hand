using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class VREnabler : MonoBehaviour {

	public bool EnableVR;

	// Use this for initialization
	void Start () {
		UnityEngine.XR.XRSettings.enabled=EnableVR;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
