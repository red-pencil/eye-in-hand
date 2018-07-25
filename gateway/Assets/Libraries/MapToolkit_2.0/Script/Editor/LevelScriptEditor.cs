#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Manager))]
public class LevelScriptEditor : Editor
{
	
		public enum License
	{
		GoogleMapsAPI,
		GoogleMapsAPI_forBusiness
	};
	
	public enum ScreenResolution
	{
		High,
		Low
	};

	public enum MapQuality
	{
		High,
		Low
	};
	
	public enum MapType
	{
		Roadmap,
		Satellite,
		Hybrid,
		Terrain
	};
	
	public enum MarkAccess
	{
		Local,
		Web
	};

	public enum BuildingAccess
	{
		Local,
		Web
	};
	
	public enum Orientation
	{
		Portrait,
		Landscape
	};
	
	
	Manager mg;
	ScreenResolution resolution;
	License license;
	MapQuality quality;
	MapType maptype;
	float  wheelSpeed;
	MarkAccess markAccess;
	BuildingAccess buildingAccess;
	bool _isCompile;
		private Orientation ori;
	
	public override void OnInspectorGUI ()
	{

		mg = (Manager)target;
			
		if (GUI.GetNameOfFocusedControl () == null)
			GUI.FocusControl ("");
		
		if(mg.sy_Map != null &&mg.sy_Building != null && mg.sy_Config != null && mg.sy_Coordinate != null && mg.sy_CurrentStatus != null &&
			mg.sy_Editor != null && mg.sy_Map != null && mg.sy_Mark != null && mg.sy_OtherOption != null){
			
			//PlatformSetting();
			
		GUILayout.Space (6f);
		GUI.backgroundColor = Color.cyan;
		GUIStyle myButtonStyle = new GUIStyle (GUI.skin.box);
		myButtonStyle.normal.textColor = Color.white;
		GUILayout.Box ("Google Maps API licensing", myButtonStyle, GUILayout.ExpandWidth (true));
		GUI.backgroundColor = Color.white;	
		GUILayout.Space (6f);
		
			//License

	    string str_license = System.Enum.GetName (typeof(License), mg.sy_Editor.license);
		license = (License)System.Enum.Parse (typeof(License), str_license);
		mg.sy_Editor.license = (int)(License)EditorGUILayout.EnumPopup ("License", license);

		if(license == License.GoogleMapsAPI){
			mg.sy_Map.apikey[0] =EditorGUILayout.TextField (" ▶API Key",mg.sy_Map.apikey[0]);
			mg.sy_Map.apikey[1]="";
			mg.sy_Map.apikey[2]="";
		}
		else if(license == License.GoogleMapsAPI_forBusiness){
			mg.sy_Map.apikey[0]="";
			mg.sy_Map.apikey[1] =EditorGUILayout.TextField (" ▶Client ID",mg.sy_Map.apikey[1]);
			mg.sy_Map.apikey[2]=EditorGUILayout.TextField (" ▶Signature", mg.sy_Map.apikey[2]);
		}

		if(mg.sy_Map.apikey[0].Length<5 &&( mg.sy_Map.apikey[1].Length<5 ||mg.sy_Map.apikey[2].Length<5)){
			GUI.enabled = false;
		}
		GUILayout.Space (6f);
		GUI.backgroundColor = Color.cyan;
		myButtonStyle.normal.textColor = Color.white;
		GUILayout.Box ("Coordinates Setting", myButtonStyle, GUILayout.ExpandWidth (true));
		GUI.backgroundColor = Color.white;	
		GUILayout.Space (6f);
		
		if(mg.sy_Editor !=null){
		mg.sy_Editor.longitude_x = EditorGUILayout.TextField ("longitudeX", mg.sy_Editor.longitude_x);
		mg.sy_Editor.latitude_y = EditorGUILayout.TextField ("latitudeY", mg.sy_Editor.latitude_y);
		mg.sy_Editor.zoom = EditorGUILayout.IntField ("Zoom", mg.sy_Editor.zoom);
		}
		

		BeginContents ();

		 mg.sy_Map.language = EditorGUILayout.TextField ("Language", mg.sy_Map.language);
		
		//ScreenResolution

		string str_resolution = System.Enum.GetName (typeof(ScreenResolution), mg.sy_Editor.resolution);
		resolution = (ScreenResolution)System.Enum.Parse (typeof(ScreenResolution), str_resolution);
		mg.sy_Editor.resolution = (int)(ScreenResolution)EditorGUILayout.EnumPopup ("Resolution", resolution);
		
		//MapQuality
		string str_quality = System.Enum.GetName (typeof(MapQuality), mg.sy_Editor.quality);
		quality = (MapQuality)System.Enum.Parse (typeof(MapQuality), str_quality);
		mg.sy_Editor.quality = (int)(MapQuality)EditorGUILayout.EnumPopup ("Quality", quality);
		
		//MapType
		string str_maptype = System.Enum.GetName (typeof(MapType), mg.sy_Editor.maptype);
		maptype = (MapType)System.Enum.Parse (typeof(MapType), str_maptype);
		mg.sy_Editor.maptype = (int)(MapType)EditorGUILayout.EnumPopup ("Map type", maptype);
		
		//WheelSpeed
		mg.sy_Map.wheelSpeed=EditorGUILayout.Slider("Wheel Speed",mg.sy_Map.wheelSpeed,2,10);

		EndContents ();
	#if UNITY_ANDROID || UNITY_IPHONE		
				string str_ori = System.Enum.GetName (typeof(Orientation), mg.sy_Editor.ori);
		ori = (Orientation)System.Enum.Parse (typeof(Orientation), str_ori);
		mg.sy_Editor.ori = (int)(Orientation)EditorGUILayout.EnumPopup ("Default Orientation", ori);
#endif 
			
		mg.sy_Map.activeMarkBuilding = EditorGUILayout.Toggle ("Active Mark/Building", mg.sy_Map.activeMarkBuilding);

#if 	!UNITY_WEBPLAYER
		if(mg.sy_Map.activeMarkBuilding){
		GUILayout.Space (6f);
		GUI.backgroundColor = Color.cyan;
		myButtonStyle.normal.textColor = Color.white;
		GUILayout.Box ("Configuration", myButtonStyle, GUILayout.ExpandWidth (true));
		GUI.backgroundColor = Color.white;	
		GUILayout.Space (6f);
		mg.sy_Config.phpurl = EditorGUILayout.TextField ("URL", mg.sy_Config.phpurl);
#if !(UNITY_ANDROID || UNITY_IPHONE)
		MarkConfig ();
		BuildingConfig ();
#else
		mg.sy_Editor.Access.building =1;
		mg.sy_Editor.Access.mark =1;			
#endif
		}
		CheckPHPfile("pc");
#else	
		EditorSettings.webSecurityEmulationHostUrl = "http://maps.googleapis.com/maps/api";
	   CheckPHPfile("web");
#endif
		GUILayout.Space (6f);
		Utility ();
		}	
	}
		     void PlatformSetting()
    {
		
		     Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 		

			if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
			 mg.sy_Editor.platform = 0;
	        else	if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXIntel)
			 mg.sy_Editor.platform = 1;
		     else	if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
			mg.sy_Editor.platform = 2;
		    else	if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
			mg.sy_Editor.platform = 3;

    }
	
	void CheckPHPfile(string state){

		FileInfo _info = new FileInfo(Application.streamingAssetsPath+"/connect.php");
		
		if(state.Equals("web")){
	  if(!_info.Exists){
		CopyFolder("Assets/MapToolkit_2.0/Temp/PHP",Application.streamingAssetsPath,"*.php");
		AssetDatabase.Refresh();		
		}}
		else if(state.Equals("pc")){
			  if(_info.Exists){
		_info.Delete();
		AssetDatabase.Refresh();		
		}
		}	
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// MarkConfig
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void MarkConfig ()
	{
		mg = (Manager)target;
		GUILayout.Label ("Access Method", GUILayout.ExpandWidth (true));
		
		//MarkAccess
		string str_markAccess = System.Enum.GetName (typeof(MarkAccess), mg.sy_Editor.Access.mark );
		markAccess = (MarkAccess)System.Enum.Parse (typeof(MarkAccess), str_markAccess);
		mg.sy_Editor.Access.mark  = (int)(MarkAccess)EditorGUILayout.EnumPopup (" ▶Mark", markAccess);

		
	}
	

		public void CopyFolder (string sourceFolder, string destFolder, string extention)
	{
		if (!Directory.Exists (destFolder)) 
			Directory.CreateDirectory (destFolder);

		string[] files = Directory.GetFiles (sourceFolder,extention);
		foreach (string file in files) {
			string name = Path.GetFileName (file);
			string dest = Path.Combine (destFolder, name);
			File.Copy (file, dest, true);
		}
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// BuildingConfig
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	void BuildingConfig ()
	{
		string str_buildingAccess = System.Enum.GetName (typeof(BuildingAccess), mg.sy_Editor.Access.building);
		buildingAccess = (BuildingAccess)System.Enum.Parse (typeof(BuildingAccess), str_buildingAccess);
		mg.sy_Editor.Access.building = (int)(BuildingAccess)EditorGUILayout.EnumPopup (" ▶Building ", buildingAccess);
		GUILayout.Space (6f);
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Utility
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	void Utility ()
	{
		GUI.backgroundColor = Color.cyan;
		GUIStyle myButtonStyle = new GUIStyle (GUI.skin.box);
		myButtonStyle.normal.textColor = Color.white;
		GUILayout.Box ("Utility", myButtonStyle, GUILayout.ExpandWidth (true));
		GUI.backgroundColor = Color.white;	
		GUILayout.Space (2f);
		
		if (GUILayout.Button ("Create Mark")) {
			if (GameObject.Find ("Manager").GetComponent<Xml_Manager> () == null) {
				GameObject.Find ("Manager").AddComponent <Xml_Manager>();
			}
			EditorWindow.GetWindow<XML_Window> (false, "Mark Maker", true);
			Xml_Manager manager = GameObject.Find ("Manager").GetComponent<Xml_Manager> ();
			manager.ListClear ();
		}
		
		GUILayout.Space (2f);
		if (GUILayout.Button ("Building Layout Tools")) {
			EditorWindow.GetWindow<BuildingLayout> (false, "Building Layout", true);
		}
			GUILayout.Space (2f);
		if (GUILayout.Button ("Other Option")) {
			EditorWindow.GetWindow<OtherOption> (false, "Other Option", true);
		}
		GUILayout.Space (2f);
			if (GUILayout.Button ("Delete Temporary Files")) {
			DeleteFiles("Assets/MapToolkit_2.0/Temp/BuildingData");
			DeleteFiles("Assets/MapToolkit_2.0/Temp/MarkData");
			AssetDatabase.Refresh();
		}
		
	}
	
	void DeleteFiles(string Path){
	
DirectoryInfo dir = new DirectoryInfo(Path);
		
System.IO.FileInfo[] files = dir.GetFiles("*.*",SearchOption.AllDirectories);
foreach (System.IO.FileInfo file in files){
			//if(file.Extension !=".csy")
			file.Delete();
		}

	}

	private int GetFileCount (string szPath)
	{
		if (Directory.Exists (szPath)) {
			string [] files = Directory.GetFiles (szPath);
			return files.Length;
		}
		return 0;
	}
	
	static public void BeginContents ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Space (5f);
		EditorGUILayout.BeginHorizontal ("AS TextArea", GUILayout.MinHeight (10f));
		GUILayout.BeginVertical ();
		GUILayout.Space (5f);
	}

	static public void EndContents ()
	{
		GUILayout.Space (5f);
		GUILayout.EndVertical ();
		EditorGUILayout.EndHorizontal ();
		GUILayout.Space (5f);
		GUILayout.EndHorizontal ();
		GUILayout.Space (5f);
	}
}
#endif