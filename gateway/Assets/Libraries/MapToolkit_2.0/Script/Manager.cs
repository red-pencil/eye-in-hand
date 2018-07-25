// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com

using UnityEngine;
using System.Collections;
using System.Collections.Generic ;

public class Manager : MonoBehaviour
{	

	public Variables.MarkGroup sy_Mark;
	public Variables.BuildingGroup sy_Building;
	public Variables.MapGroup sy_Map;
	public Variables.EditorGroup sy_Editor;
	public Variables.OtherOptionGroup sy_OtherOption;
	public Variables.ConfigGroup sy_Config;
	public Variables.CoordinateGroup sy_Coordinate;
	public Variables.CurrentStatusGroup sy_CurrentStatus;
	public Variables.CameraGroup sy_Camera;
	public string[] sy_GeoData;
	protected internal CamContorl cc = new CamContorl ();
	protected internal Zoom zm = new Zoom ();
	protected internal GetBuilding_URL gbuilding = new GetBuilding_URL ();
	protected internal VrCamera vc = new VrCamera ();
	protected internal VR_MobileCameara vrMobileCam = new VR_MobileCameara ();
	private CreateTile ct = new CreateTile ();
	private Calculation cal = new Calculation ();
	private Mark mark = new Mark ();
	private GeoCoding geo = new GeoCoding ();
	private MobileCamera mobileCam = new MobileCamera ();
	private GetXML_URL gxml = new GetXML_URL ();
	private GISHelp gHelp = new GISHelp ();
	private AutoComplete ac = new AutoComplete ();
	private ClipboardHelper clip = new ClipboardHelper ();
	private Camera subcam;
	private Camera vrcam;
	float baseOrthSize;

	void Awake ()
	{
		//	sy_Map.minLevel = 1;
		//	sy_Map.maxLevel = 20;

		EditorValueSetting ();    //1  플랫폼 설정 때문에  먼저읽어드린다. 
		DefinePlatform ();          //2 
		initiSetting ();
		if (sy_OtherOption.ScreenSetting.platform == Variables.OtherOptionGroup.Platform.WebPlayer)
			GetURL ();
	}
	
	void DefinePlatform ()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			sy_OtherOption.ScreenSetting.platform = Variables.OtherOptionGroup.Platform.Mobile;
		} else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer) {
			sy_OtherOption.ScreenSetting.platform = Variables.OtherOptionGroup.Platform.Standalone;
		} /*else if (Application.isWebPlayer) {
			sy_OtherOption.ScreenSetting.platform = Variables.OtherOptionGroup.Platform.WebPlayer;
		}*/
		//sy_OtherOption.ScreenSetting.platform = Variables.OtherOptionGroup.Platform.Mobile;
	}

	void GetURL ()
	{
		if (sy_Map.activeMarkBuilding) {
			string currentUrl = Application.absoluteURL;
			string[] s = currentUrl.Split ('/');
			string _s = s [s.Length - 1];
			_s = currentUrl.Substring (0, currentUrl.Length - _s.Length);
			sy_Config.phpurl = _s + "StreamingAssets/";	
		}
	}
	
	void EditorValueSetting ()
	{
		sy_Map.longitude_x = double.Parse (sy_Editor.longitude_x);
		sy_Map.latitude_y = double.Parse (sy_Editor.latitude_y);
		sy_Map.zoom = sy_Editor.zoom;

		ChangeLicense (sy_Editor.license);
		ChangeScreenResolution (sy_Editor.resolution);
		ChangeQuality (sy_Editor.quality);
		ChangeType (sy_Editor.maptype);
		ChangeMarkAccessMethod (sy_Editor.Access.mark);
		ChangeBuildingAccessMethod (sy_Editor.Access.building);
		ChangeAssetBundleDataSave (sy_Editor.ads);
		ChangePlatformDefines (sy_Editor.platform);
		ChangeOrientation (sy_Editor.ori);
	}
	
	void initiSetting ()
	{
		
		if (GameObject.Find ("> Building Layout") != null) {
			if (GameObject.Find ("> Building Layout").transform.childCount == 0)
				Destroy (GameObject.Find ("> Building Layout"));
		}

		GameObject camGroup = new GameObject ("Camera Group");
		sy_Camera.mainCam = Camera.main;
		Camera.main.transform.parent = camGroup.transform;
		Camera.main.transform.position = new Vector3 (0, 1, 0);
		GameObject subCamera = new GameObject ("SubCamera");
		subCamera.transform.parent = camGroup.transform;
		subcam = (Camera)subCamera.AddComponent (typeof(Camera));
		subcam.orthographic = true;
		subcam.enabled = false;
		
		GameObject vrCamera = new GameObject ("3D Camera");
		vrCamera.transform.parent = camGroup.transform;
		vrcam = (Camera)vrCamera.AddComponent (typeof(Camera));
		vrcam.transform.rotation = Quaternion.Euler (90, 0, 0);
		vrcam.orthographic = false;
		vrcam.nearClipPlane = 0.3f;
		vrcam.enabled = false;
		sy_Camera.perspectiveCam = vrcam; 
		
		GameObject vrTarget = new GameObject ("3D Target");
		vrTarget.transform.parent = Camera.main.transform;
		Camera.main.gameObject.AddComponent<MarkMove> ();

		sy_Camera.vrCam = vc;
		sy_Camera.vrMobileCam = vrMobileCam;
	}

	IEnumerator initiWeb ()
	{
		yield return StartCoroutine (gxml.Run (sy_Config.phpurl));
		sy_Mark.table_name = gxml.markTable;
		sy_Mark.field_name = gxml.fieldGroup.ToArray ();
		sy_Mark.field_contents = gxml.fieldContents;
		sy_Mark.coordinate = gxml.markCoordinate;
		
		if (sy_Mark.table_name != null) {
			if (sy_Mark.table_name.Length <= 0 || sy_Mark.table_name == null || sy_Mark.coordinate.Length <= 0 || sy_Mark.coordinate == null) {
				sy_CurrentStatus.isReady = false;
			} else {
				sy_CurrentStatus.isReady = true;
			}
		} else {
			sy_CurrentStatus.isReady = true;
		}
	}

	void initiLocal ()
	{
		GetXML gxml = new GetXML ();
		gxml.Run ();
		sy_Mark.table_name = gxml.tableGroup.ToArray ();
		sy_Mark.field_name = gxml.fieldGroup.ToArray ();
		sy_Mark.field_contents = gxml.fieldContents;
		sy_Mark.coordinate = gxml.markCoordinate;
		sy_CurrentStatus.isReady = true;
	}
	
	IEnumerator buildingWeb (string url)
	{
		yield return StartCoroutine (gbuilding.Run (url));
		sy_Building.name = gbuilding.fileNameList;
		sy_Building.coordinate = gbuilding.buildingCoordinate;
		if (sy_OtherOption.ScreenSetting.platform != Variables.OtherOptionGroup.Platform.Mobile &&
			sy_OtherOption.ScreenSetting.platform != Variables.OtherOptionGroup.Platform.WebPlayer) {
			sy_Building.binary_filename = gbuilding.LocalFolderCheck ();
		}
	}
	
	void buildingLocal ()
	{
		gbuilding.LocalRun ();
		sy_Building.name = gbuilding.fileNameList;
		sy_Building.coordinate = gbuilding.buildingCoordinate;
		sy_Building.binary_filename = gbuilding.LocalFolderCheck ();
	}

	IEnumerator Start ()
	{		
		if (sy_OtherOption.ScreenSetting.platform != Variables.OtherOptionGroup.Platform.Mobile) {
			if (sy_Map.activeMarkBuilding) {
				if (sy_OtherOption.ScreenSetting.platform != Variables.OtherOptionGroup.Platform.WebPlayer) {

					if (sy_Config.Access.building == Variables.ConfigGroup.BuildingAccess.Local) {
						buildingLocal ();
					} else if (sy_Config.Access.building == Variables.ConfigGroup.BuildingAccess.Web) {
						yield return StartCoroutine (buildingWeb (sy_Config.phpurl));
					}
					if (sy_Config.Access.mark == Variables.ConfigGroup.MarkAccess.Local) {
						initiLocal ();
					} else if (sy_Config.Access.mark == Variables.ConfigGroup.MarkAccess.Web) {
						yield return StartCoroutine (initiWeb ());
					}
				} else {
					if (!Application.isEditor) {
						yield return StartCoroutine (buildingWeb (sy_Config.phpurl));
						yield return StartCoroutine (initiWeb ());
					} else {
						//	Debug.Log ("WebPlayer: 에디터 모드에서 마크와 모델링을 가져올수 없습니다.");
						sy_CurrentStatus.isReady = true;	
					}
				}
			} else {
				sy_CurrentStatus.isReady = true;	
			}
		
			if (sy_CurrentStatus.isReady && (sy_Map.apikey [0].Length != 0 || sy_Map.apikey [1].Length != 0 && sy_Map.apikey [2].Length != 0)) {
				mark.MarkSet ();
				OrthSizeSetting ();
				ct.InitiCreateTile ();
			}
		} else {   // -----------------------------------------------------------------------------------------------------------------------------------Mobile
			if (sy_Map.activeMarkBuilding) {
				yield return StartCoroutine (buildingWeb (sy_Config.phpurl));
				yield return StartCoroutine (initiWeb ());
			} else {
				sy_CurrentStatus.isReady = true;		
			}
			if (sy_CurrentStatus.isReady && (sy_Map.apikey [0].Length != 0 || sy_Map.apikey [1].Length != 0 && sy_Map.apikey [2].Length != 0)) {
				mark.MarkSet ();
				OrthSizeSetting ();
				ct.InitiCreateTile ();
			}
		}
	}
	
	void LateUpdate ()
	{
		if (sy_OtherOption.ScreenSetting.platform != Variables.OtherOptionGroup.Platform.Mobile) {
			if (sy_CurrentStatus.isReady && (sy_Map.apikey [0].Length != 0 || sy_Map.apikey [1].Length != 0 && sy_Map.apikey [2].Length != 0)) {
	
				if (!sy_CurrentStatus.is3DCam) {
					cc.ScrollWheel ();
				}
				cc.ScreenPan ();
				cc.Cam3DMode ();	
				CurCameraCoordinate ();    //IMPORTANT : Process-1
				MouseCoordinate ();
	

				ct.AfterCreateTile (sy_CurrentStatus.is3DCam);
				if (!sy_CurrentStatus.is3DCam) {
					zm.ZoomStep ();
				}
				GCCollection ();
			}
		} else {
			if (sy_CurrentStatus.isReady && (sy_Map.apikey [0].Length != 0 || sy_Map.apikey [1].Length != 0 && sy_Map.apikey [2].Length != 0)) {
				CurCameraCoordinate (); 
				if (!sy_Camera.camPause) {
					mobileCam.Run ();
				}
				ct.AfterCreateTile (sy_CurrentStatus.is3DCam);
				GCCollection ();
			}
		}
	}

	void GCCollection ()
	{
		if (Input.GetMouseButtonUp (0)) {
			System.GC.Collect ();
			Resources.UnloadUnusedAssets ();
		}
	}
	
	void OnGUI ()
	{
		if (GUI.GetNameOfFocusedControl () == null)
			GUI.FocusControl ("");
	}
	
	public  void StartDelete ()
	{
		StartCoroutine (zm.DelAfterImage ());
	}
	
	protected internal void AddMap (Transform target, Vector2 camTilePos)
	{
		StartCoroutine (ct.AddMap (target, camTilePos));
	}
	
	void OrthSizeSetting ()
	{
		if (sy_Map.resolution.Equals (Variables.MapGroup.ScreenResolution.High)) {
			Camera.main.orthographicSize = 4;
			subcam.orthographicSize = 4;
			baseOrthSize = 4;
		} else if (sy_Map.resolution.Equals (Variables.MapGroup.ScreenResolution.Low)) {
			Camera.main.orthographicSize = 2.6f;
			subcam.orthographicSize = 2.6f;
			baseOrthSize = 2.6f;
		}
		if (sy_OtherOption.ScreenSetting.platform == Variables.OtherOptionGroup.Platform.Mobile && (int)sy_Map.orientation == 1) {
			Camera.main.orthographicSize = 2.6f;
			baseOrthSize = 2.6f;
		}
	}

	void CurCameraCoordinate ()
	{
		Vector3 pos = Camera.main.transform.position;
		sy_Coordinate.center = cal.CurrentCoordinate (pos);
	}
	
	void MouseCoordinate ()
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		sy_Coordinate.mouse = cal.CurrentCoordinate (pos);
	}

	public void SetMarkTarget (Transform target)
	{
		Camera.main.GetComponent<MarkMove> ().target = target;
	}
	
	public void MarkRefresh ()
	{
		sy_CurrentStatus.isMarkPoint = false;
		Camera.main.GetComponent<MarkMove> ().target = null;
		Destroy (GameObject.Find ("Mark"));
		mark.MarkSet ();	
	}

	public Transform GetMarkTarget ()
	{
		return Camera.main.GetComponent<MarkMove> ().target;
	}

	public void RunMarkMove ()
	{
		Camera.main.GetComponent<MarkMove> ().RunMove ();	
	}

	public void ImportModeling (string fileName, double[] target)
	{
		StartCoroutine (gbuilding.Call (sy_Config.phpurl, fileName, target));
	}

	public void Vr_Mobile_ChangeTheMap ()
	{
		StartCoroutine ("RunChangeTheMap");
		StartDelete ();
	}

	IEnumerator RunChangeTheMap ()
	{
		int curZoom = sy_Map.zoom;
		yield return new WaitForSeconds(0.4f);
		if (curZoom == sy_Map.zoom) {
			zm.Vr_ChangeZoom (curZoom);
		}
	}
	
	public string[]  AutoComplete (string keyword, string idleText, string[][] field_name)
	{
		return ac.Run (keyword, idleText, field_name);
	}
	
	public double GetDistanceP1toP2 (double P1_longitude_x, double P1_latitude_y, double P2_longitude_x, double P2_latitude_y)
	{
		CalDistance calDis = new CalDistance ();
		return calDis.distance (P1_latitude_y, P1_longitude_x, P2_latitude_y, P2_longitude_x);
	}

	public float GetBearingP1toP2 (double P1_longitude_x, double P1_latitude_y, double P2_longitude_x, double P2_latitude_y)
	{
		CalDistance calDis = new CalDistance ();
		return calDis.bearingP1toP2 (P1_latitude_y, P1_longitude_x, P2_latitude_y, P2_longitude_x);
	}
	
	public void LimitDistance ()
	{
		gbuilding.CalDistance (true);	
	}
	
	public void ChangeZoom ()
	{
		zm.ChangeZoom (sy_Map.zoom);
	}

	public void ClipboardCopy (string text)
	{
		clip.Copy (text);
	}

	public string ClipboardPaste ()
	{
		return clip.Paste;
	}

	public void GeneralMode ()
	{
		cc.GeneralMode ();
	}
	
	public void VrMode ()
	{
		cc.VrMode ();	
	}

	public int Progress (bool type, int percent)
	{
		if (!sy_CurrentStatus.isReady) {
			if (gbuilding.www_building != null && type.Equals (false)) {
				return  (int)gbuilding.www_building.progress * percent;
			} else if (gxml.www_mark != null && type.Equals (true)) {
				return  (int)gxml.www_mark.progress * percent;
			} else {
				return 0;
			}
		} else {
			return percent;	
		}
	}

	public void RefreshMap ()
	{
		zm.RefreshMap ();	
	}

	public void ZoomStep ()
	{
		zm.ZoomStep ();
	}

	public IEnumerator GetGeoData (string keyword, string url)
	{
		yield return StartCoroutine (geo.GetData (keyword,url));	
		sy_GeoData = geo.geoData;
	}

	public Vector3 GIStoPos (double[] coordinate)
	{
		double[] co_pos = gHelp.GetPixelDelta (coordinate);
		Vector3 pos = new Vector3 ((float)co_pos [0], 0, (float)co_pos [1]); 
		return pos;
	}
	
	public double[] PostoGIS (Vector3 pos)
	{
		double[] coordinate = cal.CurrentCoordinate (pos);
		return coordinate;
	}

	private int autozoom;
	private string autostate;
	private float autoratio;
	private int initiResolution;
	private int initiQuality;

	public void AutoZoom (int zoom, float delay, float zoomRatio)
	{
		if (sy_Map.zoom - zoom > 0)
			autostate = "out";
		else
			autostate = "in";
		
		autozoom = zoom;
		autoratio = zoomRatio;
		initiResolution = (int)sy_Map.resolution;
		initiQuality = (int)sy_Map.quality;

		CancelInvoke ();
		InvokeRepeating ("ZoomInOut", delay, 0.03f);
	}

	public void StopAutoZoom ()
	{
		CancelInvoke ();

	}

	void ZoomInOut ()
	{
	
		if (sy_CurrentStatus.is3DCam) {

			float orthsizeAfter = 0;
			float desiredDistance = vc.desiredDistance;
			float orthsizeOrigin = ((desiredDistance - 2.253f) / 0.173f) * 0.1f + 1.3f;
			if (autostate == "out" && orthsizeOrigin >= baseOrthSize) {
				Camera.main.orthographicSize = orthsizeOrigin;
				zm.ZoomStep ();
				orthsizeAfter = zm.OrthSize (orthsizeOrigin);
				vc.desiredDistance = ((((orthsizeAfter - 1.3f) / 0.1f) * 0.173f) + 2.253f);
			} else if (autostate == "in" && orthsizeOrigin <= baseOrthSize * 0.5f) {
				Camera.main.orthographicSize = orthsizeOrigin;
				zm.ZoomStep ();
				orthsizeAfter = zm.OrthSize (orthsizeOrigin);
				vc.desiredDistance = ((((orthsizeAfter - 1.3f) / 0.1f) * 0.173f) + 2.253f);

			}
			if (autostate == "in") {
				vc.desiredDistance -= 0.01f * autoratio;
			} else if (autostate == "out") {
				vc.desiredDistance += 0.01f * autoratio;
			}
		} else {
			if (autostate == "in") {
				sy_Camera.mainCam.orthographicSize -= 0.01f * autoratio;
			//	sy_Map.resolution = Variables.MapGroup.ScreenResolution.Low;
				sy_Map.quality = Variables.MapGroup.MapQuality.Low;
				if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
					if (sy_Camera.mainCam.orthographicSize <= baseOrthSize * 0.5f) {
						zm.ZoomStep ();
					}
				}
			} else if (autostate == "out") {
				sy_Camera.mainCam.orthographicSize += 0.01f * autoratio;
			//	sy_Map.resolution = Variables.MapGroup.ScreenResolution.Low;
				sy_Map.quality = Variables.MapGroup.MapQuality.Low;
				if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
					if (sy_Camera.mainCam.orthographicSize >= baseOrthSize) {
						zm.ZoomStep ();
					}
				}
			}
		}

		if (sy_Map.zoom == autozoom) {
			ChangeScreenResolution (initiResolution);
			ChangeQuality (initiQuality);
			RefreshMap ();
			CancelInvoke ();
		}
	}
	
	//////////////////////////////////////////////////////////
	/// Changes the option.
	//////////////////////////////////////////////////////////
	private string ChangeLicense (int num)
	{
		sy_Editor.license = num;
		string tex = System.Enum.GetName (typeof(Variables.MapGroup.License), num);
		sy_Map.license = (Variables.MapGroup.License)System.Enum.Parse (typeof(Variables.MapGroup.License), tex);
		return tex;
	}
	
	public string ChangeScreenResolution (int num)
	{
		sy_Editor.resolution = num;
		string tex = System.Enum.GetName (typeof(Variables.MapGroup.ScreenResolution), num);
		sy_Map.resolution = (Variables.MapGroup.ScreenResolution)System.Enum.Parse (typeof(Variables.MapGroup.ScreenResolution), tex);
		return tex;
	}

	public string ChangeQuality (int num)
	{
		sy_Editor.quality = num;
		string tex = System.Enum.GetName (typeof(Variables.MapGroup.MapQuality), num);
		sy_Map.quality = (Variables.MapGroup.MapQuality)System.Enum.Parse (typeof(Variables.MapGroup.MapQuality), tex);
		return tex;
	}

	public string ChangeType (int num)
	{
		sy_Editor.maptype = num;
		string tex = System.Enum.GetName (typeof(Variables.MapGroup.MapType), num);
		sy_Map.maptype = (Variables.MapGroup.MapType)System.Enum.Parse (typeof(Variables.MapGroup.MapType), tex);
		return tex;
	}

	private string ChangeMarkAccessMethod (int num)
	{
		sy_Editor.Access.mark = num;
		string tex = System.Enum.GetName (typeof(Variables.ConfigGroup.MarkAccess), num);
		sy_Config.Access.mark = (Variables.ConfigGroup.MarkAccess)System.Enum.Parse (typeof(Variables.ConfigGroup.MarkAccess), tex);
		return tex;
	}
	
	private string ChangeBuildingAccessMethod (int num)
	{
		sy_Editor.Access.building = num;
		string tex = System.Enum.GetName (typeof(Variables.ConfigGroup.BuildingAccess), num);
		sy_Config.Access.building = (Variables.ConfigGroup.BuildingAccess)System.Enum.Parse (typeof(Variables.ConfigGroup.BuildingAccess), tex);
		return tex;
	}
	
	private string ChangeAssetBundleDataSave (int num)
	{
		sy_Editor.ads = num;
		string tex = System.Enum.GetName (typeof(Variables.OtherOptionGroup.AssetDataSave), num);
		sy_OtherOption.AssetBundle.ads = (Variables.OtherOptionGroup.AssetDataSave)System.Enum.Parse (typeof(Variables.OtherOptionGroup.AssetDataSave), tex);
		return tex;
	}
	
	private string ChangePlatformDefines (int num)
	{
		sy_Editor.platform = num;
		string tex = System.Enum.GetName (typeof(Variables.OtherOptionGroup.Platform), num);
		sy_OtherOption.ScreenSetting.platform = (Variables.OtherOptionGroup.Platform)System.Enum.Parse (typeof(Variables.OtherOptionGroup.Platform), tex);
		return tex;
	}
	
	private string ChangeOrientation (int num)
	{
		sy_Editor.ori = num;
		string tex = System.Enum.GetName (typeof(Variables.MapGroup.OrientationGroup), num);
		sy_Map.orientation = (Variables.MapGroup.OrientationGroup)System.Enum.Parse (typeof(Variables.MapGroup.OrientationGroup), tex);
		return tex;
	}

}
