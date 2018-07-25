#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class XML_AddClear
{
	Xml_Manager manager;
	Vector2 mScroll = Vector2.zero;
	
	public void AddClear ()
	{
		manager = GameObject.Find ("Manager").GetComponent<Xml_Manager> ();
		
		if (GUILayout.Button ("Add Item", GUILayout.Height (30), GUILayout.Width (100f))) {
			AddList ();
		}
			
		if (GUILayout.Button ("Clear Items", GUILayout.Height (30), GUILayout.Width (100f))) {
			manager.ListClear ();
			manager.ResetFileStatus ();
		}	
	}

	public	void AddList ()
	{
		manager.ResetFileStatus ();
		
		int count = manager.xmlDic.Count;
		count++;
		
		string title = "noname";
		title = title + count;
		
		for (int i =0; i <manager.xmlArrayList.Count; i++) {
			Xml_Manager.XmlMarker tmp_xmlData = manager.xmlArrayList [i] as Xml_Manager.XmlMarker;
			if (tmp_xmlData.locationName == title) {
				title = title + "_";
			}
		}
		
		XML_Data xmldata = new XML_Data ();
		manager.xmlArrayList.Add (xmldata.Default (title));

		manager.xmlDic = new Dictionary<string, int> ();

		for (int i=0; i< manager.xmlArrayList.Count; i++) {
			Xml_Manager.XmlMarker tmp_xmlData = manager.xmlArrayList [i] as Xml_Manager.XmlMarker;
			manager.xmlDic.Add (tmp_xmlData.locationName, i);
		}
	}
	
	List<string> mDelNames = new List<string> ();
	List<int> mDelValues = new List<int> ();
	
	public void ItemList ()
	{
		Xml_Manager manager = GameObject.Find ("Manager").GetComponent<Xml_Manager> ();
		
		if (manager.xmlArrayList.Count > 0 && manager.xmlArrayList != null) {
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (3f);
				GUILayout.BeginVertical ();

				mScroll = GUILayout.BeginScrollView (mScroll);

				bool delete = false;
				int index = 0;

				for (int k = 0; k <manager.xmlArrayList.Count; k++) {
					
					Xml_Manager.XmlMarker tmp_xmlData = manager.xmlArrayList [k] as  Xml_Manager.XmlMarker;
					++index;
					GUILayout.Space (-1f);
					
					bool highlight = (manager.selectedMark == tmp_xmlData.locationName);

					GUI.backgroundColor = highlight ? Color.white : new Color (0.8f, 0.8f, 0.8f);
					GUILayout.BeginHorizontal ("AS TextArea", GUILayout.MinHeight (20f));
					GUI.backgroundColor = Color.white;
					GUILayout.Label (index.ToString (), GUILayout.Width (24f));

			
					if (GUILayout.Button (tmp_xmlData.locationName, "OL TextField", GUILayout.Height (20f))) {	
						manager.selection = tmp_xmlData.locationName;
						manager.selection_number = k;
						
						manager.selectListIndex = k;

						manager.SelectMark (manager.selection, manager.selection_number);

					}	
					
					
					if (mDelValues.Contains (k)) {
						GUI.backgroundColor = Color.red;

						if (GUILayout.Button ("OK", GUILayout.Width (30f))) {
							delete = true;
						}
						GUI.backgroundColor = Color.green;
						if (GUILayout.Button ("Delete", GUILayout.Width (50f))) {
							mDelNames.Remove (tmp_xmlData.locationName);
							mDelValues.Remove (k);
							delete = false;
						}
						GUI.backgroundColor = Color.white;
					} else {
						if (GUILayout.Button ("Delete", GUILayout.Width (50f))) {
							mDelNames.Add (tmp_xmlData.locationName);
							mDelValues.Add (k);
						}
					}
	
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndScrollView ();
				GUILayout.EndVertical ();
				GUILayout.Space (3f);
				GUILayout.EndHorizontal ();

				// If this sprite was marked for deletion, remove it from the atlas
				if (delete) {
					for (int i = 0; i< mDelNames.Count; i++) {
						for (int j=0; j<manager.xmlArrayList.Count; j++) {
							Xml_Manager.XmlMarker tmp_xmlData = manager.xmlArrayList [j] as  Xml_Manager.XmlMarker;
							if (tmp_xmlData.locationName == mDelNames [i]) {
								manager.xmlArrayList.RemoveAt (j);
							}
						}				
					}
					mDelValues.Clear ();
					mDelNames.Clear ();		
				}
				if (manager.xmlArrayList.Count == 0)
					manager.SelectMarkClear ();
			}
		}
	}
	
	
}
#endif

