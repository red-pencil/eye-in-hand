using UnityEngine;
using System.Collections;

public class GUITest : MonoBehaviour
{
	Manager mg;

	void Start ()
	{
		mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
	}

	void OnGUI ()
	{
       GUI.Box(new Rect(10,10,150,90), "Camera Moving");

        if(GUI.Button(new Rect(20,40,100,20), "On ")) {
           mg.sy_CurrentStatus.isSearchbar = false;
        }

        if(GUI.Button(new Rect(20,70,100,20), "Off")) {
             mg.sy_CurrentStatus.isSearchbar = true;
        }
	}

}



