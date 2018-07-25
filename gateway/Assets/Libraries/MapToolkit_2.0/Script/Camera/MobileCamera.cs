// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com

using UnityEngine;
using System.Collections;

public class MobileCamera
{

	private bool drag = false;
	private bool zoom = false;
	private Vector3 initialTouchPosition;
	private Vector3 initialCameraPosition;
	private Vector3 initialTouch0Position;
	private Vector3 initialTouch1Position;
	private Vector3 initialMidPointScreen;
	private float initialOrthographicSize;
	Zoom zm = new Zoom ();
	Manager mg;

	protected internal void Run ()
	{
		Camera cam = Camera.main.GetComponent<Camera>();
		mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		OrthSizeSetting ();
		
		if (mg.sy_CurrentStatus.is3DCam) {
			mg.vrMobileCam.Run (false);
		} else {
			
			if (Input.touchCount == 1) {
				zoom = false;
				Touch touch0 = Input.GetTouch (0);

				if (IsTouching (touch0)) {
					if (!drag) {
						initialTouchPosition = touch0.position;
						initialCameraPosition = cam.transform.position;

						drag = true;
					} else {
						Vector3 delta = cam.ScreenToWorldPoint (touch0.position) -
                                    cam.ScreenToWorldPoint (initialTouchPosition);
					

						Vector3 newPos = initialCameraPosition;
						newPos.x -= delta.x;
						newPos.z -= delta.z;

						cam.transform.position = newPos;
						if (touch0.phase == TouchPhase.Moved) {
							mg.sy_CurrentStatus.isMarkPoint = false;
						}
					}
				} else {
					drag = false;
				}
			} else {
				drag = false;
			}

			if (Input.touchCount == 2) {
				drag = false;
				Touch touch0 = Input.GetTouch (0);
				Touch touch1 = Input.GetTouch (1);

				if (!zoom) {
					initialTouch0Position = touch0.position;
					initialTouch1Position = touch1.position;
					initialCameraPosition = cam.transform.position;
					initialOrthographicSize = Camera.main.orthographicSize;
					initialMidPointScreen = (touch0.position + touch1.position) / 2;

					zoom = true;
				} else {
					cam.transform.position = initialCameraPosition;
					cam.orthographicSize = initialOrthographicSize;

					float scaleFactor = GetScaleFactor (touch0.position,
                                                   touch1.position,
                                                   initialTouch0Position,
                                                   initialTouch1Position);
					//	if ((TouchStatus == "Zoom In" && mg.zm.CatchZoomLevel (initialOrthographicSize / scaleFactor) < 20) ||
					//		(TouchStatus == "Zoom Out" && mg.zm.CatchZoomLevel (initialOrthographicSize / scaleFactor) > 1)) {
					
					Vector2 currentMidPoint = (touch0.position + touch1.position) / 2;
					Vector3 initialMidPointWorldBeforeZoom = cam.ScreenToWorldPoint (initialMidPointScreen);
			
					Camera.main.orthographicSize = initialOrthographicSize / scaleFactor;
					
					if (!mg.sy_CurrentStatus.isMarkPoint) {
						Vector3 initialMidPointWorldAfterZoom = cam.ScreenToWorldPoint (initialMidPointScreen);
						Vector3 initialMidPointDelta = initialMidPointWorldBeforeZoom - initialMidPointWorldAfterZoom;

						Vector3 oldAndNewMidPointDelta =
                    cam.ScreenToWorldPoint (currentMidPoint) -
                    cam.ScreenToWorldPoint (initialMidPointScreen);

						Vector3 newPos = initialCameraPosition;
						newPos.x -= oldAndNewMidPointDelta.x - initialMidPointDelta.x;
						newPos.z -= oldAndNewMidPointDelta.z - initialMidPointDelta.z;

						cam.transform.position = newPos;
					}
					SetLimits2 ();
					if (Input.GetTouch (0).phase == TouchPhase.Ended || Input.GetTouch (1).phase == TouchPhase.Ended) {
						zm.ZoomStep ();
					}
	
				}
			} else {
				zoom = false;
			}
		}
	}
	/*
	private float initTouchDist; // Touch Point 1 and 2's Distance
	private string TouchStatus = "Idle";

	void SetLimits ()
	{
		if (Input.touchCount > 1) { // Touch Point Count : over 2
			if (Vector2.Distance (Input.GetTouch (0).position, Input.GetTouch (1).position) > initTouchDist) {
				TouchStatus = "Zoom In";
			} else if (Vector2.Distance (Input.GetTouch (0).position, Input.GetTouch (1).position) < initTouchDist) {
				TouchStatus = "Zoom Out";
			}
 
			initTouchDist = Vector2.Distance (Input.GetTouch (0).position, Input.GetTouch (1).position);
		} else
			TouchStatus = "Idle";
	}
	*/
	void SetLimits2 ()
	{
		int _zoom = mg.sy_Map.zoom;
		int min = Seung2 (2, Mathf.Abs (_zoom - mg.sy_Map.minLevel + 1));
		int max = Seung2 (2, Mathf.Abs (_zoom - mg.sy_Map.maxLevel - 1));
		Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, (baseOrthSize / max) * 1.001f, (baseOrthSize * 0.5f * min * 0.99f));		
	}

	float baseOrthSize;

	void OrthSizeSetting ()
	{
		if ((int)mg.sy_Map.resolution == 0) {
			baseOrthSize = 4;
		} else if ((int)mg.sy_Map.resolution == 1) {
			baseOrthSize = 2.6f;
		}
		if (mg.sy_OtherOption.ScreenSetting.platform == Variables.OtherOptionGroup.Platform.Mobile && (int)mg.sy_Map.orientation == 1) {
			baseOrthSize = 2.6f;
		}
	}
	
	int Seung (int a, float b)
	{
		int c = a;
		int i = 0;
		while (c<=b) {
			c *= a;
			i++;
		}
		return i;
	}
	
	int Seung2 (int a, int b)
	{
		int c = 1;
		int i = 0;
		for (i = 0; i < b; i++) {
			c *= a;
		}
		return c;
	}

	static bool IsTouching (Touch touch)
	{
		return touch.phase == TouchPhase.Began ||
                touch.phase == TouchPhase.Moved ||
                touch.phase == TouchPhase.Stationary;
	}

	protected internal float GetScaleFactor (Vector2 position1, Vector2 position2, Vector2 oldPosition1, Vector2 oldPosition2)
	{
		float distance = Vector2.Distance (position1, position2);
		float oldDistance = Vector2.Distance (oldPosition1, oldPosition2);

		if (oldDistance == 0 || distance == 0) {
			return 1.0f;
		}

		return distance / oldDistance;
	}


}
