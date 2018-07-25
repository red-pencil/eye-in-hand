using UnityEngine;
using System.Collections;
using System;

public class Searchbar : MonoBehaviour
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

	////////////////////////////////////////////////////////////////////////////// < Class> ////////////////////////////////////////////////////////////////////////////////////
	private Manager mg;
	private MenuOption mo = new MenuOption ();

	public GNSSObject home;

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

		Rect SearchbarBox_Rect = new Rect (posX * 0.01f, 10, posX * 1.98f, 40);
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
		
		
		GUI.Box (SearchbarBox_Rect, " ");
		
		GUI.skin.textField.alignment = TextAnchor.MiddleLeft;
		
		GUI.SetNextControlName ("TextField");
		stringToEdit = GUI.TextField (SearchTextfield_Rect, stringToEdit, 50);
		
		if (GUI.GetNameOfFocusedControl () == "TextField") {
			if (needsRefocus == true) {
				TextEditor editor = (TextEditor)GUIUtility.GetStateObject (typeof(TextEditor), GUIUtility.keyboardControl);
//				editor.selectPos = stringToEdit.Length + 1;
//				editor.pos = stringToEdit.Length + 1;
				needsRefocus = false;
			}
		}
		if (stringToEdit.Length > 0 && tmp_str != stringToEdit) {
			auto_strs = mg.AutoComplete (stringToEdit, idleText, mg.sy_Mark.field_name);
			tmp_str = stringToEdit;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////
		/// Geo Coding
		///////////////////////////////////////////////////////////////////////////////////////////////
		if (stringToEdit.Length > 0 && auto_strs.Length == 0) {
			if (Event.current.isKey && Event.current.keyCode == KeyCode.Return) {
				StartCoroutine (GetGeoCode (stringToEdit));
				mg.sy_CurrentStatus.isAutoComplete = false;
			}
		} else if (stringToEdit.Length > 0 && mg.sy_CurrentStatus.isAutoComplete && auto_strs.Length != 0) {
			
			GUI.skin.button.normal.background = Resources.Load ("UI/SearchBox")as Texture2D;

			GUI.skin.button.normal.textColor = Color.black;
			GUI.skin.button.focused.textColor = Color.white;
			GUI.skin.button.alignment = TextAnchor.MiddleLeft;
			int dropboxCount = (int)mg.sy_OtherOption.Autocomplete.dropboxCount;
			dropboxCount = Mathf.Clamp (dropboxCount, 2, auto_strs.Length);
			for (int i = 0; i < auto_strs.Length; i++) {
				if (i < mg.sy_OtherOption.Autocomplete.dropboxCount) {
					GUI.SetNextControlName ("SearchedTerm" + i);
					if (GUI.Button (new Rect (posX * 0.398f, 50 + (i * 30), posX * 0.8f, 30), auto_strs [i])) {	
						SelectMark (auto_strs [i]);
					}
				}
			}
			
			GUI.skin.button.normal.textColor = Color.white;
			GUI.skin.box.normal.textColor = Color.white;
			GUI.skin.button.alignment = TextAnchor.MiddleCenter;
			GUI.skin.button.normal.background = GUI.skin.toggle.normal.background;
			
			if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.DownArrow) {
				if (auto_SelectNum < dropboxCount - 1)
					auto_SelectNum += 1;
				
				if (!searchControlStart)
					auto_SelectNum = 0;
			
				GUI.FocusControl ("SearchedTerm" + auto_SelectNum);
			
				searchControlStart = true;
				isGetSearchButtonName = false;
			}
			////////////////////////////////////////////////////////////////////////////////////////////////
			/// Click UpArrow
			///////////////////////////////////////////////////////////////////////////////////////////////
			else if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.UpArrow) {
				auto_SelectNum -= 1;
				if (auto_SelectNum >= 0) {
					GUI.FocusControl ("SearchedTerm" + auto_SelectNum);
				}
				isGetSearchButtonName = false;
			}
			////////////////////////////////////////////////////////////////////////////////////////////////
			/// Click the Backspace
			///////////////////////////////////////////////////////////////////////////////////////////////
			else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Backspace) {
				needsRefocus = true;
				GUI.FocusControl ("TextField");
			}
			if (searchControlStart && !isGetSearchButtonName) {
				if (auto_SelectNum < 0) {
					needsRefocus = true;
					GUI.FocusControl ("TextField");
					auto_SelectNum = 0;
				}
				auto_SelectNum = Mathf.Clamp (auto_SelectNum, 0, dropboxCount - 1);
				if (auto_SelectNum >= 0)
					currentFocusButtonName = auto_strs [auto_SelectNum];
				isGetSearchButtonName = true;
			}
			////////////////////////////////////////////////////////////////////////////////////////////////
			/// Enter
			///////////////////////////////////////////////////////////////////////////////////////////////
			if (Event.current.isKey && Event.current.keyCode == KeyCode.Return) {
				SelectMark (currentFocusButtonName);
				mg.sy_CurrentStatus.isAutoComplete = false;
			}
		}	
		if (GUI.Button (SeletedMarkInfo_Rect, "Vehicle")) {
			GUI.FocusControl ("");
			StartCoroutine (GoHome ());

		}
		/*
		if (GUI.Button (SeletedMarkInfo_Rect, "Mark List")) {
			GUI.FocusControl ("");
			if (mg.sy_Mark.table_name != null) {
				if (isTable) {
					isTable = false;
					isField = false;
					mg.sy_CurrentStatus.isSearchbar = false;
				} else {
					isTable = true;	
					mg.sy_CurrentStatus.isSearchbar = true;
					mg.sy_CurrentStatus.isMenu = false;
				}
				mg.sy_CurrentStatus.isAutoComplete = false;
				searchControlStart = false;
				auto_SelectNum = 0;
				currentFocusButtonName = "";
				markT = mg.sy_Mark.table_name;
				markF = new string[0];
			}
		}*/
		if (GUI.Button (new Rect (posX * 0.213f, 15, posX * 0.17f, 30), "Menu")) {
			GUI.FocusControl ("");
			if (mg.sy_CurrentStatus.isMenu) {
				mg.sy_CurrentStatus.isMenu = false;
			} else {
				mg.sy_CurrentStatus.isMenu = true;
				isTable = false;
				isField = false;
				mo.GoHome ();
			}
			mg.sy_CurrentStatus.isAutoComplete = false;
			searchControlStart = false;
			auto_SelectNum = 0;
			currentFocusButtonName = "";
		}
		GUI.skin.textField.alignment = TextAnchor.MiddleCenter;
		
		string camMode = "";
		
		if (mg.sy_CurrentStatus.is3DCam)
			camMode = "3D ";
		else
			camMode = "2D ";
		GUI.skin.box.fontSize = 16;
	
		if (GUI.Button (new Rect (posX * 1.213f, 15, posX * 0.17f, 30), "Search")) {
			StartCoroutine (GetGeoCode (stringToEdit));
			mg.sy_CurrentStatus.isAutoComplete = false;
		}
		
		if (GUI.Button (new Rect (posX * 1.4f, 15, posX * 0.17f, 30), camMode)) {
			if (mg.sy_CurrentStatus.is3DCam) {
				mg.GeneralMode ();
			} else {
				mg.VrMode ();
				mg.LimitDistance ();
			}
		}
		
		if (markPointName != "None") {
			GUI.backgroundColor = Color.cyan;
		} else {
			GUI.backgroundColor = Color.white;
		}
		if (GUI.Button (new Rect (posX * 1.586f, 15, posX * 0.385f, 30), markPointName)) {
			mg.sy_CurrentStatus.isMenu = false;
			isTable = false;
			isField = false;
			mg.sy_CurrentStatus.isSearchbar = false;
			mg.sy_CurrentStatus.isAutoComplete = false;
			searchControlStart = false;
			auto_SelectNum = 0;
			currentFocusButtonName = "";
		}
		GUI.backgroundColor = Color.white;
		
		if (GUI.GetNameOfFocusedControl () == "TextField") {
	
			mg.sy_CurrentStatus.isSearchbar = true;
			mg.sy_CurrentStatus.isMenu = false;
			mg.sy_CurrentStatus.isAutoComplete = true;
			searchControlStart = false;
			auto_SelectNum = 0;
			
			currentFocusButtonName = "";
			if (Event.current.isKey && Event.current.keyCode == KeyCode.Return) {
				SelectMark (currentFocusButtonName);
				mg.sy_CurrentStatus.isAutoComplete = false;
			}
			
		}


		if (Input.GetMouseButtonDown (0)) {
		
			if (Event.current != null && Event.current.isMouse) {
				
				if (MarkWindow_Rect.Contains (Event.current.mousePosition)) {
					if (isTable) {
						isTable = true;	
						mg.sy_CurrentStatus.isSearchbar = true;
						mg.sy_CurrentStatus.isMenu = true;
					}
					mg.sy_CurrentStatus.isAutoComplete = false;
				} else if (MarkWindow_Rect2.Contains (Event.current.mousePosition)) {
					if (isField) {
						isField = true;
					} else {
						isTable = false;
						mg.sy_CurrentStatus.isMenu = false;
						mg.sy_CurrentStatus.isSearchbar = false;
					}
					mg.sy_CurrentStatus.isAutoComplete = false;
				} else {
					isTable = false;
					isField = false;
					mg.sy_CurrentStatus.isMenu = false;
					mg.sy_CurrentStatus.isSearchbar = false;
					mg.sy_CurrentStatus.isAutoComplete = false;
				}

			} else if (GUI.GetNameOfFocusedControl () == "TextField") {
				
				isTable = false;
				isField = false;
				mg.sy_CurrentStatus.isMenu = false;
				mg.sy_CurrentStatus.isSearchbar = true;
				mg.sy_CurrentStatus.isAutoComplete = true;
				searchControlStart = false;
				auto_SelectNum = 0;
				currentFocusButtonName = "";
				if (stringToEdit == "Please enter your search terms.")
					stringToEdit = "";
			
			}
			
		}
	
		if (Event.current.functionKey && Event.current.keyCode == mg.sy_OtherOption.UserKey.vrModeKey) {
			GUI.FocusControl ("");
			if (Event.current.type == EventType.keyDown)
				mg.sy_CurrentStatus.isChangeKey = true;
		}
		if (Event.current.type == EventType.KeyUp && Event.current.keyCode == mg.sy_OtherOption.UserKey.vrModeKey) {
			mg.sy_CurrentStatus.isChangeKey = false;
		}

		
		if (mg.sy_CurrentStatus.isMenu) {
			mo.Running ();
		}
			
		if (isTable) {
			GUI.Window (0, MarkWindow_Rect, (GUI.WindowFunction)MarkWindow, "List");	
			if (isField) {
				GUI.Window (1, MarkWindow_Rect2, (GUI.WindowFunction)MarkWindow, tableName);
			}
		}	
	}

	string[] markT;
	string[] markF;

	void MarkWindow (int windowID)
	{

		Rect rec = new Rect (btn_rect_x, 0, btn_width * 0.85f, btn_height * 1.4f);
		Rect tableList = new Rect (0, 0, 0, (btn_height * markT.Length) + ((Interval - btn_height) * markT.Length));
		Rect fieldList = new Rect (0, 0, 0, (btn_height * markF.Length) + ((Interval - btn_height) * markF.Length));
		
		if (windowID.Equals (0)) {
			tableScroll = GUI.BeginScrollView (new Rect (btn_rect_x, (h_value * 0.28f), btn_width, h_value * 3.22f), tableScroll, tableList, false, false);
			DropDownList (markT.Length, rec, Interval, markT, "Table");
			GUI.EndScrollView ();
		} else if (windowID.Equals (1)) {
			fieldScroll = GUI.BeginScrollView (new Rect (btn_rect_x, (h_value * 0.28f), btn_width, h_value * 3.22f), fieldScroll, fieldList, false, false);
			DropDownList (markF.Length, rec, Interval, markF, "Field");
			GUI.EndScrollView ();
		}
		
	}

	string tableName;
	int tableNum = 0;

	void DropDownList (int numRows, Rect rec, float Interval, string[] tile, string option)
	{	
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		Array.Sort (tile);
		for (int i = 0; i < numRows; i++) {
			bool fClicked = false;
			string rowLabel = (i + 1) + ". " + tile [i].Split ('.') [0];
			GUI.skin.button.alignment = TextAnchor.MiddleLeft;
			fClicked = GUI.Button (rec, rowLabel);
			if (fClicked) {
				if (option.Equals ("Table")) {
					tableName = tile [i].Split ('.') [0];
					markF = mg.sy_Mark.field_name [i];
					tableNum = i;

				} else if (option.Equals ("Field")) {
					mg.sy_CurrentStatus.isMarkPoint = true;
					GameObject markTarget = GameObject.Find ("Target");
					Transform _target = markTarget.transform.GetChild (tableNum).GetChild (i).transform;

					mg.SetMarkTarget (_target);
			
					if (Math.Abs (_target.position.x) > limitsPos) {
						
						string str = GameObject.Find ("Setting").transform.GetChild (tableNum).GetChild (i).GetComponent<MarkPoint> ().markCoordinate;
						string[] s = str.Split (',');
						mg.sy_Map.longitude_x = double.Parse (s [0]);
						mg.sy_Map.latitude_y = double.Parse (s [1]);
						mg.ChangeZoom ();
						if (mg.sy_CurrentStatus.is3DCam) {
							double[] _double = { double.Parse (s [0]), double.Parse (s [1]) };
							GetBuilding (_double);

						}

					} else {
						mg.RunMarkMove ();
						if (mg.sy_CurrentStatus.is3DCam) {
							string[] s = mg.sy_Mark.coordinate [tableNum] [i].Split (',');
							double[] _double = { double.Parse (s [0]), double.Parse (s [1]) };
							GetBuilding (_double);
						}
					}
				}
				
				isField = true;
			}
			rec.y += Interval;
		}
	}

	void SelectMark (string markName)
	{
		  
		stringToEdit = markName;
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		mg.sy_CurrentStatus.isMarkPoint = false;
		GameObject target = GameObject.Find ("Mark").transform.Find ("Target").gameObject;
		for (int k = 0; k < target.transform.childCount; k++) {
			GameObject _target = target.transform.GetChild (k).gameObject;
			for (int j = 0; j < _target.transform.childCount; j++) {
				if (_target.transform.GetChild (j).name == markName) {
					mg.SetMarkTarget (_target.transform.GetChild (j).transform);
					if (Math.Abs (_target.transform.GetChild (j).transform.position.x) > limitsPos) {
						string str = GameObject.Find ("Setting").transform.GetChild (k).GetChild (j).GetComponent<MarkPoint> ().markCoordinate;
						string[] s = str.Split (',');
						
						mg.sy_Map.longitude_x = double.Parse (s [0]);
						mg.sy_Map.latitude_y = double.Parse (s [1]);
						mg.ChangeZoom ();

						mg.sy_CurrentStatus.isMarkPoint = true;
						
						if (mg.sy_CurrentStatus.is3DCam) {
							double[] _double = { double.Parse (s [0]), double.Parse (s [1]) };
							GetBuilding (_double);
						}
								
						break;
					} else {
						mg.RunMarkMove ();
						mg.sy_CurrentStatus.isMarkPoint = true;
						if (mg.sy_CurrentStatus.is3DCam) {
							string[] s = mg.sy_Mark.coordinate [k] [j].Split (',');
							double[] _double = { double.Parse (s [0]), double.Parse (s [1]) };
							GetBuilding (_double);
						}
					}
				}
			}
		}
	}

	



	public IEnumerator GoHome ()
	{

		if (home != null) {
			mg.sy_Map.longitude_x =  (home.location [1]);
			mg.sy_Map.latitude_y =  (home.location[0]);
			mg.sy_Map.zoom = 17;
			mg.ChangeZoom ();
		}
		yield return 0;
	}
	public IEnumerator GetGeoCode (string keyword)
	{
		yield return StartCoroutine (mg.GetGeoData (keyword, mg.sy_Config.phpurl));	
		geoData = mg.sy_GeoData;
		
		if (geoData != null) {
			mg.sy_Map.longitude_x = double.Parse (geoData [0]);
			mg.sy_Map.latitude_y = double.Parse (geoData [1]);
			mg.ChangeZoom ();
			stringToEdit = geoData [2];
			if (mg.sy_CurrentStatus.is3DCam) {
				double[] _double = { double.Parse (geoData [0]), double.Parse (geoData [1]) };
				GetBuilding (_double);	
			}
		}
	}

	public void GetBuilding (double[] target)
	{
		for (int k = 0; k < mg.sy_Building.coordinate.Count; k++) {
			string[] str = mg.sy_Building.coordinate [k].Split (',');
			double dis = mg.GetDistanceP1toP2 (target [1], target [0], double.Parse (str [1]), double.Parse (str [0]));
		
			if (limits () != 0 && dis < limits ()) {
				mg.ImportModeling (mg.sy_Building.name [k], target);
			}
		}	
	}

	private	int limits ()
	{
		Manager mg = (Manager)GameObject.Find ("Manager").GetComponent<Manager> ();
		int zoom = mg.sy_Map.zoom;
		int zoomMax = 20;
		for (int i = 0; i < 8; i++) {
			if (zoom == zoomMax - i) {
				return 	(int)mg.sy_OtherOption.Camera.distance [i];
			} 
		}
		return 0;	
	}
	


}
