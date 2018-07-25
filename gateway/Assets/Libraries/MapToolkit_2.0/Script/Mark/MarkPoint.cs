// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com

using UnityEngine;
using System.Collections;

public class MarkPoint : MonoBehaviour
{
	protected internal Manager mg;
	protected internal Transform target;
	public string markCoordinate;
	protected internal Vector3 offset = Vector3.up;
	private Camera cam;
	private Transform thisTransform;
	protected internal Texture2D hoverMouseSprite;
	protected internal Texture2D originSprite;
	protected internal Texture2D activeSprite;
	GISHelp gHelp = new GISHelp ();
	
	void Start ()
	{
		thisTransform = transform;
		originSprite = gameObject.GetComponent<GUITexture> ().texture as Texture2D;
		RefreshPos ();
	}

	protected internal void RefreshPos ()
	{
		Vector3 pos = MarkPos (markCoordinate);
		target.position = pos;
	}
	
	protected internal Vector3 MarkPos (string target)
	{
		string[] str = target.Split (',');
		double[] d = {double.Parse (str [0]), double.Parse (str [1])};
		double[] test = gHelp.GetPixelDelta (d);
		Vector3 pos = new Vector3 ((float)test [0], 0, (float)test [1]);  
		return pos;
	}
	
	void Update ()
	{
		if (mg.sy_CurrentStatus.is3DCam) {
			cam = GameObject.Find ("3D Camera").GetComponent<Camera> (); 	
		} else {
			cam = Camera.main;
		}
		thisTransform.position = cam.WorldToViewportPoint (target.position + offset);
		
		if (Camera.main.GetComponent<MarkMove> ().target != null) {
			if (Camera.main.GetComponent<MarkMove> ().target.name == gameObject.name && mg.sy_CurrentStatus.isMarkPoint) {
				GetComponent<GUITexture>().texture = activeSprite;
			} else {
				if (!mouseEvent)
					GetComponent<GUITexture>().texture = originSprite;
			}
		} 
	}
	
	bool mouseEvent;

	void OnMouseEnter ()
	{
		mouseEvent = true;
		if (hoverMouseSprite) {
			GetComponent<GUITexture>().texture = hoverMouseSprite;
		}
	}
	
	void OnMouseExit ()
	{
		mouseEvent = false;
		if (hoverMouseSprite) {
			GetComponent<GUITexture>().texture = originSprite;
		}
	}

	Vector3 clickPos;

	void  OnMouseDown ()
	{
		clickPos = Input.mousePosition;
	}

	void  OnMouseUp ()
	{
		if ((clickPos - Input.mousePosition).magnitude < 20) {
			Camera mainCam = Camera.main;
			mainCam.GetComponent<MarkMove> ().target = target;
			mainCam.GetComponent<MarkMove> ().RunMove ();
			mg.sy_CurrentStatus.isMarkPoint = true;
		} 
	}

}
