using UnityEngine;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Collections.Generic;

public class GNSSRoute : MonoBehaviour
{


	public string RouteListPath;
	private ArrayList locationList;
	private ArrayList locationListScreen;
	private string lineParameter;
	Manager mg;

	public GNSSRouteServer _server;

	bool _creating=false;


	// Use this for initialization
	void Start ()
	{
		Zoom.OnChangeZoom += OnChangeZoom;	
		mg = GameObject.Find ("Manager").GetComponent<Manager> ();
		locationList = new ArrayList ();
		
		locationListScreen = new ArrayList ();

		LoadPoints ();
		
		for (int i = 0; i < locationList.Count; i++) {
			double[] d_pos = (double[])locationList [i];
			double[] point = { (double)d_pos [1],  (double)d_pos [0] };
			Vector3 pos = mg.GIStoPos (point);  
			locationListScreen.Add (pos);
		}
		_updateLineParameter ();

	}

	void OnDestroy()
	{
		SavePoints ();
	}

	void reCalCoordinate ()
	{
		locationListScreen = new ArrayList ();
		for (int i=0; i< locationList.Count; i++) {
			
			double[] d_pos = (double[])locationList [i];
			double[] invers_pos = new double[]{d_pos [1], d_pos [0]};
			Vector3 pos3 = mg.GIStoPos (invers_pos);
			
			locationListScreen.Add (pos3);
			
		}

	}

	void OnChangeZoom ()
	{

		reCalCoordinate ();
	}


	void _updateLineParameter()
	{
		lineParameter = "&path=color:0xff0030";
		for (int i=0; i< locationList.Count; i++) {
			double[] d_pos = (double[])locationList [i];
			lineParameter += "|" + d_pos [0].ToString () + "," + d_pos [1].ToString ();
		}
		#if !(UNITY_IPHONE)
		mg.sy_Map.addParameter = lineParameter;
		mg.RefreshMap();
		#endif
	}

	void UpdateClients()
	{
		if(_server.IsServer)
			_server.UpdateClients (locationList);
	}

	void _updatePoints()
	{
		_updateLineParameter ();
		OnChangeZoom ();
		UpdateClients();
	}
	void _CheckMouse()
	{
		if (Input.GetMouseButtonUp(0)) {
			Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			double[] d_pos= mg.PostoGIS(pos);
			locationList.Add (new double[]{d_pos[1],d_pos[0]});
			Debug.Log (d_pos[0]+","+d_pos[1]);
			_updatePoints ();
		}
		if (Input.GetMouseButtonUp (1)) {
			RemoveLast ();
		}
		if (Input.GetKey (KeyCode.A))
			ClearPoints ();
	}

	void LateUpdate ()
	{	
		if (Input.GetKey(KeyCode.Space)) {
			_CheckMouse ();
		}
	}


	public void StartCreation()
	{
		_creating = true;
	}

	public void StopCreation()
	{
		_creating = false;
	}

	public void ClearPoints()
	{
		if (locationList.Count > 0) {
			locationList.Clear ();
			_updatePoints ();
		}
	}

	public void RemoveLast()
	{
		if (locationList.Count > 0) {
			locationList.RemoveAt (locationList.Count - 1);
			_updatePoints ();
		}
	}

	void LoadPoints()
	{
		try{
			using (StreamReader reader = new StreamReader (Application.dataPath + "\\" + RouteListPath)) {
				while (!reader.EndOfStream) {
					string line = reader.ReadLine ();

					string[] parts = line.Split (",".ToCharArray ());
					locationList.Add (new double[]{ double.Parse (parts [0]), double.Parse (parts [1]) });
				}
				reader.Close ();
			}
		}catch(Exception e) {
			Debug.Log (e.Message);
		}

	}

	public void SavePoints()
	{

		try{
			using (StreamWriter writer = new StreamWriter(Application.dataPath + "\\" + RouteListPath)) {
				for (int i = 0; i < locationList.Count; ++i) {
					double[] points=(double[] )locationList [i];
					writer.WriteLine (string.Format ("{0},{1}", points [0], points [1]));
				}
				writer.Close ();
			}
		}catch(Exception e) {
			Debug.Log (e.Message);
		}

	}

}
