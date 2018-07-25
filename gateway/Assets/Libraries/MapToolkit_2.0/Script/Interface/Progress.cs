using UnityEngine;
using System.Collections;

public class Progress : MonoBehaviour
{
	
	private Manager mg;
	private int progress_building;
	private int progress_mark;
	
	void Awake ()
	{
		mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
	}
	
	void Update ()
	{
		if (!mg.sy_CurrentStatus.isReady) {
			progress_building = mg.Progress (false, 50);
			progress_mark = mg.Progress (true, 50);
			
			int total = progress_building + progress_mark;

			Debug.Log ("Data DownLoading : " + total);
		}
	}
}
