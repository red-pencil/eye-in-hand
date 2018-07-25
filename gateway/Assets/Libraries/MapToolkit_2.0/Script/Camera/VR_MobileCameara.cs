// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com

using UnityEngine;
using System.Collections;

public class VR_MobileCameara
{

	enum ScreenResolution
	{
		High,
		Low
	};
	protected internal Transform target;
	protected internal float distance;
	private float desiredDistance;
	protected internal Camera vrCamera;
	private Quaternion currentRotation;
	private Quaternion desiredRotation;
	private Quaternion rotation;
	private Vector3 position;
	private float xDeg = 0.0f;
	private float yDeg = 0.0f;
	private float zoomDampening = 20f;
	protected internal bool isInitiCreateTile;
	CreateTile ct = new CreateTile ();
	Vector2 curDist = Vector2.zero;
	Vector2 prevDist = Vector2.zero;
	bool isZooming = false;
	float baseOrthSize;
	
	private void Init ()
	{
		distance = Vector3.Distance (vrCamera.transform.position, target.position);
		desiredDistance = distance;

		position = vrCamera.transform.position;
		rotation = vrCamera.transform.rotation;
	
		currentRotation = vrCamera.transform.rotation;
		desiredRotation = vrCamera.transform.rotation;
 
		xDeg = Vector3.Angle (Vector3.right, vrCamera.transform.right);
		yDeg = Vector3.Angle (Vector3.up, vrCamera.transform.up);
		
	}

	private bool max;
	private bool min;
	protected internal bool one;
	private bool isPanning ;

	protected internal void Run (bool mouseDragging)
	{
		BaseOrthSize ();
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		target = GameObject.Find ("3D Target").transform;
		vrCamera = GameObject.Find ("3D Camera").GetComponent<Camera> ();
		float zoomRate = mg.sy_OtherOption.Mobile.zoomRate * 0.001f;
		float panSpeed = mg.sy_OtherOption.Mobile.panSpeed * 0.001f;
		float rotateSpeed = mg.sy_OtherOption.Mobile.rotateSpeed * 5f;

		if (one) {
			Init ();
			one = false;
		}

		if (Input.touchCount == 1) {
			Touch _touch = Input.GetTouch (0);
			if (_touch.phase == TouchPhase.Moved) {	

				Vector3 _touch_delta = new Vector3 (_touch.deltaPosition.x, -_touch.deltaPosition.y, 0);
			
				xDeg += Mathf.Deg2Rad * _touch_delta.x * rotateSpeed;
				yDeg += Mathf.Deg2Rad * _touch_delta.y * rotateSpeed;
				
				if (!isInitiCreateTile) {
					isInitiCreateTile = true;
					ct.InitiCreateTile ();
				}
				
				yDeg = ClampAngle (yDeg, mg.sy_OtherOption.Camera.angleMin, mg.sy_OtherOption.Camera.angleMax);

				desiredRotation = Quaternion.Euler (yDeg, xDeg, 0);
				currentRotation = vrCamera.transform.rotation;
 
				rotation = Quaternion.Lerp (currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
				vrCamera.transform.rotation = rotation;
				Camera.main.transform.rotation = Quaternion.Euler (90, vrCamera.transform.eulerAngles.y, 0);	
			} 
		}
		
		if (Input.touchCount > 1) { 
			ZoomLimits ();
			if (Input.GetTouch (0).phase == TouchPhase.Began && Input.GetTouch (1).phase == TouchPhase.Began) {
				test = mg.sy_Map.zoom;
			}
			
			Vector3 touch1Dir = Input.touches [0].deltaPosition.normalized;
			Vector3 touch2Dir = Input.touches [1].deltaPosition.normalized;
			float dotProduct = Vector2.Dot (touch1Dir, touch2Dir);
			
			if (dotProduct > 0) {
				isPanning = true;
				isZooming = false;
			}
			if (dotProduct < 0) {
				isZooming = true;
				isPanning = false;
			}
			Vector2 touchDeltaPosition = Input.GetTouch (0).deltaPosition;
			if (isPanning) {
				Camera.main.transform.Translate (-touchDeltaPosition.x * panSpeed, -touchDeltaPosition.y * panSpeed, 0);	
				if (touchDeltaPosition.magnitude > 5) 
					mg.sy_CurrentStatus.isMarkPoint = false;
			} 
		
			if (isZooming) {
				float orthsizeOrigin = 0;
				float orthsizeAfter = 0;
				//------------------------------------------------------------------------------Zoom
				curDist = Input.GetTouch (0).position - Input.GetTouch (1).position;
				prevDist = (Input.GetTouch (0).position - Input.GetTouch (0).deltaPosition) - (Input.GetTouch (1).position - Input.GetTouch (1).deltaPosition); 
				float deltaValue = curDist.magnitude - prevDist.magnitude;
				float currentDistSq = curDist.sqrMagnitude;
				float prevDistSq = prevDist.sqrMagnitude;
				
			
				if (currentDistSq > prevDistSq + 0.5f && min.Equals (false) && !(mg.sy_CurrentStatus.isMenu) && mg.sy_CurrentStatus.isSearchbar.Equals (false)) {
					desiredDistance -= Mathf.Abs (deltaValue) * zoomRate;
					orthsizeOrigin = ((desiredDistance - 2.253f) / 0.173f) * 0.1f + 1.3f;
				
					orthsizeAfter = mg.zm.OrthSize (orthsizeOrigin);
					Camera.main.orthographicSize = orthsizeAfter;
					desiredDistance = ((((orthsizeAfter - 1.3f) / 0.1f) * 0.173f) + 2.253f);
					mg.zm.MobileZoomStep (orthsizeOrigin);
				}
	
				if (currentDistSq < prevDistSq - 0.5f && max.Equals (false) && !(mg.sy_CurrentStatus.isMenu) && mg.sy_CurrentStatus.isSearchbar.Equals (false)) {
					desiredDistance -= -(Mathf.Abs (deltaValue) * zoomRate);
					orthsizeOrigin = ((desiredDistance - 2.253f) / 0.173f) * 0.1f + 1.3f;
		
					orthsizeAfter = mg.zm.OrthSize (orthsizeOrigin);
					Camera.main.orthographicSize = orthsizeAfter;
					desiredDistance = ((((orthsizeAfter - 1.3f) / 0.1f) * 0.173f) + 2.253f);
					mg.zm.MobileZoomStep (orthsizeOrigin);
					
				}
			}
			if (Input.GetTouch (0).phase == TouchPhase.Ended || Input.GetTouch (1).phase == TouchPhase.Ended) {
				mg.gbuilding.CalDistance (true);
				if (test != mg.sy_Map.zoom) {
					mg.Vr_Mobile_ChangeTheMap (); 
				}
			}
		}
		
		desiredDistance = Mathf.Clamp (desiredDistance, 1, baseOrthSize * 2);
		position = target.position - (rotation * Vector3.forward * desiredDistance);
		vrCamera.transform.position = position;
	}

	int test;

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
		if (mg.sy_Map.zoom >= mg.sy_Map.maxLevel) {
			min = true;	
		} else if (mg.sy_Map.zoom <= mg.sy_Map.minLevel) {
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
