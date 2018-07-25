// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com

using System.Collections.Generic ;
using UnityEngine;

public class Variables
{

	[System.Serializable]
public class MarkGroup                                                            //-----------------------------------------------------------------Mark
	{
		public string[] table_name;
		public string[][] field_name;
		public string[][][] field_contents;
		public string[][] coordinate;
	}
	
	[System.Serializable]
public class BuildingGroup                                                            //-----------------------------------------------------------------Building
	{
		public List<string> name;
		public List<string> coordinate;
		public List<string> binary_filename = new List<string> ();
		public List<string> activated_building_name = new List<string> ();
	}
	
	[System.Serializable]
public class MapGroup                                                               //-----------------------------------------------------------------Map
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

		[System.Serializable]
		public enum OrientationGroup
		{
			Portrait,
			Landscape
		};
	
		public License license;
		public string addParameter;
		public string[] apikey = new string[3]{"","",""};
		public double longitude_x;
		public double latitude_y;
		public int zoom;
		public ScreenResolution resolution;
		public MapQuality quality;
		public MapType maptype;
		public float wheelSpeed = 8;
		public string language = "en";
		public int minLevel = 1;
		public int maxLevel = 20;
		public OrientationGroup orientation;
		public bool activeMarkBuilding;

		
	}
	
	[System.Serializable]
public class EditorGroup                                                           //-----------------------------------------------------------------Editor
	{
		public string longitude_x = "-122.403";
		public string latitude_y = "37.8";
		public int zoom = 17;
		public int license;
		public int resolution;
		public int quality;
		public int maptype;
		public AccessGroup Access;
		public int alm = 1;  // assetbundle Loading Method
		public int ads = 1; //aseestbundle Data storage
		public int ori = 0; //Orientation
		public int platform = 1;
		[System.Serializable]
		public class AccessGroup
		{
			public int mark;
			public int building;
		}
	}

	[System.Serializable]
	public class CameraGroup                                                          //-----------------------------------------------------------------Editor
	{
		public bool camPause = false;
		public Camera perspectiveCam;
		public Camera mainCam;
		public VrCamera vrCam;
		public VR_MobileCameara vrMobileCam;
	}
	
	[System.Serializable]
	public class OtherOptionGroup                                                          //-----------------------------------------------------------------OtherOption
	{
		[System.Serializable]	
		public enum Platform
		{
			WebPlayer,
			Standalone,
			Mobile
		};
		
		[System.Serializable]
		public class ScreenGroup
		{
			public Platform platform;
		}
		
		public ScreenGroup ScreenSetting;
		
		[System.Serializable]
		public enum AssetDataSave
		{
			Binary,
			Cache
		};
		
		[System.Serializable]
		public class AssetbundleGroup
		{
			public bool[] createBundleType = new bool[3]{true,false,false};
			public AssetDataSave ads;
		}

		[System.Serializable]
		public class AutoCompleteGroup
		{
			public float dropboxCount = 5;
		}

		[System.Serializable]
		public class UserKeyGroup
		{
			public KeyCode vrModeKey = KeyCode.LeftAlt;
			public KeyCode generalModeKey = KeyCode.Escape;
			public KeyCode helpKey = KeyCode.F1;
		}
		
		[System.Serializable]
		public class CameraGroup
		{
			public float angleMin = 40;
			public float angleMax = 90;
			public float[] distance = {140,400,800,1600,2600,4000,5800,8200};
		}
		
		[System.Serializable]
		public class MobileGroup
		{
			public float zoomRate = 8;
			public float panSpeed = 8;
			public float rotateSpeed = 5;
		}
		
		public AssetbundleGroup AssetBundle;
		public CameraGroup Camera;
		public UserKeyGroup UserKey;
		public AutoCompleteGroup Autocomplete;
		public MobileGroup Mobile;
	}
	
	[System.Serializable]
	public class ConfigGroup                                                          //-----------------------------------------------------------------Config
	{
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

		[System.Serializable]
		public class AccessGroup
		{
			public MarkAccess mark;
			public BuildingAccess building;
		}
		
		public string phpurl = "" ;
		public AccessGroup Access;
	}
	
	[System.Serializable]
	public class CoordinateGroup                                                          //-----------------------------------------------------------------Coordinate
	{
		public double[] center;
		public double[] mouse;
	}

	[System.Serializable]
	public class CurrentStatusGroup                                                        //-----------------------------------------------------------------CurrentStatus
	{
		public bool is3DCam;
		public bool isChangeKey;
		public bool isMenu;
		public bool isSearchbar;
		public bool isMarkPoint;
		public bool isHelp;
		public bool isReady;
		public bool isAutoComplete;
	}

}
