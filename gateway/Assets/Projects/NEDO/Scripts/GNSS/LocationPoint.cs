using UnityEngine;
using System.Collections;

public class LocationPoint : MonoBehaviour
{

	public double latitude;
	public double longitude;
	private float objectScale;
	Manager mg;

	void Start ()
	{
		mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		Zoom.OnChangeZoom += OnChangeZoom;
		objectScale = gameObject.transform.localScale.x;
		double[] point = {longitude,latitude };
		Vector3 convertPoint = mg.GIStoPos (point);
		Vector3 pos = new Vector3 (convertPoint.x, 0.01f, convertPoint.z);  
		gameObject.transform.position = pos;
	}

	void OnChangeZoom ()
	{
		int zoom = mg.sy_Map.zoom;
		int zoomLevel = mg.sy_Editor.zoom;
		double[] point = {longitude,latitude };
		Vector3 convertPoint = mg.GIStoPos (point);
		Vector3 pos = new Vector3 (convertPoint.x, 0.01f, convertPoint.z);  
		gameObject.transform.position = pos;
		float scalefactor = Mathf.Pow (2f, zoom - zoomLevel);
		transform.localScale = new Vector3 (scalefactor * objectScale, scalefactor * objectScale, scalefactor * objectScale);
	}
}

