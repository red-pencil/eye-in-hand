using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class SceneLoaderUI : MonoBehaviour {

	[Serializable]
	public class SceneInfo
	{
		public int ID;
		public string Name;
	}

	public SceneInfo[] Scenes;
	string[] selStrings = new string[] {"Eyegaze", "Mouse", "Audio"};
	int selectionType;
	// Use this for initialization
	void Start () {
	
	}
	void OnGUI()
	{
		Rect camRect= Camera.main.pixelRect;
		float buttonW = 200;
		float buttonH = 40;
		float H = (buttonH + 10) * Scenes.Length;
		Rect Pos = new Rect ((camRect.width-buttonW)/2,(camRect.height-H)/2,buttonW,buttonH);
		for (int i = 0; i < Scenes.Length; ++i) {
			if (GUI.Button (Pos, Scenes [i].Name)) {
				Debug.Log ("Loading Scene:" + Scenes [i].ID);
				SceneManager.LoadScene (Scenes [i].ID);
			}

			Pos.y += buttonH + 10;
		}

		selectionType = GUI.SelectionGrid(new Rect(25, 25, 300, 30), selectionType, selStrings, selStrings.Length);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
