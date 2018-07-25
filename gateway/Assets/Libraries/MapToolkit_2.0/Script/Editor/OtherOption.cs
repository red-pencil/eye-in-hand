#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class OtherOption  : EditorWindow
{

		public enum AssetDataSave
	{
		Binary,
		Cache
	};

			public enum Platform
		{
		WebPlayer,
			Standalone,
			Mobile
			
		};
	
	private Manager mg;
	private AssetDataSave ads;
	private Platform platform;
	private bool showLevel;
	private float buildingCount;
	private Vector2 option_scroll = Vector2.zero;
	
	void OnGUI ()
	{
		mg = GameObject.Find ("Manager").GetComponent<Manager> (); 		
		option_scroll= EditorGUILayout.BeginScrollView(option_scroll);
		EditorGUILayout.BeginVertical();
		GUILayout.Space (10);
		//DefinePlatform();
		UserKey();
		Option();
		MobileSetting();
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();
	}
	
	void DefinePlatform(){
		GUI.backgroundColor = Color.cyan;
		GUIStyle myButtonStyle = new GUIStyle (GUI.skin.box);
		myButtonStyle.normal.textColor = Color.white;
		GUILayout.Box ("Screen", myButtonStyle, GUILayout.ExpandWidth (true));
		GUI.backgroundColor = Color.white;	
		GUILayout.Space (6f);	
				if(mg.sy_OtherOption != null){
		         string str_platform= System.Enum.GetName (typeof(Platform), mg.sy_Editor.platform);
		         platform = (Platform)System.Enum.Parse (typeof(Platform), str_platform);
		         mg.sy_Editor.platform = (int)(Platform)EditorGUILayout.EnumPopup ("  Platform", platform);
	
		}
		GUILayout.Space (6f);	
		GUILayout.Space (6f);	
		
	}
	
	void Option(){
	GUI.backgroundColor = Color.cyan;
		GUIStyle myButtonStyle = new GUIStyle (GUI.skin.box);
		myButtonStyle.normal.textColor = Color.white;
		GUILayout.Box ("Camera", myButtonStyle, GUILayout.ExpandWidth (true));
		GUI.backgroundColor = Color.white;	
		GUILayout.Space (6f);
		
		mg.sy_OtherOption.Camera.angleMin =EditorGUILayout.FloatField("  Min",mg.sy_OtherOption.Camera.angleMin,GUILayout.ExpandWidth(true));
		mg.sy_OtherOption.Camera.angleMax =EditorGUILayout.FloatField("  Max",mg.sy_OtherOption.Camera.angleMax);
		GUILayout.Space (6);
		
		GUI.backgroundColor = Color.cyan;
		GUI.backgroundColor = Color.white;	
		GUILayout.Space (6f);
		
		EditorGUI.indentLevel++;

		showLevel =  EditorGUILayout .Foldout(showLevel,"Distance(m)");
		GUILayout.Space (6f);

		if(showLevel){
			EditorGUI.indentLevel++;
	      for(int i=0; i< 8; i++){
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Level"+(20-i).ToString(),GUILayout.Width(80));
			mg.sy_OtherOption.Camera.distance[i]=Mathf.RoundToInt( EditorGUILayout.Slider(mg.sy_OtherOption.Camera.distance[i],0,10000));
			EditorGUILayout.EndHorizontal();
			}
			GUILayout.Space (6f);
			if(GUILayout.Button("Default Setting",GUILayout.Height(30))){
				DefaultSetting();
			}
			EditorGUI.indentLevel--;
		}
		
		GUILayout.Space (6f);
		//    GUILayout.Space (6);
			EditorGUI.indentLevel--;
		
		GUI.backgroundColor = Color.cyan;
		GUILayout.Box ("AutoComplete", myButtonStyle, GUILayout.ExpandWidth (true));
		GUI.backgroundColor = Color.white;	
		GUILayout.Space (6f);

		EditorGUI.indentLevel++;
	  mg.sy_OtherOption.Autocomplete.dropboxCount = Mathf.RoundToInt( EditorGUILayout.Slider("Dropbox Count",mg.sy_OtherOption.Autocomplete.dropboxCount,1,10));
		EditorGUI.indentLevel--;
		
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// AssetBundle
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		GUILayout.Space (6f);
		GUI.backgroundColor = Color.cyan;
		GUILayout.Box ("Assetbundle", myButtonStyle, GUILayout.ExpandWidth (true));
		GUI.backgroundColor = Color.white;	
		GUILayout.Space (6f);

		if(mg.sy_Editor != null){
		string str_ads = System.Enum.GetName (typeof(AssetDataSave), mg.sy_Editor.ads);
		ads = (AssetDataSave)System.Enum.Parse (typeof(AssetDataSave), str_ads);
		mg.sy_Editor.ads = (int)(AssetDataSave)EditorGUILayout.EnumPopup ("  Data Loading Method", ads);
			
		#if (UNITY_ANDROID || UNITY_IPHONE || UNITY_WEBPLAYER)
			ads = AssetDataSave.Cache;
			mg.sy_Editor.ads =1;
		#endif
					}

		GUILayout.Space (6f);
		
		string[] str_platform ={" Standalone"," iOS"," Android" };
		BeginContents ();
		EditorGUILayout.LabelField(" ▶AssetBundle Platform");
		EditorGUI.indentLevel++;
		for(int k =0;  k<3;k++){
		  mg.sy_OtherOption.AssetBundle.createBundleType[k] = EditorGUILayout.Toggle(str_platform[k],  mg.sy_OtherOption.AssetBundle.createBundleType[k]);
				}
		EditorGUI.indentLevel--;
		EndContents();
	}
	
		 void DefaultSetting ()
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		
		mg.sy_OtherOption.Camera.distance [0] = 140;
		mg.sy_OtherOption.Camera.distance [1] = 400;
		mg.sy_OtherOption.Camera.distance [2] = 800;
		mg.sy_OtherOption.Camera.distance [3] = 1600;
		mg.sy_OtherOption.Camera.distance [4] = 2600;
		mg.sy_OtherOption.Camera.distance [5] = 4000;
		mg.sy_OtherOption.Camera.distance [6] = 5800;
		mg.sy_OtherOption.Camera.distance [7] = 8200;
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
	
	void UserKey(){
		GUI.backgroundColor = Color.cyan;
		GUIStyle myButtonStyle = new GUIStyle (GUI.skin.box);
		myButtonStyle.normal.textColor = Color.white;
		GUI.SetNextControlName ("UserKey");
		GUILayout.Box ("User Key", myButtonStyle, GUILayout.ExpandWidth (true));
		GUI.backgroundColor = Color.white;	
		GUILayout.Space (6f);
		
			if (GUI.GetNameOfFocusedControl () == "3DMode") {
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None ){
			 mg.sy_OtherOption.UserKey.vrModeKey =   Event.current.keyCode;
				GUI.FocusControl("");
		}
		}
		else if (GUI.GetNameOfFocusedControl () == "2DMode") {
					if (Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None ){
			 mg.sy_OtherOption.UserKey.generalModeKey =   Event.current.keyCode;
			GUI.FocusControl("");
		}
		}
			else if (GUI.GetNameOfFocusedControl () == "Help") {
					if (Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None ){
			 mg.sy_OtherOption.UserKey.helpKey =   Event.current.keyCode;
			GUI.FocusControl("");
		}
	}
		
		GUI.SetNextControlName ("3DMode");
		EditorGUILayout.TextField("  3D Mode",mg.sy_OtherOption.UserKey.vrModeKey.ToString());
		GUI.SetNextControlName ("2DMode");
		EditorGUILayout.TextField("  2D Mode",mg.sy_OtherOption.UserKey.generalModeKey.ToString());
		GUI.SetNextControlName ("Help");
		EditorGUILayout.TextField("  Help",mg.sy_OtherOption.UserKey.helpKey.ToString());
	}
	
	void MobileSetting(){
		GUI.backgroundColor = Color.cyan;
		GUIStyle myButtonStyle = new GUIStyle (GUI.skin.box);
		myButtonStyle.normal.textColor = Color.white;
		GUILayout.Box ("Mobile VR Mode Setting", myButtonStyle, GUILayout.ExpandWidth (true));
		GUI.backgroundColor = Color.white;	
		GUILayout.Space (6f);	
		
		EditorGUI.indentLevel++;
	    mg.sy_OtherOption.Mobile.zoomRate = Mathf.RoundToInt( EditorGUILayout.Slider("Zoom Rate",mg.sy_OtherOption.Mobile.zoomRate,1,10));
		GUILayout.Space (6f);
		mg.sy_OtherOption.Mobile.panSpeed = Mathf.RoundToInt( EditorGUILayout.Slider("Pan Speed",mg.sy_OtherOption.Mobile.panSpeed,1,10));
		GUILayout.Space (6f);
		mg.sy_OtherOption.Mobile.rotateSpeed = Mathf.RoundToInt( EditorGUILayout.Slider("Rotate Speed",mg.sy_OtherOption.Mobile.rotateSpeed,1,10));
		GUILayout.Space (6f);
		EditorGUI.indentLevel--;
	}

	
}
#endif