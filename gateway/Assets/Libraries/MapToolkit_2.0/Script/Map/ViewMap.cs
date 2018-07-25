// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com

using UnityEngine;
using System.Collections;

public class ViewMap : MonoBehaviour
{
	private enum MapQuality
	{
		High,
		Low
	};

	private enum MapType
	{
		Roadmap,
		Satellite,
		Hybrid,
		Terrain
	};
	
	protected internal double[] coordin;
	protected internal string addParameter;
	private int quality;
	private string maptype;
	protected internal int zoom;
	private string language;
//	private  string mapCoordinate;
	protected internal Manager mg;
	private string licenseKey;
	
	void Start ()
	{
		License ();
		Bound ();
		SecQuality ();
		SecType ();
		SecLanguage ();	
		SecParameter ();
		StartCoroutine (Conncet ());	
	}
	
	void License ()
	{
	
		if ((int)mg.sy_Map.license == 0) {
			licenseKey = "&key=" + mg.sy_Map.apikey [0];
		} else if ((int)mg.sy_Map.license == 0) {
			licenseKey = "&client=" + mg.sy_Map.apikey [1] + "&signature=" + mg.sy_Map.apikey [2];
		}
	}
	
	void Bound ()
	{
	
		int limit = 180;		
		int right = 1;
		int left = 1;

		for (int i=1; i<10; i++) {
			if (coordin [0] > limit && coordin [0] < limit * (right + 2)) {
				coordin [0] = coordin [0] - (360 * i);
				break;
			} else {
				right += 2;
			}
			
			if (coordin [0] < -limit && coordin [0] > -limit * (left + 2)) {
				coordin [0] = coordin [0] + (360 * i);
				break;
			} else {
				left += 2;
			}
		}	
	}
	
	void SecQuality ()
	{
		MapQuality mq = (MapQuality)mg.sy_Map.quality;
		switch (mq) {
		case MapQuality.High:
			quality = 2;	
			break;
		case MapQuality.Low:
			quality = 0;
			break;
		}
	}
	
	void SecType ()
	{
		MapType mt = (MapType)mg.sy_Map.maptype;
		maptype = mt.ToString ().ToLower ();
	}
	
	void SecLanguage ()
	{
		language = mg.sy_Map.language;
	}

	void SecParameter ()
	{
		if (mg.sy_Map.addParameter == null) {
			addParameter = "";
		} else {
			addParameter = mg.sy_Map.addParameter;	
		}
	}
	
	void OnTriggerEnter (Collider col)
	{
		if (col.tag == "Respawn") {
			int col_num = int.Parse (col.name);
			int num = int.Parse (gameObject.name);
			if (num > col_num) {
				Destroy (gameObject);
			} 
		}
	}

	Vector3 pos;
	int a_zoom;
	WWW www;

	private IEnumerator Conncet ()
	{		
		a_zoom = mg.sy_Map.zoom;
		int tileLimits = 10;
		if (mg.sy_CurrentStatus.is3DCam)
			tileLimits = 25;
		yield return new WaitForSeconds(0.1f);		
		pos = Camera.main.transform.position;
		if (Mathf.Abs ((pos - transform.position).magnitude) < tileLimits && a_zoom == mg.sy_Map.zoom) {
			string mapURL = "http://maps.googleapis.com/maps/api/staticmap?center=" + coordin [1] + "," + coordin [0] + "&maptype=" + maptype + "&mobile=true" + "&zoom=" + zoom + "&size=256x256" + "&scale=" + quality + "&sensor=false&language=" + language + addParameter + licenseKey;// "&key=AIzaSyAw7g6U0w96vYINCgAWF31a6T0ksX2d2ms";
			if (mg.sy_OtherOption.ScreenSetting.platform != Variables.OtherOptionGroup.Platform.WebPlayer) {
				www = new WWW (mapURL);	
			} else {
				WWWForm form = new WWWForm ();
				form.AddField ("mapurl", mapURL);
				form.AddField ("byteSize", "100");
				if (Application.isEditor) {
					www = new WWW (mapURL);	
				} else {
					www = new WWW (mg.sy_Config.phpurl + "connect.php", form);
				}
			}
			
			yield return www;
		
			if (www.error != null) {
				www = null;
				StartCoroutine (Conncet ());
			} else {
				transform.GetComponent<MeshRenderer> ().enabled = true;	
				Texture2D mapTex = new Texture2D (4, 4, TextureFormat.ETC_RGB4, false);
				www.LoadImageIntoTexture (mapTex);
				transform.GetComponent<Renderer>().material.mainTexture = mapTex;
				www.Dispose ();
			}
		} else {
			Destroy (gameObject);	
		}
	}
}

