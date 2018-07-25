// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com

using UnityEngine;
using System.Collections;

public class CreateTile
{	
	
	int _happen_x;
	int _happen_z;
	int bg_happen_x;
	int bg_happen_z;
	float cameraRadious;
	float tileSize = 2;
	float bgTileSize = 8f;
	CreateMesh cmesh = new CreateMesh ();
	
	protected internal void InitiCreateTile ()
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> ();
		mg.AddMap (cmesh.TempTile (Vector2.zero, tileSize), Vector2.zero);
		if (mg.sy_CurrentStatus.is3DCam) {
			mg.AddMap (cmesh.TempTile (Vector2.zero, bgTileSize), Vector2.zero);
		}
	}
	
	protected internal void AfterCreateTile (bool is3DCam)
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> ();
		Vector3 camPos = Camera.main.transform.position;

		int tileNum_x = (int)Mathf.Round (camPos.x / tileSize);
		int tileNum_y = (int)Mathf.Round (camPos.z / tileSize);	
		
		if (_happen_x != tileNum_x) {
			mg.AddMap (cmesh.TempTile (new Vector2 (tileNum_x * tileSize, tileNum_y * tileSize), tileSize), new Vector2 (tileNum_x, tileNum_y));
			DelTile (camPos);
			_happen_x = tileNum_x;
		}
		if (_happen_z != tileNum_y) {
			mg.AddMap (cmesh.TempTile (new Vector2 (tileNum_x * tileSize, tileNum_y * tileSize), tileSize), new Vector2 (tileNum_x, tileNum_y));
			DelTile (camPos);
			_happen_z = tileNum_y;
		}
		if (is3DCam) {
		
			int bgTileNum_x = (int)Mathf.Round (camPos.x / bgTileSize);
			int bgTileNum_y = (int)Mathf.Round (camPos.z / bgTileSize);	
			
			if (bg_happen_x != bgTileNum_x) {
				mg.AddMap (cmesh.TempTile (new Vector2 (bgTileNum_x * bgTileSize, bgTileNum_y * bgTileSize), bgTileSize), new Vector2 (bgTileNum_x, bgTileNum_y));
				bg_happen_x = bgTileNum_x;
			}
			if (bg_happen_z != bgTileNum_y) {
				mg.AddMap (cmesh.TempTile (new Vector2 (bgTileNum_x * bgTileSize, bgTileNum_y * bgTileSize), bgTileSize), new Vector2 (bgTileNum_x, bgTileNum_y));
				bg_happen_z = bgTileNum_y;
			}
		}
		
	}

	protected internal IEnumerator AddMap (Transform target, Vector2 camTilePos)
	{
		Calculation cal = new Calculation ();
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> ();
		double[] coordin = new double[]{mg.sy_Map.longitude_x,mg.sy_Map.latitude_y};
		for (int i =0; i<target.childCount; i++) {
			Transform obj = target.GetChild (i);
			cal.AddMap (obj, coordin, camTilePos);
			obj.name = i.ToString (); 
		}
		yield break;
	}

	protected internal void DelTile (Vector3 camPos)
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> ();
		if (mg.sy_OtherOption.ScreenSetting.platform != Variables.OtherOptionGroup.Platform.Mobile) {
			cameraRadious = 35;
		} else {
			if (mg.sy_CurrentStatus.is3DCam == true)
				cameraRadious = 18;
			else
				cameraRadious = 10;	
		}
		for (int k=0; k<GameObject.Find("TileGroup").transform.childCount; k++) {	
			GameObject tile = GameObject.Find ("TileGroup").transform.GetChild (k).gameObject;
			Vector3 distPos = new Vector3 (tile.transform.position.x, camPos.y, tile.transform.position.z);
			float distance = Vector3.Distance (camPos, distPos);
			if (distance > cameraRadious) {
				tile.transform.GetComponent<Renderer>().material.mainTexture = null;
				GameObject.Destroy (tile);
			}
		}
	}
	
}
