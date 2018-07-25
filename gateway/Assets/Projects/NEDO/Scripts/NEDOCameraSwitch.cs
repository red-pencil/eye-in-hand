using UnityEngine;
using System.Collections;

public class NEDOCameraSwitch : MonoBehaviour {

	public GameObject FPSCam;
	public GameObject ThirdPSCam;

	public NEDOVehicleGround vehicle; 

	public NEDODirectionPointer NavigationArrow;

	// Use this for initialization
	void Start () {
		vehicle.isPLCControlled = false;	
		FPSCam.SetActive (true);
		ThirdPSCam.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			vehicle.isPLCControlled = true;	
			FPSCam.gameObject.SetActive(true);
			ThirdPSCam.gameObject.SetActive(false);
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			vehicle.isPLCControlled = true;	
			FPSCam.gameObject.SetActive(false);
			ThirdPSCam.gameObject.SetActive(true);
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			vehicle.isPLCControlled = false;	
			FPSCam.gameObject.SetActive(false);
			ThirdPSCam.gameObject.SetActive(true);
		}

		if (Input.GetButtonDown ("NavigationArrow")) {
			NavigationArrow.gameObject.SetActive (!NavigationArrow.gameObject.activeSelf);
		}
	}
}
