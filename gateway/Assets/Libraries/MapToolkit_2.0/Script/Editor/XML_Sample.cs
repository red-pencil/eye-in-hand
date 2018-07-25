#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class XML_Sample
{
	Xml_Manager manager;
	
	public void Sample ()
	{
		manager = GameObject.Find ("Manager").GetComponent<Xml_Manager> ();
		
		if (GUILayout.Button ("Sample", GUILayout.Height (30), GUILayout.Width (100f))) {
			
			manager.ListClear ();
			manager.ResetFileStatus ();
			manager.selectedMark = "";
			
			XML_Data xmldata = new XML_Data ();
			for (int i=0; i<xmldata.Sample().Count; i++) {
				manager.xmlArrayList.Add (xmldata.Sample () [i]);
			}
			
			manager.xmlDic = new Dictionary<string, int> ();

			for (int i=0; i< manager.xmlArrayList.Count; i++) {
				Xml_Manager.XmlMarker tmp_xmlData = manager.xmlArrayList [i] as Xml_Manager.XmlMarker;
				manager.xmlDic.Add (tmp_xmlData.locationName, i);
			}


		}	
	}
}
#endif