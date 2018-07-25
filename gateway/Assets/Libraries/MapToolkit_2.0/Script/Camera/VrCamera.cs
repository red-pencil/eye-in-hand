// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com

using UnityEngine;
using System.Collections;
 
public class VrCamera
{
	enum ScreenResolution
	{
		High,
		Low
	};
	protected internal Transform target;
	private float distance;
	protected internal float xSpeed = 80.0f;
	protected internal float ySpeed = 80.0f;
	protected internal int zoomRate = 20;
	protected internal float panSpeed = 0.1f;
	protected internal float zoomDampening = 20f;
	protected internal float xDeg = 0.0f;
	protected internal float yDeg = 0.0f;
	protected internal float desiredDistance;
	private Quaternion currentRotation;
	private Quaternion desiredRotation;
	private Quaternion rotation;
	private Vector3 position;
	protected internal Camera vrCamera;
	Zoom zm = new Zoom ();
	CreateTile ct = new CreateTile ();
	GetBuilding_URL gbuilding = new GetBuilding_URL ();
	public bool isInitiCreateTile;
	float baseOrthSize;

	public void Init ()
	{

		target = GameObject.Find ("3D Target").transform;
		vrCamera = GameObject.Find ("3D Camera").GetComponent<Camera> ();
		
		distance = Vector3.Distance (vrCamera.transform.position, target.position);
		desiredDistance = distance;

		position = vrCamera.transform.position;
		rotation = vrCamera.transform.rotation;
	
		currentRotation = vrCamera.transform.rotation;
		desiredRotation = vrCamera.transform.rotation;
 
		xDeg = Vector3.Angle (Vector3.right, vrCamera.transform.right);
		yDeg = Vector3.Angle (Vector3.up, vrCamera.transform.up);
	}

	public bool max;
	public bool min;

	public void Run (bool mouseDragging)
	{
		BaseOrthSize ();
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		
		float wheelSpeed = mg.sy_Map.wheelSpeed * 0.15f;
		if (!mouseDragging)
		if (mg.sy_CurrentStatus.isChangeKey || Input.GetKey (mg.sy_OtherOption.UserKey.vrModeKey)) {
			
			if (Input.GetMouseButton (0)) {	
				if (!isInitiCreateTile) {
					isInitiCreateTile = true;
					ct.InitiCreateTile ();
				}
				
				xDeg += Input.GetAxis ("Mouse X") * xSpeed * 0.02f;
				yDeg -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;

				yDeg = ClampAngle (yDeg, mg.sy_OtherOption.Camera.angleMin, mg.sy_OtherOption.Camera.angleMax);

				desiredRotation = Quaternion.Euler (yDeg, xDeg, 0);
				currentRotation = vrCamera.transform.rotation;
 
				rotation = Quaternion.Lerp (currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
				vrCamera.transform.rotation = rotation;

			} 
		}
		
		// Mouse ScrolWheel => Zoom In/Out
		float orthsizeOrigin = 0;
		float orthsizeAfter = 0;
		ZoomLimits ();
		if (Input.GetAxis ("Mouse ScrollWheel") < 0 && max.Equals (false) && !(mg.sy_CurrentStatus.isMenu) && mg.sy_CurrentStatus.isSearchbar.Equals (false)) {
			desiredDistance -= Input.GetAxis ("Mouse ScrollWheel") * Mathf.Abs (desiredDistance) * wheelSpeed;
			orthsizeOrigin = ((desiredDistance - 2.253f) / 0.173f) * 0.1f + 1.3f;
			if (orthsizeOrigin >= baseOrthSize) { 
				Camera.main.orthographicSize = orthsizeOrigin;
				orthsizeAfter = zm.OrthSize (orthsizeOrigin);
				desiredDistance = ((((orthsizeAfter - 1.3f) / 0.1f) * 0.173f) + 2.253f);
				zm.ZoomStep ();
			
			}
		} else if (Input.GetAxis ("Mouse ScrollWheel") > 0 && min.Equals (false) && !(mg.sy_CurrentStatus.isMenu) && mg.sy_CurrentStatus.isSearchbar.Equals (false)) {
			desiredDistance -= Input.GetAxis ("Mouse ScrollWheel") * Mathf.Abs (desiredDistance) * wheelSpeed;
			orthsizeOrigin = ((desiredDistance - 2.253f) / 0.173f) * 0.1f + 1.3f;
			if (orthsizeOrigin <= baseOrthSize * 0.5f) { 
				Camera.main.orthographicSize = orthsizeOrigin;
				orthsizeAfter = zm.OrthSize (orthsizeOrigin);
				desiredDistance = ((((orthsizeAfter - 1.3f) / 0.1f) * 0.173f) + 2.253f);
				zm.ZoomStep ();

			}
		} 
		desiredDistance = Mathf.Clamp (desiredDistance, 1, baseOrthSize * 2);
		position = target.position - (rotation * Vector3.forward * desiredDistance);
		vrCamera.transform.position = position;
		gbuilding.CalDistance (false);
	}
	
	// Camera Angle
	private static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
	
	void ZoomLimits ()
	{	
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
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
	
	void BaseOrthSize ()
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		ScreenResolution resolution = (ScreenResolution)mg.sy_Map.resolution;
		if (resolution.Equals (ScreenResolution.High)) {
			baseOrthSize = 4;
		} else {
			baseOrthSize = 2.6f;
		}
		if (mg.sy_OtherOption.ScreenSetting.platform == Variables.OtherOptionGroup.Platform.Mobile && (int)mg.sy_Map.orientation == 1) {
			baseOrthSize = 2.6f;
		}
	}



}