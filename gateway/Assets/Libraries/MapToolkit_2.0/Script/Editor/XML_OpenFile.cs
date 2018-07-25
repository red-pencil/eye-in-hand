#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class XML_OpenFile
{
	
	Xml_Manager manager;
	string message ;
	string FileName;
	public enum FileStatus
	{
		Ready,
		Success,
		Cancel,
		Error
	}
	
	public void Open ()
	{
		manager = GameObject.Find ("Manager").GetComponent<Xml_Manager> ();
		
		GUILayout.BeginHorizontal ();
		
		if (GUILayout.Button ("Open", GUILayout.Height (30), GUILayout.Width (100f))) {
			LoadData ();
		}	
		
		EditorGUILayout.HelpBox (message, MessageType.Info, true);
		
		string tex = System.Enum.GetName (typeof(FileStatus), manager.loadFileStatus);
		switch ((FileStatus)System.Enum.Parse (typeof(FileStatus), tex)) {
			
		case FileStatus.Ready :
			GUI.color = Color.gray;
			message = "Opens an existing XML file";
			GUI.color = Color.white;
			break;
				
		case FileStatus.Cancel :
			GUI.color = Color.red;
			message = "File operation cancel";
			GUI.color = Color.white;
			break;
					
		case FileStatus.Success :
			if (FileName != null) {
				GUI.color = Color.white;
				string[] s = FileName.Split ('/');
				message = "Selected file  : " + s [s.Length - 1];
				GUI.color = Color.white;
			}
			break;
		default :
			break;
		}

		GUILayout.EndHorizontal ();
	}

	void LoadData ()
	{
		Xml_Manager manager = GameObject.Find ("Manager").GetComponent<Xml_Manager> ();
		
		FileName = EditorUtility.OpenFilePanel ("XML Maker", "Assets/MapToolkit_2.0/Temp/XmlData", "xml");
		
		if (FileName == null || FileName == "") {
			manager.LoadFileStatus ((int)FileStatus.Cancel);
			return;
		} else {
			manager.ListClear ();	
			manager.ResetFileStatus ();
		}
		
		XML_Mark_Container xmlMarkCollection = XML_Mark_Container.Load (FileName);
		
		for (int i=0; i< xmlMarkCollection.XML_Marks.Length; i++) {
			
		
			Xml_Manager.XmlMarker xmlData = new Xml_Manager.XmlMarker ();
					
			xmlData.locationName = xmlMarkCollection.XML_Marks [i].Name;
			xmlData.coordinate_x = xmlMarkCollection.XML_Marks [i].coordinate_x;
			xmlData.coordinate_y = xmlMarkCollection.XML_Marks [i].coordinate_y;
				
			xmlData.normalPixelInset_x = xmlMarkCollection.XML_Marks [i].normalPixelInset_x;
			xmlData.normalPixelInset_y = xmlMarkCollection.XML_Marks [i].normalPixelInset_y;
			xmlData.normalPixelInset_w = xmlMarkCollection.XML_Marks [i].normalPixelInset_w;
			xmlData.normalPixelInset_h = xmlMarkCollection.XML_Marks [i].normalPixelInset_h;
				
				
			xmlData.normalSprite = xmlMarkCollection.XML_Marks [i].normalSprite;
			xmlData.normalColor_r = xmlMarkCollection.XML_Marks [i].normalColor_r;
			xmlData.normalColor_g = xmlMarkCollection.XML_Marks [i].normalColor_g;
			xmlData.normalColor_b = xmlMarkCollection.XML_Marks [i].normalColor_b;
			xmlData.normalColor_a = xmlMarkCollection.XML_Marks [i].normalColor_a;

			xmlData.hoverSprite = xmlMarkCollection.XML_Marks [i].hoverSprite;
			xmlData.activeSprite = xmlMarkCollection.XML_Marks [i].activeSprite;
				
			xmlData.textPixelInset_x = xmlMarkCollection.XML_Marks [i].textPixelInset_x;
			xmlData.textPixelInset_y = xmlMarkCollection.XML_Marks [i].textPixelInset_y;
				
			xmlData.textFontSize = xmlMarkCollection.XML_Marks [i].textFontSize;
			xmlData.textShow = xmlMarkCollection.XML_Marks [i].textShow;
				
			xmlData.textColor_r = xmlMarkCollection.XML_Marks [i].textColor_r;
			xmlData.textColor_g = xmlMarkCollection.XML_Marks [i].textColor_g;
			xmlData.textColor_b = xmlMarkCollection.XML_Marks [i].textColor_b;
			xmlData.textColor_a = xmlMarkCollection.XML_Marks [i].textColor_a;
				
				
			manager.xmlArrayList.Add (xmlData);
		}
		
		Xml_Manager.XmlMarker tmp_xmlData2 = manager.xmlArrayList [0] as Xml_Manager.XmlMarker;	
		
		manager.selection = tmp_xmlData2.locationName;
		manager.selection_number = 0;
		manager.SelectMarkClear ();	
		manager.selectedMark = "";
		
		manager.LoadFileStatus ((int)FileStatus.Success);
	}
}
#endif