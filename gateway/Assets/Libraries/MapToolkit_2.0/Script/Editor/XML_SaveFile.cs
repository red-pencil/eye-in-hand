#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;

public class XML_SaveFile
{
	
	Xml_Manager manager;
	
	enum FileStatus
	{
		Ready,
		Success,
		Cancel,
		Error
	}
	string message ;
	
	public void Save ()
	{
		manager = GameObject.Find ("Manager").GetComponent<Xml_Manager> ();
		
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Save", GUILayout.Height (30), GUILayout.Width (100f))) {
			SaveData ();
		}
		EditorGUILayout.HelpBox (message, MessageType.Info, true);
		
		string tex = System.Enum.GetName (typeof(FileStatus), manager.saveFileStatus);
		
		switch ((FileStatus)System.Enum.Parse (typeof(FileStatus), tex)) {
			
		case FileStatus.Ready :
			GUI.color = Color.gray;
			message = "Saves the current xml file.";
			GUI.color = Color.white;
			break;
				
		case FileStatus.Cancel :
			GUI.color = Color.red;
			message = ("File operation cancel.");
			GUI.color = Color.white;
			break;
			
		case FileStatus.Error :
			GUI.color = Color.red;
			message = ("You can't save the content is empty.");
			GUI.color = Color.white;
			break;
					
		case FileStatus.Success :
			GUI.color = Color.white;
			message = "Save Path : " + Application.streamingAssetsPath + "/MarkData";
			GUI.color = Color.white;
			AssetDatabase.Refresh ();
			break;
		default :
			break;
		}
		GUILayout.EndHorizontal ();

	}
	
	public void SaveData ()
	{
		Xml_Manager manager = GameObject.Find ("Manager").GetComponent<Xml_Manager> ();
		
		string sSavePath = EditorUtility.SaveFilePanel ("Save Mark XML File", "Assets/MapToolkit_2.0/Temp/MarkData", "noname", "xml");
		if (sSavePath == null || sSavePath == "") {
			manager.SaveFileStatus ((int)FileStatus.Cancel);
			return;
		}
	
		
		if (manager.xmlArrayList == null || manager.xmlArrayList.Count == 0) {
			manager.SaveFileStatus ((int)FileStatus.Error);
			return;
		}
		
		
		string xmlData;
		
		xmlData = "<MapMarkCollection><XML_Marks>";
		
		for (int i=0; i <manager.xmlArrayList.Count; i++) {
			

			Xml_Manager.XmlMarker tmp_xmlData = (Xml_Manager.XmlMarker)manager.xmlArrayList [i] as  Xml_Manager.XmlMarker;
		
			xmlData += "<XML_Mark name=\"" + tmp_xmlData.locationName + "\">";
			xmlData += "<coordinate_x>" + tmp_xmlData.coordinate_x + "</coordinate_x>";
			xmlData += "<coordinate_y>" + tmp_xmlData.coordinate_y + "</coordinate_y>";
					
			xmlData += "<normalSprite>" + tmp_xmlData.normalSprite + "</normalSprite>";
			xmlData += "<hoverSprite>" + tmp_xmlData.hoverSprite + "</hoverSprite>";
			xmlData += "<activeSprite>" + tmp_xmlData.activeSprite + "</activeSprite>";
					
			xmlData += "<normalColor_r>" + tmp_xmlData.normalColor_r + "</normalColor_r>";
			xmlData += "<normalColor_g>" + tmp_xmlData.normalColor_g + "</normalColor_g>";
			xmlData += "<normalColor_b>" + tmp_xmlData.normalColor_b + "</normalColor_b>";
			xmlData += "<normalColor_a>" + tmp_xmlData.normalColor_a + "</normalColor_a>";
					
			
			xmlData += "<normalPixelInset_x>" + tmp_xmlData.normalPixelInset_x + "</normalPixelInset_x>";
			xmlData += "<normalPixelInset_y>" + tmp_xmlData.normalPixelInset_y + "</normalPixelInset_y>";
			xmlData += "<normalPixelInset_w>" + tmp_xmlData.normalPixelInset_w + "</normalPixelInset_w>";
			xmlData += "<normalPixelInset_h>" + tmp_xmlData.normalPixelInset_h + "</normalPixelInset_h>";
		
			xmlData += "<textPixelInset_x>" + tmp_xmlData.textPixelInset_x + "</textPixelInset_x>";
			xmlData += "<textPixelInset_y>" + tmp_xmlData.textPixelInset_y + "</textPixelInset_y>";
					
			xmlData += "<textFontSize>" + tmp_xmlData.textFontSize + "</textFontSize>";
				
			
			int textCheck;
			if (tmp_xmlData.textShow)
				textCheck = 1;
			else
				textCheck = 0;
			xmlData += "<textShow>" + textCheck + "</textShow>";	
		
			xmlData += "<textColor_r>" + tmp_xmlData.textColor_r + "</textColor_r>";
			xmlData += "<textColor_g>" + tmp_xmlData.textColor_g + "</textColor_g>";
			xmlData += "<textColor_b>" + tmp_xmlData.textColor_b + "</textColor_b>";
			xmlData += "<textColor_a>" + tmp_xmlData.textColor_a + "</textColor_a>";	
					
			xmlData += "</XML_Mark>";

		}
		
		xmlData += "</XML_Marks></MapMarkCollection>";

		XML_Mark_Container xml_Mark_Container = XML_Mark_Container.LoadFromText (xmlData);
		
		xml_Mark_Container.Save (sSavePath);
		
		
		string[] filename = sSavePath.Split ('/');
		message = filename [filename.Length - 1] + " "; 

		manager.SaveFileStatus ((int)FileStatus.Success);
		

	}
}
#endif