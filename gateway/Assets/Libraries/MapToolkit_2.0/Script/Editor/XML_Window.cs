#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class XML_Window : EditorWindow
{
	bool isPlay;
	XML_OpenFile  _open = new XML_OpenFile ();
	XML_SaveFile  _save = new XML_SaveFile ();
	XML_Sample _sample = new XML_Sample ();
	XML_AddClear _addclear = new XML_AddClear ();
	
	public void OnGUI ()
	{	
		GUILayout.Space (10f);
		
		_open.Open ();
		_save.Save ();
		
		GUILayout.Space (8f);
		GUILayout.BeginHorizontal ();
		
		_sample.Sample ();
		_addclear.AddClear ();
		
		
		GUILayout.EndHorizontal ();
		GUILayout.Space (8f);
		
		GUI.backgroundColor = Color.cyan;
		GUIStyle myButtonStyle = new GUIStyle (GUI.skin.box);
		myButtonStyle.normal.textColor = Color.white;

		GUILayout.Box ("Mark XML Item List", myButtonStyle, GUILayout.ExpandWidth (true));
		
		GUI.backgroundColor = Color.white;
		
		GUILayout.Space (5f);
		_addclear.ItemList ();
	}
}
#endif