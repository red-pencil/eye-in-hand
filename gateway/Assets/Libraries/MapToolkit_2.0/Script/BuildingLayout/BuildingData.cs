// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com
using UnityEngine;
using System.Collections;

public class BuildingData : MonoBehaviour
{
	public string coordinate;
	public int zoom;
	public int sy_zoom;
	public string fileName;
	public AssetBundle bundle;
	Manager mg;
	GISHelp gHelp = new GISHelp ();

	void Awake ()
	{
		mg = (Manager)GameObject.Find ("Manager").GetComponent<Manager> ();	
	}
	
	void Start ()
	{

		
		if (transform.parent.name != "> Building Layout") {
			int temp_zoomlevel = sy_zoom - zoom;
			if (temp_zoomlevel > 0) {
				transform.localScale *= Mathf.Pow (2, Mathf.Abs (sy_zoom - zoom));
			} else {
				transform.localScale /= Mathf.Pow (2, Mathf.Abs (sy_zoom - zoom));
			}		
			RefreshPosition ();

		}
	}

	void Update ()
	{
		/*
		if (!mg.sy_CurrentStatus.is3DCam && transform.parent.name != "> Building Layout") {
			mg.sy_Building.activated_building_name.Remove (fileName);
			bundle.Unload (true);
			Destroy (gameObject);	
		}
		*/
	}
	
	public void RefreshPosition ()
	{
		string[] cords = coordinate.Split (',');	
		double[] target = {double.Parse (cords [0]),double.Parse (cords [1])};
		double[] modelingPos = gHelp.GetPixelDelta (target);	
		transform.position = new Vector3 ((float)modelingPos [0], transform.position.y, (float)modelingPos [1]);
	}

}
