 #if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class Xml_Manager: MonoBehaviour
{

	// Use this for initialization

	
	[System.Serializable]

	public class XmlMarker
	{
		public string locationName;
		public string  coordinate_x;
		public string  coordinate_y;
		public Color normalColor;
		public int[] normalColor_2;
		public int normalColor_r;
		public int normalColor_g;
		public int normalColor_b;
		public int normalColor_a;
		public float normalPixelInset_x;
		public float normalPixelInset_y;
		public float normalPixelInset_w;
		public float normalPixelInset_h;
		public string normalSprite;
		public string hoverSprite;
		public string activeSprite;
		public bool textShow;
		public float textPixelInset_x;
		public float textPixelInset_y;
		public int textFontSize;
		public float textColor_r;
		public float textColor_g;
		public float textColor_b;
		public float textColor_a;
	
		public enum textAlignment
		{
			left,
			center,
			right,
		}
		
	}
	
	[HideInInspector]
	public MarkXmlUI markXmlUI;
	[HideInInspector]
	public XmlMarker _maker;
	[HideInInspector]
	public  Dictionary<string, int> xmlDic = new Dictionary<string, int> ();
	[HideInInspector]
	public  ArrayList xmlArrayList = new ArrayList ();
	[HideInInspector]
	public int selectListIndex;
	private string  prefabPath = "Assets/MapToolkit_2.0/Temp/Prefabs/MarkXML.prefab";
	[HideInInspector]
	public string selectedMark;
	[HideInInspector]
	public  string selection = null;
	[HideInInspector]
	public int selection_number = 0;
	[HideInInspector]
	public FileStatus loadFileStatus;
	[HideInInspector]
	public  FileStatus saveFileStatus;

	
	public enum FileStatus
	{
		Ready,
		Success,
		Cancel,
		Error
	}

	public void  ListClear ()
	{
		xmlArrayList.Clear (); 
		xmlDic.Clear (); 
	}
	
	public void ResetFileStatus ()
	{
		loadFileStatus = FileStatus.Ready;
		saveFileStatus = FileStatus.Ready;
	}
	
	public void LoadFileStatus (int num)
	{
		string tex = System.Enum.GetName (typeof(FileStatus), num);
		loadFileStatus = (FileStatus)System.Enum.Parse (typeof(FileStatus), tex);
	}
	
	public void SaveFileStatus (int num)
	{
		string tex = System.Enum.GetName (typeof(FileStatus), num);
		saveFileStatus = (FileStatus)System.Enum.Parse (typeof(FileStatus), tex);
	}
	
	public void SelectMark (string spriteName, int index)
	{

		XmlMarker tmp_xmlData = xmlArrayList [index] as  XmlMarker;
		
		selectedMark = spriteName;

		GameObject markPrefab = AssetDatabase.LoadAssetAtPath (prefabPath, typeof(GameObject)) as GameObject;
		MarkXmlUI mark_ui = markPrefab.GetComponent<MarkXmlUI> ();
		
		mark_ui.name = tmp_xmlData.locationName;
		mark_ui.coordinate_x = tmp_xmlData.coordinate_x;
		mark_ui.coordinate_y = tmp_xmlData.coordinate_y;
		
		mark_ui.normalPixelInset_x = tmp_xmlData.normalPixelInset_x;
		mark_ui.normalPixelInset_y = tmp_xmlData.normalPixelInset_y;
		mark_ui.normalPixelInset_w = tmp_xmlData.normalPixelInset_w;
		mark_ui.normalPixelInset_h = tmp_xmlData.normalPixelInset_h;

		mark_ui.textPixelInset_x = tmp_xmlData.textPixelInset_x;
		mark_ui.textPixelInset_y = tmp_xmlData.textPixelInset_y;
		mark_ui.textFontSize = tmp_xmlData.textFontSize;
		
		mark_ui.normalColor = new Color (tmp_xmlData.normalColor_r / 255.0f, tmp_xmlData.normalColor_g / 255.0f, tmp_xmlData.normalColor_b / 255.0f, tmp_xmlData.normalColor_a / 255.0f);
		
		mark_ui.textShow = tmp_xmlData.textShow;
		
		mark_ui.textColor = new Color (tmp_xmlData.textColor_r / 255.0f, tmp_xmlData.textColor_g / 255.0f, tmp_xmlData.textColor_b / 255.0f, tmp_xmlData.textColor_a / 255.0f);
		
		mark_ui.mainTexture = Resources.Load ("UI/" + tmp_xmlData.normalSprite)as Texture2D;  
		mark_ui.overTexture = Resources.Load ("UI/" + tmp_xmlData.hoverSprite)as Texture2D;      
		mark_ui.activeTexture = Resources.Load ("UI/" + tmp_xmlData.activeSprite)as Texture2D;   
		
		Select (markPrefab);
	}

	static public void Select (GameObject go)
	{
		Selection.activeGameObject = go;
	}

	public void SelectMarkClear ()
	{
		GameObject markPrefab = AssetDatabase.LoadAssetAtPath (prefabPath, typeof(GameObject)) as GameObject;
		
		MarkXmlUI mark_ui = markPrefab.GetComponent<MarkXmlUI> ();
		
		mark_ui.name=mark_ui.coordinate_x = mark_ui.coordinate_y = "";
		mark_ui.normalPixelInset_x = mark_ui.normalPixelInset_y = mark_ui.normalPixelInset_w = mark_ui.normalPixelInset_h = mark_ui.textPixelInset_x = mark_ui.textPixelInset_y = mark_ui.textFontSize = 0;
		mark_ui.normalColor = mark_ui.textColor = Color.white; 
		mark_ui.textShow = false;
		mark_ui.mainTexture = mark_ui.overTexture = mark_ui.activeTexture = null;	
		Select (markPrefab);
	}
	
	public void updateXmlMark (int index)
	{
		XmlMarker tmp_xmlData = xmlArrayList [index] as  XmlMarker;
		GameObject Prefab = AssetDatabase.LoadAssetAtPath (prefabPath, typeof(GameObject)) as GameObject;
		
		MarkXmlUI mark_ui = Prefab.GetComponent<MarkXmlUI> ();

		tmp_xmlData.locationName = mark_ui.name;
		tmp_xmlData.coordinate_x = mark_ui.coordinate_x;
		
		tmp_xmlData.coordinate_y = mark_ui.coordinate_y;
		tmp_xmlData.normalPixelInset_x = mark_ui.normalPixelInset_x;

		tmp_xmlData.normalPixelInset_y = mark_ui.normalPixelInset_y;
		tmp_xmlData.normalPixelInset_w = mark_ui.normalPixelInset_w;
		tmp_xmlData.normalPixelInset_h = mark_ui.normalPixelInset_h;
		
		tmp_xmlData.normalPixelInset_x = mark_ui.normalPixelInset_x;
		tmp_xmlData.normalPixelInset_y = mark_ui.normalPixelInset_y;

		tmp_xmlData.textFontSize = mark_ui.textFontSize;
		
		tmp_xmlData.normalColor_r = (int)(mark_ui.normalColor.r * 255);
		tmp_xmlData.normalColor_g = (int)(mark_ui.normalColor.g * 255);
		tmp_xmlData.normalColor_b = (int)(mark_ui.normalColor.b * 255);
		tmp_xmlData.normalColor_a = (int)(mark_ui.normalColor.a * 255);

		tmp_xmlData.textShow = mark_ui.textShow;
		tmp_xmlData.textFontSize = mark_ui.textFontSize;
		
		tmp_xmlData.textColor_r = (int)(mark_ui.textColor.r * 255);
		tmp_xmlData.textColor_g = (int)(mark_ui.textColor.g * 255);
		tmp_xmlData.textColor_b = (int)(mark_ui.textColor.b * 255);
		tmp_xmlData.textColor_a = (int)(mark_ui.textColor.a * 255);
		

		if (mark_ui.mainTexture != null) {
			tmp_xmlData.normalSprite = mark_ui.mainTexture.name; 
		}
		if (mark_ui.overTexture != null) {
			tmp_xmlData.hoverSprite = mark_ui.overTexture.name;
		}
		if (mark_ui.activeTexture != null) {
			tmp_xmlData.activeSprite = mark_ui.activeTexture.name;
		}
	
		
		xmlArrayList [index] = tmp_xmlData;
	}
	
}

#endif