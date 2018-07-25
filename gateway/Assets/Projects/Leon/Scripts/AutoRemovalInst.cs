using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRemovalInst : MonoBehaviour {

	float _timeOut=0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		_timeOut += Time.deltaTime;
		if (_timeOut > 2) {
			GameObject.Destroy (gameObject);
		}
	}
}
