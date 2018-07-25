using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public class GetInfo : MonoBehaviour
{
	
	private Manager mg;
	private bool isOnce;
	private bool isCopy;
	
	void Awake ()
	{
		mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
	}
	
	void OnGUI ()
	{
		if (Input.GetMouseButtonDown (1) && !mg.sy_CurrentStatus.is3DCam && mg.sy_OtherOption.ScreenSetting.platform != Variables.OtherOptionGroup.Platform.Mobile) {
			isOnce = true;
			isCopy = false;
		}
		if (isOnce) {

			Vector3 pos = Input.mousePosition;
			
			string textAreaString = "";
			string zoomTex = "> Zoom Level: " + mg.sy_Map.zoom + "　";
			string curCoordinTex = "> Center Coordinate" + "\n\n" + mg.sy_Coordinate.center [0] + "\n" + mg.sy_Coordinate.center [1];  
			string mouseCoordinTex = "> Mouse Coordinate" + "\n\n" + mg.sy_Coordinate.mouse [0] + "\n" + mg.sy_Coordinate.mouse [1];  
			textAreaString = zoomTex + "\n\n" + curCoordinTex + "\n\n" + mouseCoordinTex;
			
			GUI.skin.box.fontSize = 15;
			GUI.skin.box.wordWrap = false;
			GUI.skin.box.normal.textColor = Color.white;
			GUI.Box (new Rect (pos.x, Screen.height - pos.y, 160, 200), textAreaString);
			
			//Clipboard//
			if (Input.GetMouseButtonDown (0) && !isCopy) {
				isCopy = true;
				mg.ClipboardCopy(mg.sy_Coordinate.mouse [0] + "," + mg.sy_Coordinate.mouse [1]);
			
				Debug.Log ("Copy from mouse coordinate : " + mg.sy_Coordinate.mouse [0] + "," + mg.sy_Coordinate.mouse [1]);
			}
		}
		if (Input.GetMouseButtonUp (1)) {
			isOnce = false;
		}
	}
}
