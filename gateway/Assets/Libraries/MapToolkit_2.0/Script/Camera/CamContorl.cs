// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com

using UnityEngine;
using System.Collections;

public class CamContorl
{
	
	private bool mouseDragging;
	private Vector3 initialTouchPosition;
	private Vector3 initialCameraPosition;
	Camera mainCam;
	Camera subCam;
	Manager mg;

	protected internal void ScreenPan ()
	{
		mg = GameObject.Find ("Manager").GetComponent<Manager> (); 

		if (Input.GetMouseButtonDown (0) && !mg.sy_CurrentStatus.isChangeKey && !Input.GetKey (mg.sy_OtherOption.UserKey.vrModeKey) && mg.sy_CurrentStatus.isSearchbar.Equals (false) && !(mg.sy_Camera.camPause)) {
			mouseDragging = true;
			initialTouchPosition = Input.mousePosition;
			initialCameraPosition = Camera.main.transform.position;
		}
		
		if (mouseDragging && !mg.sy_CurrentStatus.isChangeKey && !Input.GetKey (mg.sy_OtherOption.UserKey.vrModeKey) && mg.sy_CurrentStatus.isMenu.Equals (false) && mg.sy_CurrentStatus.isSearchbar.Equals (false) && !(mg.sy_Camera.camPause)) {
			Vector3 delta = Camera.main.ScreenToWorldPoint (Input.mousePosition) - Camera.main.ScreenToWorldPoint (initialTouchPosition);
			Vector3 newPos = initialCameraPosition;
			newPos.x -= delta.x;
			newPos.z -= delta.z;
			Camera.main.transform.position = new Vector3 (newPos.x, 1, newPos.z);
		}	
		if (Input.GetMouseButtonUp (0)) {
			if ((initialTouchPosition - Input.mousePosition).magnitude > 20 && !mg.sy_CurrentStatus.isChangeKey && !Input.GetKey (mg.sy_OtherOption.UserKey.vrModeKey) && !(mg.sy_CurrentStatus.isSearchbar)) { // MarkPoint 'OnMouseUp ()'  역으 로 작 용
				mg.sy_CurrentStatus.isMarkPoint = false;
			}
			subCam.transform.position = new Vector3 (mainCam.transform.position.x, mainCam.transform.position.z, 1);
			mouseDragging = false;
		}

	}
	
	float cameraMin = 0.1f;
	int cameraMax = 100;
	Vector3 mousePos;
	Vector3 dragOrigin;
	private bool max;
	private bool min;
	enum ScreenResolution
	{
		High,
		Low
	};
	
	protected internal void ScrollWheel ()
	{
		ZoomLimits ();
		mainCam = Camera.main;
		subCam = (Camera)GameObject.Find ("SubCamera").GetComponent<Camera> ();
		mousePos = subCam.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, subCam.transform.position.z));
		dragOrigin = subCam.ScreenToWorldPoint (new Vector3 (Screen.width / 2, Screen.height / 2, subCam.transform.position.z));  
		
		float speed_value = 12 - mg.sy_Map.wheelSpeed;
	
		if (mg.sy_CurrentStatus.isMenu.Equals (false) && mg.sy_CurrentStatus.isSearchbar.Equals (false) && !(mg.sy_Camera.camPause)) {  //zoom in
			if (Input.GetAxis ("Mouse ScrollWheel") > 0 && subCam.orthographicSize > cameraMin && min.Equals (false)) { 
				if (!mg.sy_CurrentStatus.isMarkPoint) {
					float posY = subCam.transform.position.y + ((mousePos.y - dragOrigin.y) / (subCam.orthographicSize * speed_value));  
					float posX = subCam.transform.position.x + ((mousePos.x - dragOrigin.x) / (subCam.orthographicSize * speed_value));  
					float posZ = 1; 
					subCam.transform.position = new Vector3 (posX, posY, posZ);
					mainCam.transform.position = new Vector3 (posX, 1, posY);
				}
				
				subCam.orthographicSize -= Mathf.Floor (1000 * 1f / speed_value) / 1000;
				subCam.orthographicSize = Mathf.Clamp (subCam.orthographicSize, cameraMin, cameraMax);  
					
				mainCam.orthographicSize = subCam.orthographicSize;
			}
			if (Input.GetAxis ("Mouse ScrollWheel") < 0 && subCam.orthographicSize < cameraMax && max.Equals (false) && !(mg.sy_Camera.camPause)) {  
				if (!mg.sy_CurrentStatus.isMarkPoint) {
					float posY = subCam.transform.position.y - ((mousePos.y - dragOrigin.y) / (subCam.orthographicSize * speed_value));
					float posX = subCam.transform.position.x - ((mousePos.x - dragOrigin.x) / (subCam.orthographicSize * speed_value)); 
					float posZ = 1;    
					subCam.transform.position = new Vector3 (posX, posY, posZ);
					mainCam.transform.position = new Vector3 (posX, 1, posY);
				}
				subCam.orthographicSize += Mathf.Floor (1000 * 1f / speed_value) / 1000;
				subCam.orthographicSize = Mathf.Clamp (subCam.orthographicSize, cameraMin, cameraMax);   
				mainCam.orthographicSize = subCam.orthographicSize;
			}    
		}

	}

	void ZoomLimits ()
	{	
		mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		if (mg.sy_Map.zoom.Equals (mg.sy_Map.maxLevel)) {
			min = true;	
		} else if (mg.sy_Map.zoom.Equals (mg.sy_Map.minLevel)) {
			max = true;	
		} else {
			min = false;
			max = false;
		}
	}
	
	
	protected internal void Cam3DMode ()
	{
		Camera cam3d = GameObject.Find ("3D Camera").GetComponent<Camera> (); 	
		mg = GameObject.Find ("Manager").GetComponent<Manager> (); 

		if (mg.sy_CurrentStatus.isChangeKey || Input.GetKey (mg.sy_OtherOption.UserKey.vrModeKey)) {
			mainCam.transform.rotation = Quaternion.Euler (90, cam3d.transform.eulerAngles.y, 0);	
		}
		
		if ((mg.sy_CurrentStatus.isChangeKey || Input.GetKey (mg.sy_OtherOption.UserKey.vrModeKey)) && !(mg.sy_CurrentStatus.is3DCam) && !(mg.sy_Map.zoom < 3)) {
			if (Input.GetMouseButtonDown (0)) {
				float orthsize = mainCam.orthographicSize;
				mg.sy_CurrentStatus.is3DCam = true;
				cam3d.enabled = true;
				cam3d.transform.position = new Vector3 (mainCam.transform.position.x, ((orthsize - 1.3f) / 0.1f) * 0.173f + 2.253f, mainCam.transform.position.z);
				if (cam3d.gameObject.GetComponent<GUILayer> () == null) {
					cam3d.gameObject.AddComponent<GUILayer> ();
				}
				mg.vc.Init ();	
			}
		} else if (Input.GetKeyDown (mg.sy_OtherOption.UserKey.generalModeKey) && mg.sy_CurrentStatus.is3DCam) {
			GeneralMode ();
		}
		if (mg.sy_CurrentStatus.is3DCam) {
			mg.vc.Run (mouseDragging);
		}
	}
	
	protected internal void GeneralMode ()
	{
		mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		Camera cam3d = GameObject.Find ("3D Camera").GetComponent<Camera> (); 	
		
		mg.sy_CurrentStatus.is3DCam = false;
		cam3d.enabled = false;
		cam3d.transform.rotation = Quaternion.Euler (90, 0, 0);
		mainCam.transform.rotation = Quaternion.Euler (90, 0, 0);
		if (mg.sy_OtherOption.ScreenSetting.platform != Variables.OtherOptionGroup.Platform.Mobile) {
			mg.vc.isInitiCreateTile = false;
		} else {
			mg.vrMobileCam.isInitiCreateTile = false;
		}
	}
	
	protected internal void VrMode ()
	{
		mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		mainCam = Camera.main;
		Camera cam3d = GameObject.Find ("3D Camera").GetComponent<Camera> (); 	
		mainCam.transform.rotation = Quaternion.Euler (90, cam3d.transform.eulerAngles.y, 0);	
		float orthsize = mainCam.orthographicSize;
		cam3d.transform.position = new Vector3 (mainCam.transform.position.x, ((orthsize - 1.3f) / 0.1f) * 0.173f + 2.253f, mainCam.transform.position.z);
		if (cam3d.gameObject.GetComponent<GUILayer> () == null) {
			cam3d.gameObject.AddComponent<GUILayer> ();
		}
		if (mg.sy_OtherOption.ScreenSetting.platform != Variables.OtherOptionGroup.Platform.Mobile) {
			mg.vc.Init ();
		} else {
			mg.vrMobileCam.one = true;
		}
		mg.sy_CurrentStatus.is3DCam = true;
		cam3d.enabled = true;
	}
}
