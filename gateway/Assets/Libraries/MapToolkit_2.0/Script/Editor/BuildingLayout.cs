#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.IO;

public class BuildingLayout : EditorWindow
{
	GameObject building;
	GameObject[] modeling;
	public bool isDetailInfo;
	public bool isPreview;
	public bool isSetting;
	public int select_num;
	public	List<GameObject> selectedObj = new List<GameObject> ();
	Vector2 mScroll = Vector2.zero;
	Transform target;
	string[][] contents_xml;
	Calculation cal = new Calculation ();
	bool isRun;
	CreateAssetBundle cab = new CreateAssetBundle ();

	void OnGUI ()
	{
		if (!isRun) {
			CreateObjectInHierarchy ();
			isRun = true;
		}

		InitiSetting ();
		GUILayout.Space (10);
		
		if (isDetailInfo && modeling != null && selectedObj.Count != 0) {
			Preview (select_num);
			VariableView ();
		}
		
		
		if (!isDetailInfo) {
			BaseView ();
		    ModelingResize();
			VariableView ();
		}
		Recognizes ();
		
		if (isSetting && !EditorApplication.isPlaying && modeling != null) {
			ImportBuildingData ();	
			if (modeling != null) {
				foreach (GameObject _target in modeling) {
					CreatePrefab_AssetBundle (_target);
					DestroyImmediate (_target);
				}
			}
			AssetDatabase.Refresh ();
			isSetting = false;
		}
		
		if(isResize &&  !EditorApplication.isPlaying){
			ImportBuildingData ();	
			isResize = false;
		}
	
		if (EditorApplication.isPlaying)
			IsPlaying ();
	}
	
	void CreateObjectInHierarchy ()
	{
		if (GameObject.Find ("> Building Layout") == null) {
			building = new GameObject ("> Building Layout");
		} else {
			building = GameObject.Find ("> Building Layout");
		}
	}
	
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Initial Setting
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
	void InitiSetting ()
	{
		CreateObjectInHierarchy ();
		if(building != null)
			for (int i=0; i< building.transform.childCount; i++) {
				GameObject _building = building.transform.GetChild (i).gameObject;
				if (_building.GetComponent<BuildingData> () == null) {
					_building.AddComponent<BuildingData> ();
					_building.GetComponent<BuildingData> ().coordinate = "0,0";
				}
			}
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Preview
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
	void Preview (int num)
	{
		if (isPreview) {
			isPreview = false;
		}   
		
		BeginContents ();
		try {
			target = modeling [num].transform;
			GUILayout.Space (5);
			GUI.backgroundColor = Color.cyan;
			target.name = EditorGUILayout.TextField ("Name", target.name, GUILayout.ExpandWidth (true));
			GUI.backgroundColor = Color.white;
			GUILayout.Space (5);
		
			target.localPosition = EditorGUILayout.Vector3Field ("Position", target.localPosition);
			target.localEulerAngles = EditorGUILayout.Vector3Field ("Rotation", target.localEulerAngles);
			target.localScale = EditorGUILayout.Vector3Field ("Scale", target.localScale);
			
			string str = target.GetComponent<BuildingData> ().coordinate;
			string[] strs = str.Split (',');
			if (EditorApplication.isPlaying) {
				double[] cordin = cal.CurrentCoordinate (target.position);	
				EditorGUILayout.Vector2Field ("Coordinate", new Vector2 (float.Parse (cordin [0].ToString ()), float.Parse (cordin [1].ToString ())));
				EditorGUILayout.IntField ("Zoom", GameObject.Find ("Manager").GetComponent<Manager> ().sy_Map.zoom, GUILayout.ExpandWidth (true));
			} else {
				EditorGUILayout.Vector2Field ("Coordinate", new Vector2 (float.Parse (strs [0]), float.Parse (strs [1])));
				EditorGUILayout.IntField ("Zoom", target.GetComponent<BuildingData> ().zoom, GUILayout.ExpandWidth (true));
			}
			
			GUILayout.Space (15);
		} catch {
			select_num = 0;
			isDetailInfo = false;
			//isHelp = false;
			modeling = null;
			selectedObj.Clear ();	
		}
		EndContents ();
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// Preview > Calculate
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
		if (GUILayout.Button ("Calculate", GUILayout.Height (30), GUILayout.ExpandWidth (true))) { 			
			if (UnityEditor.EditorApplication.isPlaying) {
				if (modeling != null) {
					foreach (GameObject _target in modeling) {
						double[] _coordinate = cal.CurrentCoordinate (_target.transform.position);
						_target.GetComponent<BuildingData> ().coordinate = _coordinate [0].ToString () + "," + _coordinate [1].ToString ();
						_target.GetComponent<BuildingData> ().zoom = GameObject.Find ("Manager").GetComponent<Manager> ().sy_Map.zoom;
					}
				} else {
					//	help_message = "Select a gameobject";
					//isHelp = true;
				}
				CreateXML (modeling);
				UnityEditor.EditorApplication.isPlaying = false;
				isSetting = true;
			} else {
				//	help_message = "Playmode is possible";
				//	isHelp = true;
			}
		}
	}
	
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Calculate...After Import Value
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
	
	void ImportBuildingData ()
	{
		
		XmlDocument XmlDoc = new XmlDocument ();
		XmlDoc.Load ("config.xml"); 
		XmlNode fristNode = XmlDoc.DocumentElement; 
		//	Debug.Log (fristNode.ChildNodes [0].ChildNodes [1].InnerText);	
			
		System.Array.Resize<string[]> (ref contents_xml, fristNode.ChildNodes.Count);	
			
		for (int i = 0; i < fristNode.ChildNodes.Count; i++) {	
				
			System.Array.Resize<string> (ref  contents_xml [i], fristNode.ChildNodes [i].ChildNodes.Count);
				
			for (int y = 0; y < fristNode.ChildNodes [i].ChildNodes.Count; y++) {
				contents_xml [i] [y] = fristNode.ChildNodes [i].ChildNodes [y].InnerText;
			}
		}
			
		for (int j = 0; j < modeling.Length; j++) {	
			if (modeling [j].GetComponent<BuildingData> () == null) {
				modeling [j].AddComponent<BuildingData> ();
			}
				
			// Position Setting
			string pos_str = contents_xml [j] [0];
			string[] pos_strs = pos_str.Split (',');
			Vector3 pos = new Vector3 (float.Parse (pos_strs [0]), float.Parse (pos_strs [1]), float.Parse (pos_strs [2]));
			modeling [j].transform.localPosition = pos;

			// Rotation Setting
	
			string rotaion_str = contents_xml [j] [1];
			string[] rotaion_strs = rotaion_str.Split (',');
			Vector3 rotation = new Vector3 (MathRound (rotaion_strs [0]), MathRound (rotaion_strs [1]), MathRound (rotaion_strs [2]));
			modeling [j].transform.rotation = Quaternion.Euler (rotation);

			// Scale Setting
				
			string scale_str = contents_xml [j] [2];
			string[] scale_strs = scale_str.Split (',');
			Vector3 scale = new Vector3 (float.Parse (scale_strs [0]), float.Parse (scale_strs [1]), float.Parse (scale_strs [2]));
			modeling [j].transform.localScale = scale;

			// Coordinate Setting
				
			string cord_str = contents_xml [j] [3];
			modeling [j].GetComponent<BuildingData> ().coordinate = cord_str;
				
			// Zoom Setting
			string zoom_str = contents_xml [j] [4];
			modeling [j].GetComponent<BuildingData> ().zoom = int.Parse (zoom_str);	
		}
		this.Repaint ();
	}
	

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Variable View (Add & Reset)
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
	void BaseView ()
	{
		if (GUILayout.Button ("Recognizes Building", GUILayout.Height (30), GUILayout.ExpandWidth (true))) { //Calculate coordinate values
			if (Selection.activeGameObject == null) {
				//isHelp = true;
			} else {
				selectedObj.Clear ();
				foreach (GameObject o in Selection.gameObjects) {
					selectedObj.Add (o);
				}
				modeling = selectedObj.ToArray ();
			}
		}
	}
	bool isResize;
	
	void ModelingResize(){
		if(selectedObj.Count != 0 &&modeling != null)
		if (GUILayout.Button ("Resize Building", GUILayout.Height (30), GUILayout.ExpandWidth (true))&& EditorApplication.isPlaying)  {
				CreateXML (modeling);
				UnityEditor.EditorApplication.isPlaying = false;
				isResize = true;
		}
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Variable View (Add & Reset)
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
	void VariableView ()
	{
		
		if (modeling != null && selectedObj.Count != 0){
		if (GUILayout.Button ("Add Item", GUILayout.Height (30), GUILayout.ExpandWidth (true))) { //Calculate coordinate values
			if (Selection.activeGameObject != null) {
				foreach (GameObject addObj in Selection.gameObjects) {
					selectedObj.Add (addObj);
				}
				selectedObj = GetDistinctValues (selectedObj.ToArray ());
				modeling = selectedObj.ToArray ();
			} else {
				//	isHelp = true;	
			}
		}
		}
		
		if (GUILayout.Button ("Reset", GUILayout.Height (30), GUILayout.ExpandWidth (true))) { //Calculate coordinate values
			Reset ();
		}	
	}
	
	void Reset ()
	{
		select_num = 0;
		isDetailInfo = false;
		//	isHelp = false;
		modeling = null;
		selectedObj.Clear ();	
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Recognizes Building Object
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	void Recognizes ()
	{
		if (selectedObj.Count != 0 && modeling != null) {
			mScroll = GUILayout.BeginScrollView (mScroll);
		
			for (int i =0; i<modeling.Length; i++) {
				
				if (!isDetailInfo) {
					GUI.backgroundColor = new Color (0.8f, 0.8f, 0.8f);
				} else {
					bool highlight = (select_num == i);
					GUI.backgroundColor = highlight ? Color.white : new Color (0.8f, 0.8f, 0.8f);
				}
				
				GUILayout.BeginHorizontal ("AS TextArea", GUILayout.MinHeight (20f));
				
				if (GUILayout.Button ((i + 1).ToString (), "OL TextField", GUILayout.Height (20f))) {
					
					//	isHelp = false;
					Selection.activeObject = modeling [i];
					# if !(UNITY_3_5)
				 SceneView.GetWindow (System.Type.GetType ("UnityEditor.SceneView,UnityEditor.dll")).Focus ();
					#endif 
				//	if (SceneView.currentDrawingSceneView != null)
				//		SceneView.lastActiveSceneView.FrameSelected ();

					isDetailInfo = true;
					isPreview = true;
					select_num = i;
				}
				
				if (isDetailInfo) {
					if (i == select_num) 
						GUI.backgroundColor = Color.white;
					else 
						GUI.backgroundColor = new Color (0.8f, 0.8f, 0.8f);
					
				} else {
					GUI.backgroundColor = Color.white;
				}
				
				modeling [i] = (GameObject)EditorGUILayout.ObjectField ("", modeling [i], typeof(GameObject), true, GUILayout.Width (100), GUILayout.ExpandWidth (false));
				
				if (GUILayout.Button ("─", GUILayout.Width (50))) {
					selectedObj.RemoveAt (i);
					if (selectedObj.Count == 0) {
						isDetailInfo = false;
						selectedObj.Clear ();
			
						selectedObj = GetDistinctValues (selectedObj.ToArray ());
						modeling = selectedObj.ToArray ();
					} else {
						selectedObj = GetDistinctValues (selectedObj.ToArray ());
						modeling = selectedObj.ToArray ();
					}
		
				}

				
				GUI.backgroundColor = new Color (0.8f, 0.8f, 0.8f);
				GUILayout.EndHorizontal ();
			}
		
		
			GUILayout.EndScrollView ();
		}
	}
	
	
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Get Distinct Values
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public List<GameObject> GetDistinctValues<GameObject> (GameObject[] array)
	{
		List<GameObject> tmp = new List<GameObject> ();
		for (int i = 0; i < array.Length; i++) {
			if (tmp.Contains (array [i]))
				continue;
			tmp.Add (array [i]);
		}
		return tmp;
	}
	
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///  Is Playing
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
	void IsPlaying ()
	{
		if (building != null)
			for (int i=0; i< building.transform.childCount; i++) {
				GameObject _building = building.transform.GetChild (i).gameObject;
				if (_building.GetComponent<MeshRenderer> () != null) {
					if (_building != Selection.activeObject) {
						_building.GetComponent<MeshRenderer> ().enabled = false;
					} else {
						_building.GetComponent<MeshRenderer> ().enabled = true;
					}
				}
			}
	}
	
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// CreateXML
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
	
	void CreateXML (GameObject[] xmlTarget)
	{
		XmlDocument NewXmlDoc = new XmlDocument ();
		XmlNode Source = NewXmlDoc.CreateElement ("", "Source", "");
		NewXmlDoc.AppendChild (Source);               
		NewXmlDoc.Save ("config.xml");
		for (int i=0; i<xmlTarget.Length; i++) {
			Transform target_xml = xmlTarget [i].transform;
			XmlDocument XmlDoc = new XmlDocument ();
			XmlDoc.Load ("config.xml"); 
			XmlNode FristNode = XmlDoc.DocumentElement; 
			XmlElement root = XmlDoc.CreateElement ("Modeling"); 
			root.SetAttribute ("Id", i.ToString ());
			root.SetAttribute ("Name", xmlTarget [i].name);
			
			string pos_xml = target_xml.localPosition.x.ToString () + "," + target_xml.localPosition.y.ToString () + "," + target_xml.localPosition.z.ToString ();
			string angle_xml = target_xml.eulerAngles.x.ToString () + "," + target_xml.eulerAngles.y.ToString () + "," + target_xml.eulerAngles.z.ToString ();
			string scale_xml = target_xml.localScale.x.ToString () + "," + target_xml.localScale.y.ToString () + "," + target_xml.localScale.z.ToString ();
			string coordin_xml = target_xml.GetComponent<BuildingData> ().coordinate.ToString ();
			string zoom_xml = target_xml.GetComponent<BuildingData> ().zoom.ToString ();
			
			root.AppendChild (CreateNode (XmlDoc, "Position", pos_xml));             
			root.AppendChild (CreateNode (XmlDoc, "Rotation", angle_xml));             
			root.AppendChild (CreateNode (XmlDoc, "Scale", scale_xml));    
			root.AppendChild (CreateNode (XmlDoc, "Coordinate", coordin_xml));    
			root.AppendChild (CreateNode (XmlDoc, "Zoom", zoom_xml));    
		
			FristNode.AppendChild (root);
			XmlDoc.Save ("config.xml");
		}
	}

	protected XmlNode CreateNode (XmlDocument xmlDoc, string name, string innerXml)
	{
		XmlNode node = xmlDoc.CreateElement (string.Empty, name, string.Empty);
		node.InnerXml = innerXml;
		return node;
	}
	
	void CreatePrefab_AssetBundle (GameObject fileObj)
	{
		string dirPath = "Assets/MapToolkit_2.0/Temp/BuildingData/";
		//	string dirPath = Application.dataPath + "/Temp/BuildingData/";

		Object prefab = (Object)PrefabUtility.CreateEmptyPrefab (dirPath + fileObj.name + ".prefab");
		PrefabUtility.ReplacePrefab (fileObj, prefab, ReplacePrefabOptions.ConnectToPrefab);
		cab.Build (fileObj);
		AssetDatabase.Refresh ();

	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Contents
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	static public void BeginContents ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Space (4f);
		EditorGUILayout.BeginHorizontal ("AS TextArea", GUILayout.MinHeight (10f));
		GUILayout.BeginVertical ();
		GUILayout.Space (2f);
	}

	static public void EndContents ()
	{
		GUILayout.Space (3f);
		GUILayout.EndVertical ();
		EditorGUILayout.EndHorizontal ();
		GUILayout.Space (3f);
		GUILayout.EndHorizontal ();
		GUILayout.Space (3f);
	}
	
	float MathRound (string _value)
	{
		float f_value = float.Parse (_value);
		return Mathf.Round (f_value / 0.001f) * 0.001f;
		
		//	return f_value;
	}
	
	
}
#endif