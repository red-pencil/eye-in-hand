using UnityEngine;
using System.Collections;

public class NEDOVehicleGround : MonoBehaviour {

	public LayerMask targetLayer;
	public float GroundOffset=0.25f;
	public PLCDriverObject PLCSource;
	public NEDOMapRouteCreator Route;//Ugly
	public bool isPLCControlled=true;

	public Vector2 Position2D;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 origin = transform.position;
		if (isPLCControlled) {
			if (PLCSource.Connected) {
				double[] location = PLCSource.GetGNSSLocation ();
				double[] conv = new double[2];
				//Convert from GPS Lat/Lng to Geodetic Coordinates
				NEDOGPSHelper.Calc_XY (NEDOSettings.CurrentPrefectureID, location [0], location [1], ref conv [0], ref conv [1]);
				Position2D.x = (float) location [0];
				Position2D.y = (float)location [1];

				origin = Route.ProjectPoint (new Vector3 ((float)conv [0], 0, (float)conv [1]));
			}
		}else{
			//reading from keyboard
		}

		RaycastHit hit;
		origin.y += 100;
		if (Physics.Raycast (origin, Vector3.down, out hit,targetLayer.value)) {
			transform.position = new Vector3 (transform.position.x, hit.point.y+GroundOffset, transform.position.z);
			//transform.rotation = Quaternion.FromToRotation (Vector3.left, hit.normal);
		}


	}
}
