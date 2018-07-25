using UnityEngine;
using System.Collections;

public class NEDODirectionPointer : MonoBehaviour {

	public NEDOMapRouteCreator Route;
	public Transform Vehicle;
	public float MinimumDistance = 2;
	public Transform indicator;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		int index=Route.GetClosestPointIndex (Vehicle.position);


		Vector3 dir; 
		Vector3 dir1;
		Vector3 dir2;


		dir1 = Route.GetPointAt (index)-Vehicle.position;
		dir1.y = 0;
		if (index < Route.GetPointsCount () - 1) {
			dir2 = Route.GetPointAt (index + 1) - Vehicle.position;
			dir2.y = 0;

			if (dir1.sqrMagnitude > MinimumDistance * MinimumDistance) {
				indicator.position = Route.GetPointAt (index);
				dir = dir1;
			} else {
				indicator.position = Route.GetPointAt (index+1);
				dir = dir2;
			}
		} else
			dir = dir1;

		dir.Normalize ();
		float yaw=Quaternion.LookRotation (dir, Vector3.up).eulerAngles.y;

		yaw-=Vehicle.eulerAngles.y;

		transform.rotation = Quaternion.AngleAxis (yaw, Vector3.up);	
	}
	
}
