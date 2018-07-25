using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
public class TxHapticOutput {
	

	[Serializable]
	public class Event:UnityEvent<TxHapticOutput>
	{
	}


	public Vector3 ForceVector;
	public List<float> TactileVector;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}