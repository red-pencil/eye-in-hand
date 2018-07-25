 #if UNITY_EDITOR
using UnityEngine;
using UnityEditor;  
using System.Collections;

[CustomEditor(typeof(MarkXmlUI))]
public class MarkXmlUIInspector : Editor
{
	
	Xml_Manager manager;
	
	enum AlignmentType
	{
		left,
		center,
		right
	}
	AlignmentType mAlignmentType = AlignmentType.left;
	MarkXmlUI markXml_ui;
	
	void OnEnable ()
	{
		manager = GameObject.Find ("Manager").GetComponent<Xml_Manager> ();
		markXml_ui = target as MarkXmlUI;		
	}
		
	public override void OnInspectorGUI ()
	{

		GUILayout.BeginHorizontal ();
		GUI.backgroundColor = Color.cyan;
		GUILayout.Label ("Location Name");
		markXml_ui.name = GUILayout.TextField (markXml_ui.name);
		GUILayout.EndHorizontal ();
		
		
		GUI.backgroundColor = Color.white;


		GUILayout.Space (6);
		
		EditorGUILayout.LabelField ("Longitude  (X)");
		markXml_ui.coordinate_x = EditorGUILayout.TextField (markXml_ui.coordinate_x);
		EditorGUILayout.LabelField ("Latitude  (Y)");
		markXml_ui.coordinate_y = EditorGUILayout.TextField (markXml_ui.coordinate_y);

		GUILayout.Space (10);
		
		
		EditorGUILayout.LabelField ("Mark Image info");
		MarkXmlUI.BeginContents ();
		EditorGUILayout.BeginHorizontal ();
		//GUILayout.Space (10);
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.LabelField ("Nomal", GUILayout.Width (50));

		markXml_ui.mainTexture = EditorGUILayout.ObjectField ("", markXml_ui.mainTexture, typeof(Texture2D), false) as Texture2D;
		EditorGUILayout.EndVertical ();

		
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.LabelField ("Hover", GUILayout.Width (50));
		markXml_ui.overTexture = EditorGUILayout.ObjectField ("", markXml_ui.overTexture, typeof(Texture2D), false) as Texture2D;
		EditorGUILayout.EndVertical ();
		

		EditorGUILayout.BeginVertical ();
		EditorGUILayout.LabelField ("Active", GUILayout.Width (50));
		markXml_ui.activeTexture = EditorGUILayout.ObjectField ("", markXml_ui.activeTexture, typeof(Texture2D), false) as Texture2D;
		EditorGUILayout.EndVertical ();
		
		EditorGUILayout.EndHorizontal ();
		GUILayout.Space (10);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Color");
		markXml_ui.normalColor = EditorGUILayout.ColorField (markXml_ui.normalColor);
		EditorGUILayout.EndHorizontal ();
		
		
		MarkXmlUI.FloatPair ("Pixel Inset", "X", "Y", markXml_ui.normalPixelInset_x, markXml_ui.normalPixelInset_y);

		MarkXmlUI.FloatPair ("", "Width", "Height", markXml_ui.normalPixelInset_w, markXml_ui.normalPixelInset_h);

		GUILayout.Space (20);
		
		
		EditorGUILayout.LabelField ("Mark Text info");
		
		GUILayout.BeginHorizontal ();
		markXml_ui.textShow = EditorGUILayout.Toggle ("Text Show", markXml_ui.textShow, GUILayout.Width (100f));
		GUILayout.Label ("Mark Text show");
		GUILayout.EndHorizontal ();
		
		MarkXmlUI.BeginContents ();
		
		MarkXmlUI.FloatPair ("Pixel Offset", "X", "Y", markXml_ui.textPixelInset_x, markXml_ui.textPixelInset_y);
		markXml_ui.textFontSize = EditorGUILayout.IntField ("Font Size", markXml_ui.textFontSize, GUILayout.MinWidth (30f));
					
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Color");
		markXml_ui.textColor = EditorGUILayout.ColorField (markXml_ui.textColor);
		EditorGUILayout.EndHorizontal ();

		mAlignmentType = (AlignmentType)EditorGUILayout.EnumPopup ("Alignment", mAlignmentType, GUILayout.MinWidth (200f));

		MarkXmlUI.EndContents ();
		MarkXmlUI.EndContents ();
		
		//inspector changed
		if (GUI.changed){
			manager.updateXmlMark (manager.selectListIndex);
		}
	}
}
#endif