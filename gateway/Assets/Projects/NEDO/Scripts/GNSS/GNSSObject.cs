
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GNSSObject : MonoBehaviour
{
	private int iIndex = 0;
	private bool zoomChange;
	private int  zoomLevel;
	private float scaleDelta;
	private string lineParameter;
	public float  characterScale = 20;
	public LocationPoint point;

	public bool RecenterOnStart=true;

	Manager mg;

	PLCDriverObject SourceObject;

	public double[] location;

	public Text debugLabel;



	// Use this for initialization
	void Start ()
	{
		Zoom.OnChangeZoom += OnChangeZoom;	
		mg = GameObject.Find ("Manager").GetComponent<Manager> ();
		scaleDelta = transform.localScale.x * characterScale;
		zoomLevel = mg.sy_Map.zoom;
		float scalefactor = Mathf.Pow (2f, mg.sy_Map.zoom - 18);
		
		transform.localScale = new Vector3 (scalefactor * scaleDelta, scalefactor * scaleDelta, scalefactor * scaleDelta);
		
		SourceObject = GameObject.FindObjectOfType<PLCDriverObject> ();


		if (SourceObject.Connected) {
			location = SourceObject.GetGNSSLocation ();

		}
		if (RecenterOnStart) {
			mg.sy_Map.longitude_x = (location [1]);
			mg.sy_Map.latitude_y = (location [0]);
			mg.sy_Map.zoom = 17;
			mg.ChangeZoom ();
		}
	}
	
	double[] oldPos;
	
	void reCalCoordinate ()
	{

		int zoom = mg.sy_Map.zoom;
		int zoomLevel = mg.sy_Editor.zoom;
		double[] invers_pos = new double[]{location [1], location [0]};
		Vector3 convertPoint = mg.GIStoPos (invers_pos);
		Vector3 pos = new Vector3 (convertPoint.x, 0.0f, convertPoint.z);  
		transform.localPosition = pos;
		float scalefactor = Mathf.Pow (2f, zoom - zoomLevel);
		transform.localScale = new Vector3 (scalefactor * scaleDelta, scalefactor * scaleDelta, scalefactor * scaleDelta);


	}
	
	void OnChangeZoom ()
	{
		
		reCalCoordinate ();
	}
	
	void Update ()
	{	

		if (SourceObject.Connected) {
			location = SourceObject.GetGNSSLocation ();
		}
		reCalCoordinate ();

		if (debugLabel != null)
			debugLabel.text = "Vehicle Location: " + location [0].ToString ("0.00000°") + "," + location [1].ToString ("0.00000°");
//			debugLabel.text += "\nMouse Location: " + mg.sy_Coordinate.mouse [0] + "," + mg.sy_Coordinate.mouse [1];
	}
	
}
