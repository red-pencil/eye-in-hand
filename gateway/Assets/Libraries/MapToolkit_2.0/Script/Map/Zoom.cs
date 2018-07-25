// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com

using UnityEngine;
using System.Collections;

public class Zoom
{
	public delegate void ZoomChangeHandler ();

	public static event ZoomChangeHandler OnChangeZoom;
	
	public delegate void ZoomStateHandler (float deltaScale);

	public static event ZoomStateHandler OnZoomState;
	
	public delegate void RefreshMapHandler (Vector3 initiCamPos);

	public static event RefreshMapHandler OnRefreshMap;
	
	enum ScreenResolution
	{
		High,
		Low
	};
	private float baseOrthSize;
	float camOrthSize;
	private Vector3 initiCamPos;
	CreateTile ct = new CreateTile ();
	int tileCount;
	int limitsTileCount;
	GISHelp gis = new GISHelp ();

	void DefinePlatform ()
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		if (mg.sy_OtherOption.ScreenSetting.platform == Variables.OtherOptionGroup.Platform.Mobile) { 
			tileCount = 6;
			limitsTileCount = 6;
		} else {
			tileCount = 20;
			if (mg.sy_CurrentStatus.is3DCam) {
				limitsTileCount = 35;
			} else {
				limitsTileCount = 20;
			}
		}
	}
	
	protected internal void ChangeZoom (int zoom)
	{	 
		if (OnChangeZoom != null) {
			OnChangeZoom ();
		}
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		if (!(mg.sy_OtherOption.ScreenSetting.platform == Variables.OtherOptionGroup.Platform.Mobile && mg.sy_CurrentStatus.is3DCam)) {
			Camera.main.transform.position = new Vector3 (0, 1, 0);	
		}
		GameObject tileGroup = GameObject.Find ("TileGroup");
		for (int i=0; i<tileGroup.transform.childCount; i++) {
			GameObject.Destroy (tileGroup.transform.GetChild (i).gameObject);
		}
		
		ct.InitiCreateTile ();
		RefreshMark ();
		MarkCorrections ();
		
		if (mg.sy_Map.activeMarkBuilding && mg.sy_CurrentStatus.is3DCam) {
			SetModeling ("");
			Test7 ();
			if (mg.sy_OtherOption.ScreenSetting.platform == Variables.OtherOptionGroup.Platform.Mobile) {
				mg.StartDelete ();
			}
		}
	}

	protected internal void Vr_ChangeZoom (int zoom)
	{		
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		if (!(mg.sy_OtherOption.ScreenSetting.platform == Variables.OtherOptionGroup.Platform.Mobile && mg.sy_CurrentStatus.is3DCam)) {
			Camera.main.transform.position = new Vector3 (0, 1, 0);	
			GameObject tileGroup = GameObject.Find ("TileGroup");
			for (int i=0; i<tileGroup.transform.childCount; i++) {
				GameObject.Destroy (tileGroup.transform.GetChild (i).gameObject);
			}
		}
		
		ct.InitiCreateTile ();
		RefreshMark ();
		MarkCorrections ();
		
		if (mg.sy_Map.activeMarkBuilding && mg.sy_CurrentStatus.is3DCam) {
			SetModeling ("");
			Test7 ();
			if (mg.sy_OtherOption.ScreenSetting.platform == Variables.OtherOptionGroup.Platform.Mobile) {
				mg.StartDelete ();
			}
		}
	}
	
	void MarkCorrections ()  //마크의 Position이 확대/축소시 변경되어 마크중점이 어긋나는 현상이 발생 : 값을 실시간으로 잡아주기 위해 확대/축소 하는 순간에 타켓의 위치를 새로이 갱신
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		if (mg.sy_CurrentStatus.isMarkPoint) {
			if (Camera.main.GetComponent<MarkMove> ().target != null)
				Camera.main.GetComponent<MarkMove> ().Test ();
		}
	}
	
	protected internal void RefreshMark ()
	{
		GameObject markTarget = GameObject.Find ("Setting");
		for (int i =0; i<markTarget.transform.childCount; i++) {
			for (int k =0; k<markTarget.transform.GetChild (i).childCount; k++) {
				markTarget.transform.GetChild (i).GetChild (k). GetComponent<MarkPoint> ().RefreshPos ();
			}
		}
	}
	
	protected internal void RefreshMap ()
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		initiCamPos = Camera.main.transform.position;
		ChangeCoordinate ();
		tempTileScale = 1;
		Camera.main.transform.position = new Vector3 (0, 1, 0);	
		ChangeZoom (mg.sy_Map.zoom);
		AfterImage ("in");          
		mg.StartDelete ();        
		
		if (OnRefreshMap != null) {
			OnRefreshMap (initiCamPos);
		}
		
	}
	
	protected internal IEnumerator DelAfterImage ()
	{
		DefinePlatform ();
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		yield return new WaitForSeconds(0.2f);
		
		int k = 0;
		if (GameObject.Find ("TileGroup") != null) {
			GameObject tile = GameObject.Find ("TileGroup");
			for (int i = 0; i< tile.transform.childCount; i++) {
				if (tile.transform.GetChild (i).GetComponent<MeshRenderer> ().enabled == true) {
					k++;
				}
			}	
		}
		//Debug.Log (tileCount);
		if (k > tileCount) { 
			if (GameObject.Find ("StepOne") != null) {
				GameObject.Destroy (GameObject.Find ("StepOne"));
			}
			if (GameObject.Find ("StepTwo") != null) {
				GameObject.Destroy (GameObject.Find ("StepTwo"));
			}
			if (GameObject.Find ("StepThree") != null) {
				GameObject.Destroy (GameObject.Find ("StepThree"));
			}
			if (GameObject.Find ("StepFour") != null) {
				GameObject.Destroy (GameObject.Find ("StepFour"));
			}	
			if (GameObject.Find ("StepFive") != null) {
				GameObject.Destroy (GameObject.Find ("StepFive"));
			}	
			if (GameObject.Find ("StepSix") != null) {
				GameObject.Destroy (GameObject.Find ("StepSix"));
			}	
		} else {
			mg.StartDelete ();          
		}
	}
	
	void SubCamSet ()
	{
		Camera subCam = (Camera)GameObject.Find ("SubCamera").GetComponent<Camera> ();
		subCam.orthographicSize = Camera.main.orthographicSize;
		subCam.transform.position = Vector3.zero;
	}
	
	protected internal void InitiCamPosRest (double[] targetPos)
	{
		initiCamPos = new Vector3 ((float)gis.GetPixelDelta (targetPos) [0], 1, (float)gis.GetPixelDelta (targetPos) [1]);	
	}
	
	protected internal void  ZoomStep ()
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		camOrthSize = Camera.main.orthographicSize;
		initiCamPos = new Vector3 ((float)gis.GetPixelDelta (mg.sy_Coordinate.center) [0], 1, (float)gis.GetPixelDelta (mg.sy_Coordinate.center) [1]);
		
		OrthSizeSetting (); 

		if (camOrthSize > baseOrthSize) {
			int bigZoom = mg.sy_Map.zoom - (Seung (2, camOrthSize / baseOrthSize) + 1);
     
			if (mg.sy_Map.zoom != bigZoom) {
				mg.sy_Map.zoom = bigZoom;
	
				ChangeCoordinate ();  
				GetScale ();                   
				DelAfterImage ("out");      
				ChangeZoom (mg.sy_Map.zoom); 
				AfterImage ("out");           
				mg.StartDelete ();           
				
				float smallZoom = ZoomDetail ("out");
				Camera.main.orthographicSize = baseOrthSize - (baseOrthSize - smallZoom) / 2;
				SubCamSet ();
				SetModeling ("out");
				if (OnZoomState != null)
					OnZoomState (tempTileScale);
			}

		} else {
			int bigZoom = mg.sy_Map.zoom + (Seung (2, baseOrthSize / camOrthSize));
    
			if (mg.sy_Map.zoom != bigZoom) {
				mg.sy_Map.zoom = bigZoom;
				ChangeCoordinate ();
				GetScale ();                        
				DelAfterImage ("in");
				ChangeZoom (bigZoom);   
				AfterImage ("in");                
				mg.StartDelete ();
				float smallZoom = ZoomDetail ("in");
				Camera.main.orthographicSize = baseOrthSize + smallZoom;
				SubCamSet ();
				SetModeling ("in");
				if (OnZoomState != null)
					OnZoomState (tempTileScale);
					
			}
		}
	
	}
	
	protected internal void MobileZoomStep (float orth)
	{
		
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> ();
		mg.StartDelete ();   
		camOrthSize = orth;
		initiCamPos = new Vector3 ((float)gis.GetPixelDelta (mg.sy_Coordinate.center) [0], 1, (float)gis.GetPixelDelta (mg.sy_Coordinate.center) [1]);
		
		OrthSizeSetting ();
		if (camOrthSize > baseOrthSize) {
			int bigZoom = mg.sy_Map.zoom - (Seung (2, camOrthSize / baseOrthSize) + 1);

			if (mg.sy_Map.zoom != bigZoom) {
				ChangeCoordinate ();
				DelAfterImage ("out");   
				GetScale ();    
				mg.sy_Map.zoom -= 1;
				SetModeling ("out");
				AfterImage ("out");

				RefreshMark ();
				MarkCorrections ();
				Camera.main.transform.position = new Vector3 (0, 1, 0);
				GameObject tileGroup = GameObject.Find ("TileGroup");
				for (int i = 0; i < tileGroup.transform.childCount; i++) {
					GameObject.Destroy (tileGroup.transform.GetChild (i).gameObject);
				}
				if (OnChangeZoom != null) {
					OnChangeZoom ();
				}
				if (OnZoomState != null) {
					OnZoomState (tempTileScale);
				}
			}
		} else {
			int bigZoom = mg.sy_Map.zoom + (Seung (2, baseOrthSize / camOrthSize));
	
			if (mg.sy_Map.zoom != bigZoom) {
				ChangeCoordinate ();
				DelAfterImage ("in");   
				GetScale ();    
				mg.sy_Map.zoom += 1;
				SetModeling ("in");
				AfterImage ("in");
				
				RefreshMark ();
				MarkCorrections ();
				Camera.main.transform.position = new Vector3 (0, 1, 0);
			
				GameObject tileGroup = GameObject.Find ("TileGroup");
				for (int i = 0; i < tileGroup.transform.childCount; i++) {
					GameObject.Destroy (tileGroup.transform.GetChild (i).gameObject);
				}
				if (OnChangeZoom != null) {
					OnChangeZoom ();
				}
				if (OnZoomState != null) {
					OnZoomState (tempTileScale);
				}
			}
		}
	}

	protected internal float OrthSize (float orthsize)
	{
		OrthSizeSetting ();
		camOrthSize = orthsize;
		if (camOrthSize > baseOrthSize) {
			float smallZoom = ZoomDetail ("out");
			float result = baseOrthSize - (baseOrthSize - smallZoom) * 0.5f;
			return result;
		} else {
			float smallZoom = ZoomDetail ("in");
			float result = baseOrthSize + smallZoom;
			return result;
		}
	}
	
	protected internal int CatchZoomLevel (float orthsize)
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		OrthSizeSetting ();
		camOrthSize = orthsize;
		if (camOrthSize > baseOrthSize) {
			int bigZoom = mg.sy_Map.zoom - (Seung (2, camOrthSize / baseOrthSize) + 1);
			return bigZoom;
		} else {
			int bigZoom = mg.sy_Map.zoom + (Seung (2, baseOrthSize / camOrthSize));
			return bigZoom;
		}
	}
	
	float ZoomDetail (string mode)
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> ();
		float x = 0;
		float min = baseOrthSize / 64;  // camera orthSize min value : 0.0625
		
		for (int i=0; i<10; i++) {
			if (camOrthSize >= min && camOrthSize < min * 2) {
				if (mode.Equals ("out")) {
					x = min;	
					break;
				}
				if (mode.Equals ("in")) {
					x = min * 2;	
					break;
				}
			}
			min = min * 2;
		}
		float r = ((camOrthSize - x) / x) * baseOrthSize;
		if (mg.sy_OtherOption.ScreenSetting.platform != Variables.OtherOptionGroup.Platform.Mobile) { 
			if (r < -1) {
				r = 0;
			}
		}
		return r;	
	}

	int Seung (int a, float b)
	{
		int c = a;
		int i = 0;
		while (c<=b) {
			c *= a;
			i++;
		}
		return i;
	}

	void ChangeCoordinate ()
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		mg.sy_Map.longitude_x = mg.sy_Coordinate.center [0]; 
		mg.sy_Map.latitude_y = mg.sy_Coordinate.center [1];
	}
	
	void OrthSizeSetting ()
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		ScreenResolution resolution = (ScreenResolution)mg.sy_Map.resolution;
		if (resolution.Equals (ScreenResolution.High)) {
			baseOrthSize = 4;
		} else if (resolution.Equals (ScreenResolution.Low)) {
			baseOrthSize = 2.6f;
		}
		if (mg.sy_OtherOption.ScreenSetting.platform == Variables.OtherOptionGroup.Platform.Mobile && (int)mg.sy_Map.orientation == 1) {
			baseOrthSize = 2.6f;
		}
	}
	
	void AfterImage (string mode)
	{
		string tempName = "";

		GameObject _tilemap = GameObject.Find ("TileGroup");
		float height = _tilemap.transform.position.y;
		
		if (mode.Equals ("out")) {
			height = 0.001f;
			tempName = "StepOne";
		}
		if (mode.Equals ("in")) {
			height = -0.001f;
			tempName = "StepOne";
		}
		Vector3 tempPosition = new Vector3 (-initiCamPos .x / (1 / tempTileScale), height, -initiCamPos.z / (1 / tempTileScale));
		GameObject tile = GameObject.Instantiate (_tilemap, tempPosition, Quaternion.identity)as GameObject;
		tile.transform.localScale = new Vector3 (tempTileScale, tempTileScale, tempTileScale);
		tile.name = tempName;
		for (int i=0; i<tile.transform.childCount; i++) {
			GameObject.Destroy (tile.transform.GetChild (i).GetComponent<ViewMap> ());
			GameObject.Destroy (tile.transform.GetChild (i).GetComponent<BoxCollider> ());
			GameObject.Destroy (tile.transform.GetChild (i).GetComponent<Rigidbody> ());
		}		
	}
	
	void DelAfterImage (string mode)
	{	
		if (GameObject.Find ("StepSix") != null) {
			GameObject.Destroy (GameObject.Find ("StepSix"));
		} 

		int onTile_five = 0;
		if (GameObject.Find ("StepFive") != null) {
			GameObject tempTile = GameObject.Find ("StepFive");

			for (int i=0; i<tempTile.transform.childCount; i++) {	
				MeshRenderer mr = tempTile.transform.GetChild (i).GetComponent<MeshRenderer> ();
				if (mr.enabled.Equals (true)) {
					onTile_five ++; 
				}
			}
			if (onTile_five > limitsTileCount) {
				tempTile.name = "StepSix";
				Test6 (tempTile, mode);
			} else {
				GameObject.Destroy (tempTile);	
			}
		}
		
		int onTile_four = 0;
		if (GameObject.Find ("StepFour") != null) {
			GameObject tempTile = GameObject.Find ("StepFour");

			for (int i=0; i<tempTile.transform.childCount; i++) {	
				MeshRenderer mr = tempTile.transform.GetChild (i).GetComponent<MeshRenderer> ();
				if (mr.enabled.Equals (true)) {
					onTile_four ++; 
				}
			}
			if (onTile_four > limitsTileCount) {
				tempTile.name = "StepFive";
				Test6 (tempTile, mode);
			} else {
				GameObject.Destroy (tempTile);	
			}
		}
		
		int onTile_three = 0;
		if (GameObject.Find ("StepThree") != null) {
			GameObject tempTile = GameObject.Find ("StepThree");
		
			for (int i=0; i<tempTile.transform.childCount; i++) {	
				MeshRenderer mr = tempTile.transform.GetChild (i).GetComponent<MeshRenderer> ();
				if (mr.enabled.Equals (true)) {
					onTile_three ++; 
				}
			}
			if (onTile_three > limitsTileCount) {
				tempTile.name = "StepFour";
				Test6 (tempTile, mode);
			} else {
				GameObject.Destroy (tempTile);	
			}
		}
		
		
		int onTile_two = 0;
		if (GameObject.Find ("StepTwo") != null) {
			GameObject tempTile = GameObject.Find ("StepTwo");
	
			for (int i=0; i<tempTile.transform.childCount; i++) {	
				MeshRenderer mr = tempTile.transform.GetChild (i).GetComponent<MeshRenderer> ();
				if (mr.enabled.Equals (true)) {
					onTile_two ++; 
				}
			}		
			if (onTile_two > limitsTileCount) {
				tempTile.name = "StepThree";
				Test6 (tempTile, mode);
			} else {
				GameObject.Destroy (tempTile);	
			}
		}
		
		int onTile = 0;
		if (GameObject.Find ("StepOne") != null) {
			GameObject tempTile = GameObject.Find ("StepOne");
			for (int i=0; i<tempTile.transform.childCount; i++) {	
				MeshRenderer mr = tempTile.transform.GetChild (i).GetComponent<MeshRenderer> ();
				if (mr.enabled.Equals (true)) {
					onTile ++; 
				}
			}
			if (onTile > limitsTileCount) {
				tempTile.name = "StepTwo";
				Test6 (tempTile, mode);
			} else {
				GameObject.Destroy (tempTile);	
			}
		} 
	}

	public float tempTileScale;
	
	void GetScale ()
	{
		float min = baseOrthSize / 16;
		for (int i=0; i<5; i++) {
		
		
			if (camOrthSize > min && camOrthSize <= min * 2) {
				tempTileScale = 8 / Mathf.Pow (2, i);	
				break;
			} else if (camOrthSize > baseOrthSize && camOrthSize < baseOrthSize * 2) {
				tempTileScale = 0.5f;
				break;
			} else if (camOrthSize >= baseOrthSize * 2 && camOrthSize < baseOrthSize * 4) {
				tempTileScale = 0.25f;
				break;
			} else if (camOrthSize >= baseOrthSize * 4 && camOrthSize < baseOrthSize * 8) {
				tempTileScale = 0.125f;
				break;
			} else if (camOrthSize >= baseOrthSize * 8 && camOrthSize < baseOrthSize * 16) {
				tempTileScale = 0.0625f;
				break;
			}
			min = min * 2;
		}
		//	OnGetScale(tempTileScale);
	}

	void Test6 (GameObject tempTile, string mode)
	{
		
		float testX = 0;
		float testZ = 0;
		bool selectState;
		Vector3 camPos = initiCamPos;
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> ();
		
		if (mg.sy_OtherOption.ScreenSetting.platform != Variables.OtherOptionGroup.Platform.Mobile) { 
			selectState = true;
		} else {
			if (mg.sy_CurrentStatus.is3DCam) {
				selectState = true;
			} else {
				selectState = false;
			}
		}
			
		if (selectState) {
			if (mode.Equals ("out")) {
				tempTile.transform.localScale /= 2;
				testX = tempTile.transform.position.x / 2;
				testZ = tempTile.transform.position.z / 2;
				tempTile.transform.position = new Vector3 (-(camPos .x / 2) + testX, 0.01f, -(camPos .z / 2) + testZ);
			} else if (mode.Equals ("in")) {
				tempTile.transform.localScale *= 2;
				testX = tempTile.transform.position.x * 2;
				testZ = tempTile.transform.position.z * 2;
				tempTile.transform.position = new Vector3 (-(camPos .x * 2) + testX, -0.01f, -(camPos .z * 2) + testZ);
			}
		} else {
			if (mode.Equals ("out")) {
				tempTile.transform.localScale /= (1 / tempTileScale);
				testX = tempTile.transform.position.x / (1 / tempTileScale);
				testZ = tempTile.transform.position.z / (1 / tempTileScale);
				tempTile.transform.position = new Vector3 (-(camPos .x / (1 / tempTileScale)) + testX, 0.01f, -(camPos .z / (1 / tempTileScale)) + testZ);
			} else if (mode.Equals ("in")) {
				tempTile.transform.localScale *= tempTileScale;
				testX = tempTile.transform.position.x * tempTileScale;
				testZ = tempTile.transform.position.z * tempTileScale;
				tempTile.transform.position = new Vector3 (-(camPos .x * tempTileScale) + testX, -0.01f, -(camPos .z * tempTileScale) + testZ);
			}
		}
	}

	GISHelp gHelp = new GISHelp ();

	private void SetModeling (string mode)
	{
		if (GameObject.Find ("Building Assets") != null) {	
			GameObject building = GameObject.Find ("Building Assets");
			
			for (int i=0; i <building.transform.childCount; i++) {
				Transform _building = building.transform.GetChild (i).transform;
		
				int regZoom = _building.GetComponent<BuildingData> ().zoom;
			
				if (regZoom != 0) {
					string cord = _building.GetComponent<BuildingData> ().coordinate;
					string[] cords = cord.Split (',');
					
					double[] target = {double.Parse (cords [0]),double.Parse (cords [1])};
					double[] modelingPos = gHelp.GetPixelDelta (target);
				
					_building.transform.position = new Vector3 ((float)modelingPos [0], _building.transform.position.y, (float)modelingPos [1]);
					
					if (mode.Equals ("out")) {
						_building.transform.localScale /= 2;
					} else if (mode.Equals ("in")) {
						_building.transform.localScale *= 2;
					} 
				}
			}
		}
	}
	
	void Test7 ()
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		
		if (mg.sy_CurrentStatus.is3DCam) {
			CalDistance calDis = new CalDistance ();
		
			for (int k = 0; k < mg.sy_Building.coordinate.Count; k++) {
				string[] str = mg.sy_Building.coordinate [k].Split (',');
				double dis = calDis.distance (mg.sy_Coordinate.center [1], mg.sy_Coordinate.center [0], double.Parse (str [1]), double.Parse (str [0]));
				if (limits () != 0 && Mathf.Abs ((float)dis) < limits ()) {
					mg.ImportModeling (mg.sy_Building.name [k], mg.sy_Coordinate.center);
				} else {
					if (GameObject.Find ("Building Assets") != null) {
						GameObject test = GameObject.Find ("Building Assets");

						for (int j = 0; j < test.transform.childCount; j++) {
							if (test.transform.GetChild (j).GetComponent<BuildingData> ().fileName == mg.sy_Building.name [k]) {
								mg.sy_Building.activated_building_name.Remove (test.transform.GetChild (j).GetComponent<BuildingData> ().fileName);
								test.transform.GetChild (j).GetComponent<BuildingData> ().bundle.Unload (true);
								GameObject.DestroyImmediate (test.transform.GetChild (j).gameObject);
								break;
							}
						}
					}
				}
			}
		}
	}
	
	int limits ()
	{
		Manager mg = (Manager)GameObject.Find ("Manager").GetComponent<Manager> ();
		int zoom = mg.sy_Map.zoom;
		int zoomMax = 20;
		for (int i = 0; i < 8; i++) {
			if (zoom == zoomMax - i) {
				return (int)mg.sy_OtherOption.Camera.distance [i];
			}
		}
		return 0;
	}

}
