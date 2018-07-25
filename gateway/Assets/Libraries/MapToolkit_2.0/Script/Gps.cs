using UnityEngine;
using System.Collections;
 
public class Gps : MonoBehaviour
{
	private GUIText gps_test;
	private Transform target;
	Manager mg;
	
	IEnumerator Start ()
	{
		mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		target = new GameObject ("target").transform;
		
		if (!Input.location.isEnabledByUser) {
		}
		Input.location.Start ();
		
		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds(0.3f);
			maxWait--;
		}
		if (maxWait < 1) {
			print ("Timed out");
		}
		if (Input.location.status == LocationServiceStatus.Failed) {
			print ("Unable to determine device location");
		} else {
			double x = double.Parse (Input.location.lastData.longitude.ToString ());
			double y = double.Parse (Input.location.lastData.latitude.ToString ());
			GPSCenter (x, y);
			DisplayUI (x, y);
		}
		Input.location.Stop ();
	}
	
	void GPSCenter (double x, double y)
	{
		target.position = mg.GIStoPos (new double[] {x, y});
		mg.SetMarkTarget (target);
		mg.RunMarkMove ();
	}
	
	void DisplayUI (double x, double y)
	{
		if (GameObject.Find ("GPS_Test") == null) {
			gps_test = new GameObject ("GPS_Test").AddComponent (typeof(GUIText))as GUIText;
		} else {
			gps_test = GameObject.Find ("GPS_Test").GetComponent<GUIText> ();
		}
		gps_test.text = "GPS Coordinate: " + "x: " + x + " ,  y:  " + y + "\n\n" + "altitude: " + Input.location.lastData.altitude + "\n\n" + "horizontalAccuracy: " + Input.location.lastData.horizontalAccuracy + "\n\n" + "Time Stamp: " + Input.location.lastData.timestamp; 
		gps_test.anchor = TextAnchor.MiddleCenter;
		gps_test.alignment = TextAlignment.Center;
		gps_test.pixelOffset = new Vector2 (Screen.width / 2, Screen.height / 2);
		gps_test.material.color = Color.red;
		gps_test.fontSize = 20;	
	}
}