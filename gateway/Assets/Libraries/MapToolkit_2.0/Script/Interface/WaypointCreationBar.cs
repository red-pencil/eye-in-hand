using UnityEngine;
using System.Collections;
using System;

public class WaypointCreationBar : MonoBehaviour
{

	////////////////////////////////////////////////////////////////////////////// <Variables> ////////////////////////////////////////////////////////////////////////////////////
	private float posX;
	private float posY;
	private float w_value;
	private float h_value;
	private float btn_rect_x;
	private float btn_width;
	private float btn_height;
	private float Interval;
	private Vector2 tableScroll;
	private Vector2 fieldScroll;
	private bool isTable;
	private bool isField;
	private bool isGetSearchButtonName;
	private bool searchControlStart = false;
	private bool needsRefocus = false;
	private string idleText = "Please enter your search terms.";
	private string stringToEdit;
	private string tmp_str;
	private string currentFocusButtonName;
	private string[] auto_strs;
	private string[] geoData;
	private int auto_SelectNum = 0;
	int limitsPos = 5000;

	public GNSSRoute route;


	////////////////////////////////////////////////////////////////////////////// < Class> ////////////////////////////////////////////////////////////////////////////////////
	private Manager mg;
	private MenuOption mo = new MenuOption ();

	void Awake ()
	{
		mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		stringToEdit = idleText;
	}

	public void OnGUI ()
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		
		posX = Screen.width / 2;
		posY = Screen.height / 2;
		
		float _posX = posX - (posX * 0.4f * 0.5f);
		float _posY = posY - (posY * 0.94f * 0.5f);
		
		w_value = _posX * 0.5f;
		h_value = _posY * 0.5f;
			
		btn_rect_x = w_value * 0.03f;
		btn_width = w_value * 0.92f;
		btn_height = h_value * 0.3f;
		
		Interval = btn_height * 1.53f;
		
		if (Screen.height <= 600) {
			GUI.skin.button.fontSize = 12;
			GUI.skin.label.fontSize = 12;
			GUI.skin.textField.fontSize = 12;
		} else {
			GUI.skin.button.fontSize = 16;
			GUI.skin.label.fontSize = 16;
			GUI.skin.textField.fontSize = 16;
		}
		GUI.skin.button.normal.textColor = Color.white;
		GUI.skin.box.normal.textColor = Color.white;
		GUI.skin.button.alignment = TextAnchor.MiddleCenter;
		GUI.skin.button.normal.background = GUI.skin.toggle.normal.background;

		Rect WaypointBox_Rect = new Rect (posX * 0.01f, 10, posX * 1.98f, 40);
		Rect SearchTextfield_Rect = new Rect (posX * 0.398f, 15, posX * 0.8f, 30);
		Rect SeletedMarkInfo_Rect = new Rect (posX * 0.026f, 15, (posX * 0.17f), 30);
		Rect MarkWindow_Rect = new Rect (posX * 0.01f, 54, posX * 0.4f, posY * 0.938f);
		Rect MarkWindow_Rect2 = new Rect (posX * 0.416f, 54, posX * 0.4f, posY * 0.938f);
		
		string markPointName = "";
		if (mg.GetMarkTarget () != null && mg.sy_CurrentStatus.isMarkPoint) {
			markPointName = mg.GetMarkTarget ().name;
		} else {
			markPointName = "None";
		}

		GUI.Box (WaypointBox_Rect, " ");

		float width = posX * 0.17f;
		float offsetX = 0;
		if (GUI.Button (new Rect (posX * 0.026f+offsetX, 15, width, 30), "Start")) {
			route.StartCreation ();
		}
		offsetX += width;
		if (GUI.Button (new Rect (posX * 0.026f+offsetX, 15, width,30), "Stop")) {
			route.StopCreation ();
		}
		offsetX += width;
		if (GUI.Button (new Rect (posX * 0.026f+offsetX, 15, width,30), "Remove")) {
			route.RemoveLast ();
		}
		offsetX += width;
		if (GUI.Button (new Rect (posX * 0.026f+offsetX, 15, width,30), "Clear")) {
			route.ClearPoints ();
		}
		offsetX += width;



	}


}
