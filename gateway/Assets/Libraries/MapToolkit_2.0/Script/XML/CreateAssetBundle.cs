#if UNITY_EDITOR
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Text;

public class CreateAssetBundle
{
	
	string[] file_name;
	Object[] file_Obj;
	
	public  void Build (GameObject _file)
	{	
		Manager mg = (Manager)GameObject.Find ("Manager").GetComponent<Manager> ();
		string filename = _file.name;
		string path = "Assets/MapToolkit_2.0/Temp/BuildingData/" + filename + ".prefab";
		Object mainAsset = (Object)AssetDatabase.LoadAssetAtPath (path, typeof(Object)); 
		
		if (mg.sy_OtherOption.AssetBundle.createBundleType [0]) {
			
			CreateFolder ("Assets/MapToolkit_2.0/Temp/BuildingData/Standalone");
			string file_path = "Assets/MapToolkit_2.0/Temp/BuildingData/Standalone/" + GetBuildingData (_file) + ".csy";
			BuildPipeline.PushAssetDependencies (); 
			BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets; 
			BuildPipeline.BuildAssetBundle (mainAsset, null, file_path, options, BuildTarget.StandaloneWindows);    
			BuildPipeline.PopAssetDependencies ();
		}
		if (mg.sy_OtherOption.AssetBundle.createBundleType [1]) {
			CreateFolder ("Assets/MapToolkit_2.0/Temp/BuildingData/iPhone");
			string file_path = "Assets/MapToolkit_2.0/Temp/BuildingData/iPhone/" + GetBuildingData (_file) + ".csy";
			BuildPipeline.PushAssetDependencies (); 
			BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets; 
			BuildPipeline.BuildAssetBundle (mainAsset, null, file_path, options, BuildTarget.iOS);    
			BuildPipeline.PopAssetDependencies ();
		}
		if (mg.sy_OtherOption.AssetBundle.createBundleType [2]) {
			CreateFolder ("Assets/MapToolkit_2.0/Temp/BuildingData/Android");
			string file_path = "Assets/MapToolkit_2.0/Temp/BuildingData/Android/" + GetBuildingData (_file) + ".csy";
			BuildPipeline.PushAssetDependencies (); 
			BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets; 
			BuildPipeline.BuildAssetBundle (mainAsset, null, file_path, options, BuildTarget.Android);    
			BuildPipeline.PopAssetDependencies ();
		}
		FileDelete ("Assets/MapToolkit_2.0/Temp/BuildingData/" + _file.name + ".prefab");
		
		if (mg.sy_OtherOption.AssetBundle.createBundleType [0].Equals (false) &&
			mg.sy_OtherOption.AssetBundle.createBundleType [1].Equals (false) &&
			mg.sy_OtherOption.AssetBundle.createBundleType [2].Equals (false))
			Debug.Log ("[Other option > Assetbundle>Platform 'Nothing selected check' ");
		
	}
	
	void CreateFolder (string path)
	{
		DirectoryInfo di = new DirectoryInfo (path);
		if (di.Exists == false) {
			di.Create ();
		}
	}

	string GetBuildingData (GameObject _file)
	{
		string coordinate = _file.GetComponent<BuildingData> ().coordinate;
		string result = _file.name + "&" + coordinate + "&";
		return result;
	}
	
	public static bool FileDelete (string path)
	{
		#if !UNITY_WEBPLAYER
		FileInfo file = new FileInfo (path);

		if (file.Exists) {
			file.Delete ();
			return true;
		} else {
			return false;
		}
		#else
		return false;
			#endif
	}	
}
		#endif